using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SSOAuth.Data.Entity.Authorization;
using SSOAuth.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Extensions
{
    public static class SeedExtension
    {
        public static void Seed(this AuthDBContext context, AppSettings appsettings)
        {
            
            if (context.Providers.Count() == 0)
            {
                // following is the basic first provider of system that is itself
                Provider newProv = new Provider()
                {
                    IsActive = true,
                    ClientId = appsettings.AuthSettings.ClientId,
                    ClientSecret = appsettings.AuthSettings.ClientSecret,
                    CreatedBy = 0,
                    CreatedOn = DateTime.Now,
                    Domain = appsettings.ApplicationSettings.LoginURL,
                    Name = "ssoAuth"
                };
                context.Providers.Add(newProv);
                context.SaveChanges();
            }
            if (context.Users.Count() == 0)
            {

                // following is the basic first role of admin and user of system with login and password as 'admin'


                var newRole = new Role()
                {
                    IsActive = true,
                    CreatedBy = 0,
                    CreatedOn = DateTime.Now,
                    Name = "Admin",
                    RoleClaims = new List<RoleClaim>()
                                 {
                                      new RoleClaim()
                                      {
                                          IsActive = true,
                                          ClaimValue = "admin",
                                          CreatedBy = 0,
                                          CreatedOn = DateTime.Now
                                      }
                                 }
                };
                context.Roles.Add(newRole);
                context.SaveChanges();
                var newRoleId = newRole.Id;

                User newUser = new User()
                {
                    IsActive = true,
                    CreatedBy = 0,
                    CreatedOn = DateTime.Now,
                    Email = "admin@admin.com",
                    FirstName = "admin",
                    Identity = Guid.Parse("37F81DD7-8563-4FBE-9343-A9CD6BEF5BAC"),
                    IsLocked = false,
                    LastName = "",
                    UserName = "admin",
                    PasswordHash = "GNdoUMJOqxoE7E7mMBEJC1BkrmLhwsNLOudhiecy39M=",
                    PasswordSalt = "QLH34lhmLcxbMcKwPpIv6sNTalxaBkkKWKDcxWLZ9zs=",
                    UserRoles = new List<UserRole>()
                    { 
                         new UserRole()
                         {
                             IsActive = true, 
                             CreatedBy = 0,
                             CreatedOn = DateTime.Now,
                             //RoleId = newRoleId,
                             Role = newRole
                         }
                    }
                };
                context.Users.Add(newUser);
                context.SaveChanges();

            }
        }
    }
}
