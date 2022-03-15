using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Data.Entity.Authorization
{
    public class UserRole : Base
    {
        public UserRole()
        {
        }
        public virtual Role Role { get; set; }
        public long? RoleId { get; set; }

        public virtual User User {get;set;}
        public long? UserId {get;set;}
    }
}
