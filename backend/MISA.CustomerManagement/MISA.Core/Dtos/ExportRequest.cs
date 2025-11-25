using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Dtos
{
    public class ExportRequest
    {
        public List<string> Ids { get; set; } = new List<string>();   // danh sách khách hàng được chọn

        public List<string> Columns { get; set; } = new List<string>(); // danh sách cột FE gửi lên
    }
}
