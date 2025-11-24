using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Interfaces.Services
{
    public interface IBaseService<T>
    {
        List<T> GetAll();
        T GetById(Guid id);
        T Insert(T entity);
        T Update(Guid id, T entity);
        int Delete(Guid id); // gọi Update để set isDelete = 0

        int Delete(IEnumerable<Guid> ids);
    }
}
