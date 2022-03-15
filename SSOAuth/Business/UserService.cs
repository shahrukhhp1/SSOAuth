using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SSOAuth.Business.Interface;
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
    public class UserService : IUserService
    {
        private AuthDBContext _context;
        private IAuthService _authService;
        public UserService(AuthDBContext context, IAuthService authService)
        {
            this._context = context;
            this._authService = authService;
        }

        public async Task<List<UserAccessVM>> GetAllUsers()
        {
            return await _context.Users
                .Select(user => new UserAccessVM() {
                    IsActive = user.IsActive,
                    Identity = user.Identity,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsLocked = user.IsLocked,
                    UserName = user.UserName,
                    Id = user.Id,
                })
                .ToListAsync();

        }

        public async Task<UserAccessVM> GetUser(long id)
        {
            var user = await _context.Users.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            if (user == null) 
                return null;

            return new UserAccessVM()
            {
                IsActive = user.IsActive,
                Identity = user.Identity,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsLocked = user.IsLocked,
                UserName = user.UserName,
            };
        }

        public async Task<UserAccessVM> VerifyPassword(string userName, string pwd)
        {
            var user = await _context.Users.Where(x => x.UserName.Equals(userName)).FirstOrDefaultAsync();


            if (user == null)
                throw new Exception("Invalid UserName");

            if (!user.IsActive)
                throw new Exception("User is inactive");

            if (user.IsLocked)
                throw new Exception("User is locked");

            if (_authService.verifySaltAndPassword(user.PasswordSalt, pwd, user.PasswordHash))
            {
                return new UserAccessVM()
                {
                    IsActive = user.IsActive,
                    Identity = user.Identity,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsLocked = user.IsLocked,
                    UserName = user.UserName,
                    Id = user.Id
                };
            }
            else
            {
                throw new Exception("Invalid Password");
            }
        }


        public async Task<bool> EnterLoggedInInfo(long userId, long providerId,
            string token,
            string ipAddress,
            DateTime expiry)
        {
            var userLogin = await _context.UserLogins.Where(x => x.UserId.Equals(userId)
            && x.ProviderId.Equals(providerId) && x.IsActive).FirstOrDefaultAsync();
            if (userLogin == null)
            {
                var uLogin = new UserLogin()
                {
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now,
                    ProviderId = providerId,
                    UserId = userId
                };
                await _context.UserLogins.AddAsync(uLogin);
                await _context.SaveChangesAsync();
            }

            var userToken = await _context.UserTokens.Where(x => x.UserId.Equals(userId) && x.IsActive
            && x.Token.Equals(token)).FirstOrDefaultAsync();
            if (userToken == null)
            {
                var uToken = new UserToken()
                {
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now,
                    Token = token,
                    IP = ipAddress,
                    Expiry = expiry,
                    UserId = userId,
                };
                await _context.UserTokens.AddAsync(uToken);
                await _context.SaveChangesAsync();
            }
            return true;
        }


        public async Task<UserAccessVM> CreateUser(string userName,string pwd,string email,string firstName,string lastName,long createdBy)
        {
            var passwordAndHash = _authService.passwordHashGenerate(pwd);

            User newUser = new User()
            {
                CreatedBy = createdBy,
                CreatedOn = DateTime.Now,
                IsActive = true,
                Identity = Guid.NewGuid(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                IsLocked = false,
                UserName = userName,
                PasswordHash = passwordAndHash.PasswordHash,
                PasswordSalt = passwordAndHash.PasswordSalt
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return new UserAccessVM()
            {
                Id = newUser.Id,
                IsActive = newUser.IsActive,
                Identity = newUser.Identity,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                IsLocked = newUser.IsLocked,
                UserName = newUser.UserName,
            };
        }


        public async Task<bool> UpdatePassword(long userId, string pwd)
        {
            var user = await _context.Users.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
            if (user != null)
            {
                var passwordAndHash = _authService.passwordHashGenerate(pwd);
                user.PasswordHash = passwordAndHash.PasswordHash;
                user.PasswordSalt = passwordAndHash.PasswordSalt;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


    }
}
