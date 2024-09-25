// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Options;
using Volo.Abp.Security.Encryption;

using X.Abp.Account.ExternalProviders;

namespace X.Abp.Account.Public.Web.ExternalProviders;

public class AccountExternalProviderOptionsManager<TOptions> : AbpDynamicOptionsManager<TOptions>, IOptionsMonitor<TOptions>
    where TOptions : class, new()
{
    protected IAccountExternalProviderAppService AccountExternalProviderAppService { get; }

    protected IStringEncryptionService StringEncryptionService { get; }

    protected ITenantConfigurationProvider TenantConfigurationProvider { get; }

    protected IEnumerable<IPostConfigureAccountExternalProviderOptions<TOptions>> PostConfigures { get; }

    protected ILogger<AccountExternalProviderOptionsManager<TOptions>> Logger { get; set; }

    public AccountExternalProviderOptionsManager(IOptionsFactory<TOptions> factory,
        IAccountExternalProviderAppService accountExternalProviderAppService,
        IStringEncryptionService stringEncryptionService,
        ITenantConfigurationProvider tenantConfigurationProvider,
        IEnumerable<IPostConfigureAccountExternalProviderOptions<TOptions>> postConfigures)
        : base(factory)
    {
        AccountExternalProviderAppService = accountExternalProviderAppService;
        StringEncryptionService = stringEncryptionService;
        TenantConfigurationProvider = tenantConfigurationProvider;
        PostConfigures = postConfigures;

        Logger = NullLogger<AccountExternalProviderOptionsManager<TOptions>>.Instance;
    }

    protected override async Task OverrideOptionsAsync(string name, TOptions options)
    {
        TenantConfiguration tenantConfiguration;
        try
        {
            tenantConfiguration = await TenantConfigurationProvider.GetAsync();
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return;
        }

        var externalProviderItemDto = await AccountExternalProviderAppService.GetByNameAsync(new GetByNameInput
        {
            TenantId = tenantConfiguration?.Id,
            Name = name
        });

        if (externalProviderItemDto.Success && externalProviderItemDto.Enabled)
        {
            externalProviderItemDto.SecretProperties = externalProviderItemDto.SecretProperties
                .Select(secretValue => new ExternalProviderSettingsProperty(secretValue.Name, StringEncryptionService.Decrypt(secretValue.Value)))
                .ToList();

            var properties = externalProviderItemDto.Properties.Concat(externalProviderItemDto.SecretProperties)
                .ToList();

            foreach (var property in properties)
            {
                var optionsProp = typeof(TOptions).GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
                optionsProp?.SetValue(options, property.Value);
            }
        }

        foreach (var postConfigure in PostConfigures)
        {
            await postConfigure.PostConfigureAsync(name, options);
        }
    }

    public IDisposable OnChange(Action<TOptions, string> listener)
    {
        return NullDisposable.Instance;
    }

    public TOptions CurrentValue
    {
        get
        {
            return Get(Options.DefaultName);
        }
    }
}
