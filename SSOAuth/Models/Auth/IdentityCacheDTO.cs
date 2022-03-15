using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Models.Auth
{
    public class IdentityCacheDTO
    {
        public Guid IdentityId { get; set; }
        public string Token { get; set; }
    }
}
