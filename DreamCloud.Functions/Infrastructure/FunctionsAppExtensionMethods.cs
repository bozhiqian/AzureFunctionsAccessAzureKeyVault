using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Azure.Core;

namespace DreamCloud.Functions.Infrastructure;

public static class FunctionsAppExtensionMethods
{
    public static IConfigurationBuilder AddAppSettingsJson(this IConfigurationBuilder builder, FunctionsHostBuilderContext context)
    {
        builder.AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false);
        builder.AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false);
        return builder;
    }

    public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder builder, IConfiguration configuration)
    {
        var keyVaultUri = configuration.CreateKeyVaultUri();
        var keyVaultCredential = configuration.CreateKeyVaultCredential();
        builder.AddAzureKeyVault(keyVaultUri, keyVaultCredential);

        return builder;
    }

    private static TokenCredential CreateKeyVaultCredential(this IConfiguration configuration)
    {
        // WARNING: Make sure to give the App in the Azure Portal access to the KeyVault.
        //          In the Identity tab: System Assigned part: turn Status On and copy the Object ID.
        //          In the KeyVault: Access Policies > Add Access Policy > Secret Permissions Get, List and Select Principal: Object ID copied above.
        // When running on Azure, you do NOT need to set the KeyVaultTenantId.
        var keyVaultTenantId = configuration["KeyVaultTenantId"];
        if (string.IsNullOrEmpty(keyVaultTenantId))
            return new DefaultAzureCredential();

        // When debugging local from VisualStudio AND the TenantId differs from default AZURE_TENANT_ID (in Windows settings/environment variables),
        // you can store KeyVaultTenantId= in appsettings or in UserSecrets and read it here from the configuration (as done above)
        var options = new DefaultAzureCredentialOptions { VisualStudioTenantId = keyVaultTenantId };
        return new DefaultAzureCredential(options);
    }

    private static Uri CreateKeyVaultUri(this IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        var keyVaultName = configuration["KeyVaultName"];
        if (string.IsNullOrEmpty(keyVaultName))
            throw new InvalidOperationException($"Missing configuration setting {keyVaultName}");

        return new Uri($"https://{keyVaultName}.vault.azure.net/");
    }
}