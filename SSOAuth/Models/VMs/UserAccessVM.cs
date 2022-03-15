using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Models.VMs
{
    public class UserAccessVM
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public Guid Identity { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public string Password { get; set; }
    }
}
