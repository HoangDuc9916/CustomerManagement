using MISA.Core.Dtos;
using MISA.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Interfaces.Repository
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        //Check unique
        bool IsEmailExists(string email, Guid? excludeId = null);
        bool IsPhoneExists(string phone, Guid? excludeId = null);


        //Sinh mã KH theo yêu cầu
        string GenerateCustomerCode();

        PagingResult<Customer> GetPaging(int pageNumber, int pageSize, string? search, string? sortBy, string? sortDir, string? customerType);

        (int inserted, int duplicatePhones, int duplicateEmails) BulkInsertWithDuplicates(List<Customer> customers);

        Task<List<Customer>> ExportCustomersAsync(List<string> ids, List<string> columns);

    }
}
