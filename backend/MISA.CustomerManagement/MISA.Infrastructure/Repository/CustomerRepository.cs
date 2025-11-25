using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.Core.Dtos;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Infrastructure.Repository
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IConfiguration config)
            : base(config, "customer", "customer_id")
        {
        }

        /// <summary>
        /// Sinh mã Customer tiếp theo
        /// </summary>
        /// <returns>Chuỗi CustomerCode mới</returns>
        /// CreatedBy: Duchc (15/11/2025)
        public string GenerateCustomerCode()
        {
            var lastCodeSql = "SELECT customer_code FROM customer ORDER BY customer_code DESC LIMIT 1";
            var lastCode = dbConnection.QueryFirstOrDefault<string>(lastCodeSql)
                           ?? "KH" + DateTime.Now.ToString("yyyyMM") + "000000";
            long numberPart = long.Parse(lastCode.Substring(8)) + 1;
            return "KH" + DateTime.Now.ToString("yyyyMM") + numberPart.ToString("D6");
        }

        /// <summary>
        /// Kiểm tra email đã tồn tại (ngoại trừ currentId nếu cập nhật)
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <param name="currentId">Id Customer hiện tại (optional)</param>
        /// <returns>true nếu email tồn tại, false nếu chưa tồn tại</returns>
        /// CreatedBy: Duchc (16/11/2025)
        public bool IsEmailExists(string email, Guid? currentId = null)
        {
            string sql = "SELECT COUNT(1) FROM customer WHERE email = @Email";
            if (currentId.HasValue)
                sql += " AND customer_id <> @Id";

            int count = dbConnection.ExecuteScalar<int>(sql, new { Email = email, Id = currentId });
            return count > 0;
        }

        /// <summary>
        /// Kiểm tra phone đã tồn tại (ngoại trừ currentId nếu cập nhật)
        /// </summary>
        /// <param name="phone">Phone cần kiểm tra</param>
        /// <param name="currentId">Id Customer hiện tại (optional)</param>
        /// <returns>true nếu phone tồn tại, false nếu chưa tồn tại</returns>
        /// CreatedBy: Duchc (16/11/2025)
        public bool IsPhoneExists(string phone, Guid? currentId = null)
        {
            string sql = "SELECT COUNT(1) FROM customer WHERE phone = @Phone";
            if (currentId.HasValue)
                sql += " AND customer_id <> @Id";

            int count = dbConnection.ExecuteScalar<int>(sql, new { Phone = phone, Id = currentId });
            return count > 0;
        }

        /// <summary>
        /// Thêm mới Customer vào DB
        /// </summary>
        /// <param name="customer">Customer cần thêm</param>
        /// <returns>Số bản ghi thêm</returns>
        /// CreatedBy: Duchc (15/11/2025)
        public override int Insert(Customer customer)
        {
            customer.IsDelete = "1";            // mặc định khách hàng mới chưa xóa

            string sql = @"
INSERT INTO customer
(customer_id, customer_code, full_name, email, phone, customer_type, tax_code, shipping_address, billing_address, last_purchase_date, purchased_summary, last_purchased_item, avatar_url, isDelete, created_at, updated_at)
VALUES
(@CustomerId, @CustomerCode, @FullName, @Email, @Phone, @CustomerType, @TaxCode, @ShippingAddress, @BillingAddress, @LastPurchaseDate, @PurchasedSummary, @LastPurchasedItem, @AvatarUrl, @IsDelete, @CreatedAt, @UpdatedAt)";

            return dbConnection.Execute(sql, customer);
        }

        /// <summary>
        /// Cập nhật Customer trong DB
        /// </summary>
        /// <param name="customer">Customer cần cập nhật</param>
        /// <param name="id">Id Customer</param>
        /// <returns>Số bản ghi cập nhật</returns>
        /// CreatedBy: Duchc (15/11/2025)
        public override int Update(Customer customer, Guid id)
        {
            customer.CustomerId = id;
            customer.UpdatedAt = DateTime.UtcNow;
            string sql = @"
UPDATE customer SET
    full_name = @FullName,
    email = @Email,
    phone = @Phone,
    customer_type = @CustomerType,
    tax_code = @TaxCode,
    shipping_address = @ShippingAddress,
    billing_address = @BillingAddress,
    last_purchase_date = @LastPurchaseDate,
    purchased_summary = @PurchasedSummary,
    last_purchased_item = @LastPurchasedItem,
    avatar_url = @AvatarUrl,
    updated_at = @UpdatedAt
WHERE customer_id = @CustomerId";

            return dbConnection.Execute(sql, customer);
        }

        /// <summary>
        /// Lấy danh sách Customer phân trang, có thể tìm kiếm và sắp xếp theo trường bất kỳ
        /// </summary>
        /// <param name="pageNumber">Số trang cần lấy (bắt đầu từ 1)</param>
        /// <param name="pageSize">Số lượng bản ghi trên mỗi trang</param>
        /// <param name="search">Từ khóa tìm kiếm theo full name, email hoặc phone (optional)</param>
        /// <param name="sortBy">Tên trường muốn sắp xếp (mặc định "created_at")</param>
        /// <param name="sortDir">Chiều sắp xếp: "asc" hoặc "desc" (mặc định "desc")</param>
        /// <returns>
        /// Trả về một đối tượng PagingResult chứa:
        /// - Data: danh sách Customer cho trang hiện tại
        /// - TotalRecords: tổng số Customer thỏa điều kiện
        /// - PageNumber: số trang hiện tại
        /// - PageSize: số bản ghi mỗi trang
        /// </returns>
        /// CreatedBy: Duchc (16/11/2025)
       /* public PagingResult<Customer> GetPaging(int pageNumber, int pageSize, string? search, string? sortBy, string? sortDir, string? customerType)
        {
            // Chuẩn hóa sort
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "created_at" : ToSnakeCase(sortBy);
            sortDir = (sortDir ?? "desc").ToLower() == "asc" ? "ASC" : "DESC";

            int offset = (pageNumber - 1) * pageSize;

            // Base condition: chỉ lấy bản ghi chưa xóa
            string whereSql = " WHERE isDelete = 1";

            // ----- Search -----
            if (!string.IsNullOrWhiteSpace(search))
            {
                whereSql += @" AND (
                                full_name LIKE CONCAT('%', @Search, '%') OR
                                email LIKE CONCAT('%', @Search, '%') OR
                                phone LIKE CONCAT('%', @Search, '%'))";
            }

            // ----- Filter theo loại khách hàng -----
            if (!string.IsNullOrWhiteSpace(customerType))
            {
                whereSql += " AND customer_type = @CustomerType";
            }

            // ----- Count query -----
            string countSql = $"SELECT COUNT(1) FROM {tableName} {whereSql}";

            // ----- Data query -----
            string dataSql = $@"
                                SELECT *
                                FROM {tableName}
                                {whereSql}
                                ORDER BY {sortBy} {sortDir}
                                LIMIT @Offset, @PageSize;
                                ";

            var parameters = new
            {
                Search = search,
                CustomerType = customerType,
                Offset = offset,
                PageSize = pageSize
            };

            int total = dbConnection.ExecuteScalar<int>(countSql, parameters);
            var data = dbConnection.Query<Customer>(dataSql, parameters).ToList();

            return new PagingResult<Customer>
            {
                TotalRecords = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = data
            };
        }

*/

        public PagingResult<Customer> GetPaging(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortBy,
            string? sortDir,
            string? customerType)
        {
            // Whitelist
            var allowedSort = new HashSet<string>
    {
        "full_name",
        "email",
        "phone",
        "created_at",
        "customer_type"
    };

            // Chuẩn hóa sort
            sortBy = ToSnakeCase(sortBy ?? "created_at");
            if (!allowedSort.Contains(sortBy))
                sortBy = "created_at";

            sortDir = (sortDir?.ToLower() == "asc") ? "ASC" : "DESC";

            int offset = (pageNumber - 1) * pageSize;

            string whereSql = " WHERE isDelete = 1";

            if (!string.IsNullOrWhiteSpace(search))
            {
                whereSql += @" AND (
                        full_name LIKE CONCAT('%', @Search, '%') OR
                        email LIKE CONCAT('%', @Search, '%') OR
                        phone LIKE CONCAT('%', @Search, '%'))";
            }

            if (!string.IsNullOrWhiteSpace(customerType))
            {
                whereSql += " AND customer_type = @CustomerType";
            }

            string countSql = $"SELECT COUNT(1) FROM {tableName} {whereSql}";

            string dataSql = $@"
        SELECT *
        FROM {tableName}
        {whereSql}
        ORDER BY {sortBy} {sortDir}
        LIMIT @Offset, @PageSize;
    ";

            var parameters = new
            {
                Search = search,
                CustomerType = customerType,
                Offset = offset,
                PageSize = pageSize
            };

            int total = dbConnection.ExecuteScalar<int>(countSql, parameters);
            var data = dbConnection.Query<Customer>(dataSql, parameters).ToList();

            return new PagingResult<Customer>
            {
                TotalRecords = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = data
            };
        }

        /// <summary>
        /// Thêm hàng loạt Customer vào database từ danh sách, đồng thời kiểm tra trùng email và phone
        /// </summary>
        /// <param name="customers">Danh sách Customer cần thêm</param>
        /// <returns>
        /// Trả về tuple gồm:
        /// - inserted: số Customer được thêm thành công
        /// - duplicatePhones: số điện thoại bị trùng và không thêm
        /// - duplicateEmails: số email bị trùng và không thêm
        /// </returns>
        /// CreatedBy: Duchc (16/11/2025)
        public (int inserted, int duplicatePhones, int duplicateEmails) BulkInsertWithDuplicates(List<Customer> customers)
        {
            int inserted = 0;
            int duplicatePhones = 0;
            int duplicateEmails = 0;

            if (dbConnection.State != ConnectionState.Open)
                dbConnection.Open();

            string lastCode = dbConnection.QueryFirstOrDefault<string>(
                "SELECT customer_code FROM customer ORDER BY customer_code DESC LIMIT 1"
            ) ?? "KH" + DateTime.Now.ToString("yyyyMM") + "000000";

            long numberPart = long.Parse(lastCode.Substring(8));

            string sql = @"
        INSERT INTO customer
        (customer_id, customer_code, full_name, email, phone, customer_type, 
        tax_code, shipping_address, billing_address, last_purchase_date,
        last_purchased_item,
        purchased_summary, created_at, updated_at)
        VALUES
        (@CustomerId, @CustomerCode, @FullName, @Email, @Phone, @CustomerType, 
         @TaxCode, @ShippingAddress, @BillingAddress, @LastPurchaseDate,
         @LastPurchasedItem, @PurchasedSummary, @CreatedAt, @UpdatedAt)";

            using (var tran = dbConnection.BeginTransaction())
            {
                try
                {
                    foreach (var c in customers)
                    {
                        bool skip = false;

                        // --- CHECK PHONE ---
                        if (!string.IsNullOrWhiteSpace(c.Phone))
                        {
                            var existsPhone = dbConnection.ExecuteScalar<int>(
                                "SELECT COUNT(1) FROM customer WHERE phone=@Phone",
                                new { Phone = c.Phone },
                                tran
                            );

                            if (existsPhone > 0)
                            {
                                duplicatePhones++;
                                skip = true;
                            }
                        }

                        // --- CHECK EMAIL ---
                        if (!skip && !string.IsNullOrWhiteSpace(c.Email))
                        {
                            var existsEmail = dbConnection.ExecuteScalar<int>(
                                "SELECT COUNT(1) FROM customer WHERE email=@Email",
                                new { Email = c.Email },
                                tran
                            );

                            if (existsEmail > 0)
                            {
                                duplicateEmails++;
                                skip = true;
                            }
                        }

                        if (skip) continue;

                        // Sinh mã KH
                        numberPart++;
                        c.CustomerCode = "KH" + DateTime.Now.ToString("yyyyMM") + numberPart.ToString("D6");

                        dbConnection.Execute(sql, c, tran);
                        inserted++;
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }

            return (inserted, duplicatePhones, duplicateEmails);
        }

        /// <summary>
        /// Chuyển tên trường từ PascalCase hoặc camelCase sang snake_case
        /// </summary>
        /// <param name="input">Chuỗi cần chuyển đổi</param>
        /// <returns>Chuỗi đã chuyển sang định dạng snake_case</returns>
        /// CreatedBy: Duchc (16/11/2025)
        private string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (input.Contains("_")) return input.ToLowerInvariant();

            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (char.IsUpper(c))
                {
                    if (i > 0) sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }



        public async Task<List<Customer>> ExportCustomersAsync(List<string> ids, List<string> columns)
        {
            if (ids == null || !ids.Any())
                return new List<Customer>();

            // Loại bỏ null
            ids = ids.Where(x => !string.IsNullOrEmpty(x)).ToList();

            if (!ids.Any())
                return new List<Customer>();

            using (var connection = dbConnection)
            {
                string selectedCols = columns != null && columns.Any()
                    ? string.Join(",", columns)
                    : "*";

                string sql = $@"
            SELECT {selectedCols}
            FROM customer
            WHERE customer_id IN @Ids;
        ";

                var result = await connection.QueryAsync<Customer>(sql, new { Ids = ids });
                return result.ToList();
            }
        }

    }

}
