using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Infrastructure.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IConfiguration config)
            : base(config, "user", "user_id")
        {
        }
        public User GetUserByUsername(string username)
        {
            string tableName = typeof(User).Name;

            string sql = $"SELECT * FROM {tableName} WHERE username = @Username LIMIT 1";

            return dbConnection.QueryFirstOrDefault<User>(sql, new { Username = username });
        }
    }
}
