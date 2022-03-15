using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SSOAuth.Business.Interface;
using SSOAuth.Cache;
using SSOAuth.Data.Entity.Authorization;
using SSOAuth.Models;
using SSOAuth.Models.VMs;
using SSOAuth.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SSOAuth.Business
{
    public class AuthService : IAuthService
    {

        private AuthDBContext _context;
        private AuthSettings _authSettings;
        private ICachingService _cacheServ;
        public AuthService(AuthDBContext context, IOptions<AuthSettings> authSettings, ICachingService cacheServ)
        {
            this._context = context;
            this._authSettings = authSettings.Value;
            this._cacheServ = cacheServ;
        }      

        public async Task<string> GenerateToken(long userId, DateTime expiry, string secretKey, string audience)
        {

            var uRoleClaims = await _context.UserRoles
                .Include(x => x.Role)
                .ThenInclude(x => x.RoleClaims)
                .Where(x => x.UserId.Equals(userId))
                .ToListAsync();

            
            var user = await _context.Users.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var claimsdata = new List<Claim>()
            {
                 new  Claim("IdentityId" , user.Identity.ToString()),
            };
            foreach (var rl in uRoleClaims)
            {
                foreach (var cl in rl.Role.RoleClaims)
                {
                    claimsdata.Add(new Claim("claim", cl.ClaimValue));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _authSettings.Issuer,
                audience: audience,
                claims: claimsdata,
                expires: expiry,
                signingCredentials: signInCred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        public async Task<string> AuthorizationCodeGeneration(Provider providerData, string token,Guid identity)
        {
            var authCode = Guid.NewGuid().ToString();
            await _cacheServ.SetIdentityAndToken(new Models.Auth.IdentityCacheDTO() { 
                IdentityId = identity,
                Token = token
            });
            await _cacheServ.SetCodeAndProviderWithToken(new Models.Auth.CodeTokenProviderCacheDTO()
            {
                Client_id = providerData.ClientId,
                Client_secret = providerData.ClientSecret,
                Code = authCode,
                Grant_type = "authorization_code",
                Domain = providerData.Domain,
                Token = token,
                Redirect_uri = providerData.Domain,
            });
            return authCode;
        }

        public bool verifySaltAndPassword(string salt, string pwd, string pwdHash)
        {
            byte[] saltBytes = System.Convert.FromBase64String(salt);

            var testHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: pwd,
               salt: saltBytes,
               prf: KeyDerivationPrf.HMACSHA256,
               iterationCount: 100,
               numBytesRequested: 256 / 8));

            if (testHash == pwdHash)
                return true;

            return false;
        }

        public PasswordHashModel passwordHashGenerate(string password)
        {
            byte[] salt = getSalt();

            //Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)


            var pwdHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100,
                numBytesRequested: 256 / 8));

            return new PasswordHashModel() { PasswordHash = pwdHash, PasswordSalt = Convert.ToBase64String(salt) };
        }

        private byte[] getSalt()
        {
            var random = new RNGCryptoServiceProvider();

            // Maximum length of salt
            int max_length = 32;

            // Empty salt array
            byte[] salt = new byte[max_length];

            // Build the random bytes
            random.GetNonZeroBytes(salt);

            // Return the string encoded salt
            return salt;
        }

        public async Task<Provider> GetAuthClientId(string clientId)
        {            
            return await _context.Providers.Where(x => x.ClientId.Equals(clientId)).FirstOrDefaultAsync();            
        }

        public async Task<bool> VerifyAuthScope(string scope)
        {
            return true;
        }


        public string GetCallbackPath(string domain, string code, string redirect)
        {
            return "http://" + domain + "/signin-oidc?code=" + code + "&redirect=" + redirect;
        }
        
    }
}
