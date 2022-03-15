namespace SSOAuth.Settings
{
    public class AppSettings
    {
        public AppSettings()
        {
            this.ApplicationSettings = new ApplicationSetting();
            this.DataSettings = new DataSetting();
            this.ConnectionStrings = new ConnectionString();
            this.AuthSettings = new AuthSetting();
        }

        public ConnectionString ConnectionStrings { get; set; }
        public DataSetting DataSettings { get; set; }
        public ApplicationSetting ApplicationSettings { get; set; }
        public AuthSetting AuthSettings { get; set; }

        public class ConnectionString
        {
            public string AuthDB { get; set; }
        }

        public class DataSetting
        {
            public bool Migrate { get; set; }
            public bool Seed { get; set; }
        }

        public class ApplicationSetting
        {
            public bool DevelopmentMode { get; set; }
            public string LoginURL { get; set; }
        }

        public class AuthSetting
        {
            public string Issuer { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}
