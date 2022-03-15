using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System;
using Microsoft.AspNetCore;
using System.Reflection;
using SSOAuth.Helper;

namespace SSOAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = AppStartup(args);
            host.Run();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            // Check the current directory that the application is running on 
            // Then once the file 'appsetting.json' is found, we are adding it.
            // We add env variables, which can override the configs in appsettings.json
                builder
                .AddEnvironmentVariables();
        }

        static IWebHost AppStartup(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            // Specifying the configuration for serilog
            Log.Logger = new LoggerConfiguration() // initiate the logger configuration
                            .ReadFrom.Configuration(builder.Build()) // connect serilog to our configuration folder
                            .Enrich.FromLogContext() //Adds more information to our logs from built in Serilog 
                            .WriteTo.File($"logs\\log_{DateTime.Now.ToString("dd-MM-yy")}.txt" //,rollingInterval: RollingInterval.Day
                            , restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)  // decide where the logs are going to be shown
                            .CreateLogger(); //initialise the logger

            Log.Logger.Information("Application Starting");



            return WebHost.CreateDefaultBuilder(args) // Initialising the Host 
                        .ConfigureServices((context, services) =>
                        { // Adding the DI container for configuration
                        })
                        .UseStartup<Startup>()
                        .UseSerilog() // Add Serilog
                        .Build(); // Build the Host
        }
    }
}
