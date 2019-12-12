using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UserSecretConsoleAppDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environmentName}.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<SecureSettings>()
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();

            var appSettings = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);
            
            var secureSettings = configuration.GetSection("SecureSettings");
            services.Configure<SecureSettings>(secureSettings);

            services.AddTransient<WriteSettingsValues>();

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<WriteSettingsValues>().Write();
        }
    }
}
