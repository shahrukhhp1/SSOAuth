using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Models.Auth
{
    public class CodeTokenProviderCacheDTO
    {
        public string Client_id { get; set; }
        public string Client_secret { get; set; }
        public string Code { get; set; }
        public string Grant_type { get; set; }
        public string Redirect_uri { get; set; }
        public string Domain { get; set; }
        public string Token { get; set; }
    }
}
