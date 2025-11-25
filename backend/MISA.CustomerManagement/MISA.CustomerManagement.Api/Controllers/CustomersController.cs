using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Core.Dtos;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Services;
using MISA.Core.Services;
using OfficeOpenXml;

using ClosedXML.Excel;

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



[HttpPost("export")]
    public async Task<IActionResult> ExportExcel([FromBody] ExportRequest request)
    {
        // Lấy dữ liệu từ service
        var data = await _customerService.ExportCustomersAsync(request.Ids, request.Columns);

        // Nếu columns rỗng, lấy tất cả property của object
        var columns = (request.Columns != null && request.Columns.Any())
            ? request.Columns
            : data.FirstOrDefault()?.GetType().GetProperties().Select(p => p.Name).ToList() ?? new List<string>();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Customers");

        // Ghi header
        for (int c = 0; c < columns.Count; c++)
            ws.Cell(1, c + 1).Value = columns[c];

            // Ghi dữ liệu
            for (int r = 0; r < data.Count; r++)
            {
                for (int c = 0; c < columns.Count; c++)
                {
                    var prop = data[r].GetType().GetProperty(columns[c]);
                    var value = prop?.GetValue(data[r]);

                    // Convert tất cả sang string để tránh lỗi với Guid, null, DateTime...
                    ws.Cell(r + 2, c + 1).Value = value?.ToString() ?? string.Empty;
                }
            }


            // Xuất file
            using var stream = new MemoryStream();
        wb.SaveAs(stream);

        var fileName = $"customers_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

}
}
