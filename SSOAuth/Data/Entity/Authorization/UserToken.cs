using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Data.Entity.Authorization
{
    public class UserToken : Base
    {
        public UserToken()
        {
            //this.User = new User();
        }
        public virtual User User{ get; set; }
        public long? UserId { get; set; }

        public string Token { get; set; }
        public string IP { get; set; }
        public DateTime Expiry { get; set; }
    }
}
