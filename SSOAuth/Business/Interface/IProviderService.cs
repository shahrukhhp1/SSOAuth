using SSOAuth.Data.Entity.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Business.Interface
{
    public interface IProviderService
    {
        Task<Provider> GetProviderByClientId(string clientId);
     
    }
}
