using SSOAuth.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Cache
{
    public class CachingService : ICachingService
    {
        public CachingService()
        { 

        }

        public async Task<bool> SetCodeAndProviderWithToken(CodeTokenProviderCacheDTO data)
        {
            LCache.Instance.Write(data.Code, data);
            return true;
        }


        public async Task<CodeTokenProviderCacheDTO> GetProviderTokenByCode(string authorizationCode)
        {
            var ret = (CodeTokenProviderCacheDTO)LCache.Instance.TryRead(authorizationCode);
            return ret;
        }


        public async Task<bool> SetIdentityAndToken(IdentityCacheDTO data)
        {
            LCache.Instance.Write(data.IdentityId.ToString(), data);
            return true;
        }


        public async Task<IdentityCacheDTO> GetTokenByIdentity(Guid identity)
        {
            var ret = (IdentityCacheDTO)LCache.Instance.TryRead(identity.ToString());
            return ret;
        }
    }
}
