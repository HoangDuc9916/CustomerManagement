using MISA.Core.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Dtos
{
    public class UserDto
    {
        [MISARequired("Tên đăng nhập không được để trống")]
        public string UserName { get; set; }

        [MISARequired("Mật khẩu không được để trống")]
        public string Password { get; set; }
    }
}
