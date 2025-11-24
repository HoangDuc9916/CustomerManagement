using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Infrastructure.Repository
{
    public class BaseRepository<T> : IBaseRepository<T>, IDisposable where T : class
    {
        protected readonly MySqlConnection dbConnection;
        protected readonly string tableName;
        protected readonly string pkName;

        public BaseRepository(IConfiguration config, string tableName, string pkName)
        {
            var connectionString = config.GetConnectionString("DBConnection");
            dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();
            this.tableName = tableName;
            this.pkName = pkName;

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }


        /// <summary>
        /// Lấy tất cả bản ghi từ table
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// CreatedBy: Duchc (15/11/2025)
        public List<T> GetAll()
        {
            var sql = $"SELECT * FROM {tableName} WHERE isDelete = 1 ORDER BY updated_at DESC, created_at DESC";
            return dbConnection.Query<T>(sql).ToList();
        }
        /// <summary>
        /// Lấy bản ghi theo Id
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <returns>Bản ghi hoặc null</returns>
        /// CreatedBy: Duchc (15/11/2025)
        public T GetById(Guid entityId)
        {
            var sql = $"SELECT * FROM {tableName} WHERE {pkName} = @Id";
            return dbConnection.QueryFirstOrDefault<T>(sql, new { Id = entityId });
        }

        /// <summary>
        /// Xóa bản ghi theo Id
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <returns>Số bản ghi xóa</returns>
        /// CreatedBy: Duchc (15/11/2025)
        public int Delete(Guid entityId)
        {
            // Không xóa hẳn mà set isDelete = 0
            var sql = $"UPDATE {tableName} SET isDelete = '0', updated_at = @UpdatedAt WHERE {pkName} = @Id";
            return dbConnection.Execute(sql, new { Id = entityId, UpdatedAt = DateTime.UtcNow });
        }


        /// <summary>
        /// Thêm mới (phải override trong repository con)
        /// </summary>
        /// <param name="entity">Thực thể cần thêm</param>
        /// <returns>Số bản ghi thêm</returns>
        /// CreatedBy: Duchc (15/11/2025)
        public virtual int Insert(T entity)
        {
            throw new NotImplementedException("Insert must be implemented in derived repository");
        }

        /// <summary>
        /// Cập nhật (phải override trong repository con)
        /// </summary>
        /// <param name="entity">Thực thể cần cập nhật</param>
        /// <param name="entityId">Id thực thể</param>
        /// <returns>Số bản ghi cập nhật</returns>
        /// CreatedBy: Duchc (15/11/2025)
        public virtual int Update(T entity, Guid entityId)
        {
            throw new NotImplementedException("Update must be implemented in derived repository");
        }

        public int Delete(IEnumerable<Guid> entityIds)
        {
            if (entityIds == null || !entityIds.Any()) return 0;

            var sql = $"UPDATE {tableName} SET isDelete = '0', updated_at = @UpdatedAt WHERE {pkName} IN @Ids";
            return dbConnection.Execute(sql, new { Ids = entityIds, UpdatedAt = DateTime.UtcNow });
        }

        public void Dispose()
        {
            dbConnection?.Dispose();
        }

   
    }

}
