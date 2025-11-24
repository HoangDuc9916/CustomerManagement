using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Interfaces.Repository
{
    public interface IBaseRepository<T>
    {
        List<T> GetAll();
        T GetById(Guid entityId);
        int Insert(T entity);
        int Update(T entity, Guid entityId);
        int Delete(Guid entityId); // Có thể xóa hẳn hoặc gọi SoftDelete qua Update

        int Delete(IEnumerable<Guid> entityIds);
    }
}
