using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Data.Entity.Authorization
{
    public class UserLogin : Base
    {
        public UserLogin()
        {
            //this.User = new User();
            //this.Provider = new Provider();
        }
        //public long
        public virtual User User { get; set; }
        public long? UserId { get; set; }

        public virtual Provider Provider { get; set; }
        public long? ProviderId { get; set; }
    }
}
