using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Models.Auth
{
    public class MiddlewareAuthDTO
    {
        public bool IsAuthenticated { get; set; } = false;
        public string RedirectURL { get; set; }
        public bool IsPermenantRedirect { get; set; }
    }
}
