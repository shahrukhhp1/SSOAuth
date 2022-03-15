using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Models
{
    public class PasswordHashModel
    {
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
