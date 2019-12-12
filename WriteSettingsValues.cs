using System;
using Microsoft.Extensions.Options;

namespace UserSecretConsoleAppDemo
{
    public class WriteSettingsValues
    {
        private readonly AppSettings appSettings;
        private readonly SecureSettings secureSettings;

        public WriteSettingsValues(IOptions<AppSettings> appSettings, IOptions<SecureSettings> secureSettings)
        {
            this.appSettings = appSettings.Value;
            this.secureSettings = secureSettings.Value;
        }

        public void Write()
        {
            Console.WriteLine($"Value of foo is '{appSettings.Foo}'");
            Console.WriteLine($"Value of default connection is '{secureSettings.DefaultConnection}'");
            Console.WriteLine($"Value of some api token is '{secureSettings.SomeApiToken}'");

            Console.ReadLine();
        }
    }
}