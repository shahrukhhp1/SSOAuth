using SSOAuth.Data.Entity.Authorization;
using SSOAuth.Models.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Business.Interface
{
    public interface IUserService
    {
        Task<List<UserAccessVM>> GetAllUsers();
        Task<bool> UpdatePassword(long userId, string pwd);

        Task<UserAccessVM> VerifyPassword(string userName, string pwd);
        Task<UserAccessVM> CreateUser(string userName, string pwd, string email, string firstName, string lastName, long createdBy);
        
        Task<UserAccessVM> GetUser(long id);

        Task<bool> EnterLoggedInInfo(long userId, long providerId,
            string token,
            string ipAddress,
            DateTime expiry);


    }
}
