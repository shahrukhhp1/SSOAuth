using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Settings
{
    public class AuthSettings
    {
        public string Issuer { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
