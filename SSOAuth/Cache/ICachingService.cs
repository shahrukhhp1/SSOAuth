using SSOAuth.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Cache
{
    public interface ICachingService
    {
        Task<bool> SetCodeAndProviderWithToken(CodeTokenProviderCacheDTO data);
        Task<CodeTokenProviderCacheDTO> GetProviderTokenByCode(string authorizationCode);

        Task<bool> SetIdentityAndToken(IdentityCacheDTO data);
        Task<IdentityCacheDTO> GetTokenByIdentity(Guid authorizationCode);
    }
}
