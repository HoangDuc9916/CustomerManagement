using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Core.Dtos;
using MISA.Core.Interfaces.Services;

namespace MISA.CustomerManagement.Api.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] UserDto dto)
        {
            try
            {
                var user = _userService.Login(dto.UserName, dto.Password);

                // Nếu login thành công, trả về thông tin user hoặc chỉ success
                return Ok(new { data = new { user.UserId, user.Username }, error = (object)null });
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = (object)null, error = ex.Message });
            }
        }
    }
}
