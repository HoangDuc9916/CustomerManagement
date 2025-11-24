using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Core.Interfaces.Services;

namespace MISA.CustomerManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class MISABaseController<T> : ControllerBase where T : class
    {
        protected readonly IBaseService<T> _baseService;

        public MISABaseController(IBaseService<T> baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public virtual IActionResult GetAll()
        {
            var data = _baseService.GetAll();
            return Ok(new { data, meta = (object)null, error = (object)null });
        }

        [HttpGet("{id}")]
        public virtual IActionResult GetById(Guid id)
        {
            var data = _baseService.GetById(id);
            if (data == null) return NotFound(new { data = (object)null, meta = (object)null, error = "Not found" });

            return Ok(new { data, meta = (object)null, error = (object)null });
        }

        [HttpPost]
        public virtual IActionResult Insert([FromBody] T entity)
        {
            try
            {
                var data = _baseService.Insert(entity);
                return Ok(new { data, meta = (object)null, error = (object)null });
            }
            catch (Exception ex)
            {
                // Trả về JSON cho frontend
                return BadRequest(new { userMsg = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public virtual IActionResult Update(Guid id, [FromBody] T entity)
        {
            var data = _baseService.Update(id, entity);
            return Ok(new { data, meta = (object)null, error = (object)null });
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(Guid id)
        {
            var data = _baseService.Delete(id);
            return Ok(new { data, meta = (object)null, error = (object)null });
        }

        // Bulk delete
        [HttpDelete("bulk")]
        public virtual IActionResult Delete([FromBody] Guid[] ids)
        {
            if (ids == null || ids.Length == 0)
                return BadRequest(new { data = (object)null, meta = (object)null, error = "No IDs provided" });

            var data = _baseService.Delete(ids);
            return Ok(new { data, meta = (object)null, error = (object)null });
        }
    }

}
