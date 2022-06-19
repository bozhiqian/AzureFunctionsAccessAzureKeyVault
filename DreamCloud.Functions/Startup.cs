using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DreamCloud.Functions.Infrastructure;

[assembly: FunctionsStartup(typeof(DreamCloud.Functions.Startup))]
namespace DreamCloud.Functions;

public class Startup : FunctionsStartup
{
    private IConfiguration _configuration;
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<IConfiguration>(_ => this._configuration);
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        // local.settings.json are automatically loaded when debugging.
        // When running on Azure, values are loaded defined in app settings.
        // See: https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings
        _configuration = builder.ConfigurationBuilder
            .AddAppSettingsJson(builder.GetContext())
            .Build();

        var configurationBuilder = builder.ConfigurationBuilder.AddAzureKeyVault(_configuration);
        _configuration = configurationBuilder.Build();

        base.ConfigureAppConfiguration(builder);
    }
}