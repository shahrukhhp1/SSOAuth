using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Models.Auth
{
    public class AuthorizationCodeCacheDTO
    {
        public string AuthorizationCode { get; set; }
        public string Token { get; set; }
    }
}
