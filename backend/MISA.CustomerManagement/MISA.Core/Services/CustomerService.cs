using MISA.Core.Dtos;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Services
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
            : base(customerRepository)
        {
            _customerRepository = customerRepository;
        }
        // Sinh mã khách hàng mới (chỉ trả về code, không insert)
        public string GenerateCustomerCode()
        {
            return _customerRepository.GenerateCustomerCode();
        }


        /// <summary>
        /// Thêm mới Customer với mã tự sinh và validate dữ liệu
        /// </summary>
        /// <param name="entity">Khách hàng cần thêm</param>
        /// <returns>Customer vừa thêm</returns>
        /// CreatedBy: Duchc (15/11/2025)
        public override Customer Insert(Customer entity)
        {
            entity.CustomerId = Guid.NewGuid();

            // CHỈ sinh code nếu FE không gửi sẵn
            if (string.IsNullOrWhiteSpace(entity.CustomerCode))
                entity.CustomerCode = _customerRepository.GenerateCustomerCode();

            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            entity.IsDelete = "1";

            ValidateCustomer(entity);
            _customerRepository.Insert(entity);
            return entity;
        }

        /// <summary>
        /// Cập nhật Customer theo Id với validate và giữ nguyên mã cũ
        /// </summary>
        /// <param name="id">Id Customer cần cập nhật</param>
        /// <param name="entity">Thông tin Customer mới</param>
        /// <returns>Customer đã cập nhật</returns>
        /// CreatedBy: Duchc (15/11/2025)
        public override Customer Update(Guid id, Customer entity)
        {
            var existingCustomer = _customerRepository.GetById(id);
            if (existingCustomer == null)
                throw new Exception("Customer not found");

            entity.CustomerId = id;
            entity.CustomerCode = existingCustomer.CustomerCode;
            entity.CreatedAt = existingCustomer.CreatedAt;
            entity.UpdatedAt = DateTime.Now;
            entity.IsDelete = existingCustomer.IsDelete; // giữ trạng thái delete hiện tại
            ValidateCustomer(entity, id);

            _customerRepository.Update(entity, id);

            return entity;
        }

        /// <summary>
        /// Validate thông tin Customer (tên, email, điện thoại) trước khi thêm/cập nhật
        /// </summary>
        /// <param name="entity">Customer cần validate</param>
        /// <param name="currentId">Id hiện tại (nếu cập nhật)</param>
        /// <returns>Không có giá trị trả về, throw Exception nếu lỗi</returns>
        /// CreatedBy: Duchc (15/11/2025)
        private void ValidateCustomer(Customer entity, Guid? currentId = null)
        {
            if (string.IsNullOrWhiteSpace(entity.FullName))
                throw new Exception("Customer full name is required.");

            if (!string.IsNullOrWhiteSpace(entity.Phone))
            {
                if (!entity.Phone.All(char.IsDigit) || entity.Phone.Length < 10 || entity.Phone.Length > 11)
                    throw new Exception("Phone must be 10–11 digits.");
            }

            if (!string.IsNullOrWhiteSpace(entity.Email))
            {
                if (_customerRepository.IsEmailExists(entity.Email, currentId))
                    throw new Exception("Email already exists.");
            }

            if (!string.IsNullOrWhiteSpace(entity.Phone))
            {
                if (_customerRepository.IsPhoneExists(entity.Phone, currentId))
                    throw new Exception("Phone already exists.");
            }
        }

        /// <summary>
        /// Lấy danh sách Customer phân trang, có thể search và sort
        /// </summary>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Số bản ghi mỗi trang</param>
        /// <param name="search">Từ khóa tìm kiếm</param>
        /// <param name="sortBy">Trường sắp xếp</param>
        /// <param name="sortDir">Chiều sắp xếp (asc/desc)</param>
        /// <returns>PagingResult chứa dữ liệu và tổng số bản ghi</returns>
        /// CreatedBy: Duchc (16/11/2025)
        public PagingResult<Customer> GetPaging(int pageNumber, int pageSize, string? search, string? sortBy, string? sortDir, string? customerType)
        {
            return _customerRepository.GetPaging(pageNumber, pageSize, search, sortBy, sortDir,customerType );
        }



        /// <summary>
        /// Kiểm tra định dạng thông tin Customer trước khi import từ CSV
        /// </summary>
        /// <param name="c">Customer cần validate định dạng</param>
        /// <returns>Không có giá trị trả về, ném Exception nếu dữ liệu không hợp lệ</returns>
        /// CreatedBy: Duchc (17/11/2025)
        private void ValidateCustomerFormat(Customer c)
        {
            if (string.IsNullOrWhiteSpace(c.FullName))
                throw new Exception("Full name is required.");

            if (!string.IsNullOrWhiteSpace(c.Phone))
            {
                if (!c.Phone.All(char.IsDigit) || c.Phone.Length < 10 || c.Phone.Length > 11)
                    throw new Exception("Phone must be 10–11 digits.");
            }

            if (!string.IsNullOrWhiteSpace(c.Email))
            {
                if (!c.Email.Contains("@"))
                    throw new Exception("Email format is invalid.");
            }
        }

        /// <summary>
        /// Nhập danh sách Customer từ file CSV, tính số duplicate email/phone
        /// </summary>
        /// <param name="csvStream">Stream CSV</param>
        /// <param name="fileName">Tên file CSV</param>
        /// <param name="duplicatePhones">Số điện thoại trùng (out)</param>
        /// <param name="duplicateEmails">Email trùng (out)</param>
        /// <returns>Số Customer được thêm thành công</returns>
        /// CreatedBy: Duchc (17/11/2025)
        public int ImportFromCsv(Stream csvStream, string fileName,
            out int duplicatePhones, out int duplicateEmails)
        {
            var customers = new List<Customer>();

            using (var reader = new StreamReader(csvStream))
            {
                string? header = reader.ReadLine();
                if (header == null) throw new Exception("CSV is empty");

                var headers = header.Split(',')
                                    .Select(h => h.Trim().ToLowerInvariant())
                                    .ToArray();

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var cols = line.Split(',');

                    string Get(string name)
                    {
                        int index = Array.IndexOf(headers, name);
                        return index >= 0 ? cols[index].Trim() : "";
                    }

                    var c = new Customer
                    {
                        CustomerId = Guid.NewGuid(),
                        FullName = Get("fullname") != "" ? Get("fullname") : Get("full_name"),
                        Phone = Get("phone"),
                        Email = Get("email"),
                        CustomerType = Get("customertype") != "" ? Get("customertype") : Get("customer_type"),
                        TaxCode = Get("taxcode") != "" ? Get("taxcode") : Get("tax_code"),
                        ShippingAddress = Get("shippingaddress") != "" ? Get("shippingaddress") : Get("shipping_address"),
                        BillingAddress = Get("billingaddress") != "" ? Get("billingaddress") : Get("billing_address"),
                        LastPurchasedItem = Get("lastpurchaseditem") != "" ? Get("lastpurchaseditem") : Get("last_purchased_item"),
                        PurchasedSummary = Get("purchasedsummary") != "" ? Get("purchasedsummary") : Get("purchased_summary"),
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    if (DateTime.TryParse(Get("lastpurchaseddate"), out var dt))
                        c.LastPurchaseDate = dt;

                    // Chỉ validate format
                    ValidateCustomerFormat(c);

                    customers.Add(c);
                }
            }

            if (!customers.Any())
            {
                duplicatePhones = 0;
                duplicateEmails = 0;
                return 0;
            }

            // giao toàn quyền cho repository xử lý trùng + insert
            var result = _customerRepository.BulkInsertWithDuplicates(customers);

            duplicatePhones = result.duplicatePhones;
            duplicateEmails = result.duplicateEmails;

            return result.inserted;
        }

    }

}
