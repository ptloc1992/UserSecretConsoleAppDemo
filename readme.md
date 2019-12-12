# User Secrets & Environment Variables - Console Application

With .NET Core, you can take advantage of User Secrets and Environment Variables to manage sensitive data such as connection strings, API tokens, etc.

In your development environment, you can use a feature called User Secrets, which is a JSON file stored locally on your machine. This allows you to store sensitive settings without storing them in source control. 

In staging or production environments, you would use Environment Variables to store these values at the server level. The key for each setting should be identical to what is in your user secrets json file and the value should be specific to whatever environment you are running in.

.NET Core offers support for user secrets out of the box for an ASP.NET Core application. In order to use these features in a console application, it is a slightly different process to set up and configure.

#### Install NuGet Packages

Add the following NuGet packages to your console application:

- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Configuration.Abstractions
- Microsoft.Extensions.Configuration.EnvironmentVariables
- Microsoft.Extensions.Configuration.FileExtensions
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.Configuration.UserSecrets
- Microsoft.Extensions.Options.ConfigurationExtensions
- Microsoft.Extensions.DependencyInjection

#### Set Your Local Environment Variable

In Visual Studio, right-click on your console application project and go to properties. Click on the 'Debug' option in the left navigation. Add a new environment variable called ASPNETCORE_ENVIRONMENT and set the value to Development.

#### Create Your App Settings File

Add a new file to your project called appsettings.Development.json file. To support other environments, you can create additional files such as appsettings.Staging.json or appsettings.Production.json.

So the appsettings file will be copied to the output folder on build, go to the file properties of the appsettings file and set 'Copy to Output Directory' to 'Always'.

#### Creating the User Secrets File

When the user secrets file is generated, it is stored in the following location on your machine:

%APPDATA%\Microsoft\UserSecrets\\<user_secrets_id>\secrets.json

<user_secrets_id> represents a GUID that is generated when creating the secrets.

To create your user secrets file for a project, open a PowerShell window and navigate to the location of the project file. Enter the following command:

```
dotnet user-secrets init
```

As an alternative, you can also right click on the project file and select 'Manage User Secrets', which will create the file for you as well.

As part of creating the user secrets file, the user secrets id will be added to the project file of your console application:

```
<PropertyGroup>
	<OutputType>Exe</OutputType>
	<TargetFramework>netcoreapp3.1</TargetFramework>
	<UserSecretsId>975d455a-c57d-40b2-8892-3ccc752223f2</UserSecretsId>
</PropertyGroup>
```

If you are sharing user secrets across projects, you can simply edit the project file and paste in the UserSecretsId line.

Here is an example of what your secrets.json file could look like:

```
{
  "SecureSettings": {
    "DefaultConnection": "your connection string",
    "SomeApiToken": "771308f0-b397-4a3e-894b-8e30067e29f2"
  } 
}
```

#### Strongly-Typed Settings

You can strongly type your settings by creating a class that has properties that mirror the secrets.json structure:

```
public class SecureSettings
{
	public string DefaultConnection { get; set; }
	public string SomeApiToken { get; set; }
}
```

#### Configure Console Application to Use Secrets and Environment Variables

In your program.cs file, add the following code:

```
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var builder = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile($"appsettings.{environmentName}.json", optional: false, reloadOnChange: true)
	.AddUserSecrets<SecureSettings>()
	.AddEnvironmentVariables();

var secureSettings = configuration.GetSection("SecureSettings");
services.Configure<SecureSettings>(secureSettings);

services.AddDbContext<CEPEGContext>(options => options.UseSqlServer(configuration["SecureSettings:DefaultConnection"]));

var serviceProvider = services.BuildServiceProvider();
```

#### Using Values from Secrets and Environment Variables

Dependency injection is used to inject in your strongly-typed settings:

```
public class Foo
{
	private readonly SecureSettings secureSettings;

	public Foo(IOptions<SecureSettings> secureSettings)
	{
		this.secureSettings = secureSettings.Value;
	}
	
	public void LogValue()
	{
		Console.WriteLine($"Current connection string is {secureSettings.DefaultConnection}");
	}
}
```
