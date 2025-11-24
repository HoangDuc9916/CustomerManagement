using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Services;

namespace MISA.CustomerManagement.Api.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : MISABaseController<Customer>
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService) : base(customerService)
        {
            _customerService = customerService;
        }

        // GET /api/articles?search=&sortBy=&sortDir=&pageNumber=&pageSize=
        [HttpGet("articles")]
        public IActionResult GetArticles(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDir = "desc",
            [FromQuery] string? customerType = null)
        {
            try
            {
                var paging = _customerService.GetPaging(pageNumber, pageSize, search, sortBy, sortDir, customerType);
                var meta = new { page = paging.PageNumber, pageSize = paging.PageSize, total = paging.TotalRecords };
                return Ok(new { data = paging.Data, meta, error = (object)null });
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = (object)null, meta = (object)null, error = ex.Message });
            }
        }

        // POST /api/customers/import-csv
        [HttpPost("import-csv")]
        public IActionResult ImportCsv([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { data = (object)null, meta = (object)null, error = "File is empty" });

            using (var stream = file.OpenReadStream())
            {
                try
                {
                    var inserted = _customerService.ImportFromCsv(
                        stream,
                        file.FileName,
                        out int duplicatePhones,
                        out int duplicateEmails
                    );

                    return Ok(new
                    {
                        data = new { inserted, duplicatePhones, duplicateEmails },
                        meta = (object)null,
                        error = (object)null
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { data = (object)null, meta = (object)null, error = ex.Message });
                }
            }
        }

        [HttpGet("new-code")]
        public IActionResult GetNewCustomerCode()
        {
            var code = _customerService.GenerateCustomerCode();
            return Ok(code);
        }
    }
}
