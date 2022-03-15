using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Helper
{
    public static class Constants
    {
        public static string _configPath 
        {
            get
            {
                return $"config/appsettings_{Environment.MachineName}.json";
            }
            set { }
        }
    }
}
