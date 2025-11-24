using MISA.Core.Dtos;
using MISA.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Interfaces.Services
{
    public interface ICustomerService : IBaseService<Customer>
    {
        PagingResult<Customer> GetPaging(int pageNumber, int pageSize, string? search, string? sortBy, string? sortDir, string? customerType);
        int ImportFromCsv(Stream csvStream, string fileName, out int duplicatePhones, out int duplicateEmails);


        // Sinh mã khách hàng mới
        string GenerateCustomerCode();

    }
}
