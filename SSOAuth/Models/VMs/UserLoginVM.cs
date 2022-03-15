using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Models.VMs
{
    public class UserLoginVM
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RedirectURL { get; set; }
        public string ClientId { get; set; }
    }
}
