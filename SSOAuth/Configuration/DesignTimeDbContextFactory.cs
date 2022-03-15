
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SSOAuth.Data.Entity.Authorization;
using SSOAuth.Extensions;
using SSOAuth.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Configuration
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AuthDBContext>
    {
        
        public AuthDBContext CreateDbContext(string[] args)
        {
            var file = System.IO.File.ReadAllText(Constants._configPath);
            SSOAuth.Settings.AppSettings appSetting = JsonConvert.DeserializeObject<SSOAuth.Settings.AppSettings>(file);

            var builder = new DbContextOptionsBuilder<AuthDBContext>();
            builder.UseSqlServer(appSetting.ConnectionStrings.AuthDB);
            return new AuthDBContext(builder.Options);
        }
    }
}
