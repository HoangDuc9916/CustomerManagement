using Dapper;
using System.Data;

namespace MISA.CustomerManagement.Api.Helpers
{
    public class GuidTypeHelper : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            // Khi đọc từ database lên, chuyển string -> Guid
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return Guid.Empty;
            }
            if (Guid.TryParse(value.ToString(), out Guid result))
            {
                return result;
            }
            return Guid.Parse(value.ToString()!);
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToString();
        }
    }
}
