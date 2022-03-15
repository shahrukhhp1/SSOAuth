using Microsoft.EntityFrameworkCore;
using SSOAuth.Business.Interface;
using SSOAuth.Data.Entity.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Business
{
    public class ProviderService : IProviderService
    {
        private AuthDBContext _context;
        private IAuthService _authService;
        public ProviderService(AuthDBContext context, IAuthService authService)
        {
            this._context = context;
            this._authService = authService;
        }

        public async Task<Provider> GetProviderByClientId(string clientId)
        {
            return await _context.Providers.Where(x => x.ClientId.Equals(clientId)).FirstOrDefaultAsync();
        }       
        
    }
}
