using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Entities
{
    public class User
    {
        public Guid UserId { get; set; }          // user_id
        public string Username { get; set; }      // username
        public string PasswordHash { get; set; }  // password_hash
        public DateTime CreatedAt { get; set; }   // created_at
    }
}
