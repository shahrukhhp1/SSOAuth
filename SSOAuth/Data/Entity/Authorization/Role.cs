using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Data.Entity.Authorization
{
    public class Role : Base
    {
        public Role()
        {
            this.UserRoles = new HashSet<UserRole>();
            this.RoleClaims = new HashSet<RoleClaim>();
        }

        public string Name { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RoleClaim> RoleClaims { get; set; }
    }
}
