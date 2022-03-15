using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Models.Auth
{
    public class AccessTokenResponseDTO
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; } = "Bearer"; // default
        public long? Expires_in { get; set; } // in seconds
        public string Refresh_token { get; set; } //optional
        public string Scope { get; set; } //optional

        public string Error { get; set; } //optional
        public string ErrorDescription { get; set; } //optional
    }
}
