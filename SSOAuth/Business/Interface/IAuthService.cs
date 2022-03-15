using SSOAuth.Models;
using SSOAuth.Models.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSOAuth.Data.Entity.Authorization;

namespace SSOAuth.Business.Interface
{
    public interface IAuthService
    {
        Task<string> GenerateToken(long userId, DateTime expiry, string secretKey, string audience);

        PasswordHashModel passwordHashGenerate(string password);

        bool verifySaltAndPassword(string salt, string pwd, string pwdHash);

        Task<Provider> GetAuthClientId(string clientId);
        Task<bool> VerifyAuthScope(string clientId);
        Task<string> AuthorizationCodeGeneration(Provider providerData, string token, Guid identity);
        string GetCallbackPath(string domain, string code, string redirect);
    }

}
