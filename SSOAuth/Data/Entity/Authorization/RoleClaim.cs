using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Data.Entity.Authorization
{
    public class RoleClaim : Base
    {
        public virtual Role Role { get; set; }
        public long? RoleId { get; set; }

        public string ClaimValue { get; set; }
    }
}
