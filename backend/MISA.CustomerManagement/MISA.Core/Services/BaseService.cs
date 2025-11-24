using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Services;
using MISA.Core.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IBaseRepository<T> _baseRepository;

        public BaseService(IBaseRepository<T> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public virtual List<T> GetAll() => _baseRepository.GetAll();

        public virtual T GetById(Guid id) => _baseRepository.GetById(id);

        public virtual T Insert(T entity)
        {
            ValidateEntity(entity);

            
            if (entity is Customer customer)
                customer.IsDelete = "1";

            _baseRepository.Insert(entity);
            return entity;
        }

        public virtual T Update(Guid id, T entity)
        {
            ValidateEntity(entity);
            _baseRepository.Update(entity, id);
            return entity;
        }

        public virtual int Delete(Guid id)
        {
            return _baseRepository.Delete(id); // Thực chất là update isDelete = 0
        }

        public int SoftDelete(Guid id)
        {
            throw new NotImplementedException();
        }

        protected virtual void ValidateEntity(T entity)
        {
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var requiredAttr = (MISARequired)Attribute.GetCustomAttribute(prop, typeof(MISARequired));
                if (requiredAttr != null)
                {
                    if (prop.Name == "CustomerCode" || prop.Name == "CreatedAt" || prop.Name == "UpdatedAt")
                        continue;

                    var value = prop.GetValue(entity);
                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        throw new Exception($"{prop.Name} is required");
                }
            }
        }

        public int Delete(IEnumerable<Guid> ids)
        {
            return _baseRepository.Delete(ids);
        }
    }
}
