// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

using X.Abp.Account.Settings;

namespace X.Abp.Account.ExternalProviders;

public class ExternalProviderSettingsHelper : ITransientDependency
{
    protected ICurrentTenant CurrentTenant { get; }

    protected AbpExternalProviderOptions ExternalProviderOptions { get; }

    protected ISettingManager SettingManager { get; }

    protected IJsonSerializer JsonSerializer { get; }

    public ExternalProviderSettingsHelper(
        ICurrentTenant currentTenant,
        IOptions<AbpExternalProviderOptions> externalProvidersOptions,
        ISettingManager settingManager,
        IJsonSerializer jsonSerializer)
    {
        CurrentTenant = currentTenant;
        ExternalProviderOptions = externalProvidersOptions.Value;
        SettingManager = settingManager;
        JsonSerializer = jsonSerializer;
    }

    public virtual async Task<List<ExternalProviderSettings>> GetAllAsync()
    {
        var allSettings = new List<ExternalProviderSettings>();

        List<ExternalProviderSettings> hostSettingsList;
        using (CurrentTenant.Change(null))
        {
            hostSettingsList = await GetSettingsListOrNullAsync(GlobalSettingValueProvider.ProviderName, null);
        }

        var providerSettingsList = CurrentTenant.IsAvailable
            ? await GetSettingsListOrNullAsync(TenantSettingValueProvider.ProviderName, CurrentTenant.Id?.ToString())
            : hostSettingsList;

        foreach (var externalProviderDefinition in ExternalProviderOptions.Definitions)
        {
            var newSettings = CreateSettings(externalProviderDefinition);
            var existSettings = providerSettingsList?.FirstOrDefault(x => x.Name == externalProviderDefinition.Name);
            if (existSettings != null)
            {
                CloneSettings(existSettings, newSettings);
            }

            if (CurrentTenant.IsAvailable)
            {
                newSettings.Enabled = hostSettingsList?.FirstOrDefault(x => x.Name == externalProviderDefinition.Name)?.Enabled
                                      ?? false;
            }

            allSettings.Add(newSettings);
        }

        return allSettings;
    }

    public virtual async Task<ExternalProviderSettings> GetByNameAsync(string name, bool fallBackToHost = false)
    {
        var definition = ExternalProviderOptions.Definitions.FirstOrDefault(x => x.Name == name);
        if (definition == null)
        {
            throw new AbpException($"External provider with {name} not definition!");
        }

        if (CurrentTenant.IsAvailable)
        {
            var settings = await GetSettingsAsync(definition, TenantSettingValueProvider.ProviderName, CurrentTenant.Id?.ToString());

            if (settings.IsValid() || !fallBackToHost)
            {
                return settings;
            }
        }

        return await GetSettingsAsync(definition, GlobalSettingValueProvider.ProviderName, null);
    }

    public virtual ExternalProviderDefinition GetDefinitionsByNameOrNull(string name)
    {
        return ExternalProviderOptions.Definitions.FirstOrDefault(x => x.Name == name);
    }

    public virtual async Task SetAsync(ExternalProviderSettings settings)
    {
        var definition = ExternalProviderOptions.Definitions.FirstOrDefault(x => x.Name == settings.Name);
        if (definition == null)
        {
            throw new AbpException($"External provider with {settings.Name} not definition!");
        }

        var newSettings = CreateSettings(definition);

        CloneSettings(settings, newSettings);

        if (CurrentTenant.IsAvailable)
        {
            newSettings.Enabled = true;
        }

        var existSettingsList = (CurrentTenant.IsAvailable
            ? await GetSettingsListOrNullAsync(TenantSettingValueProvider.ProviderName, CurrentTenant.Id?.ToString())
            : await GetSettingsListOrNullAsync(GlobalSettingValueProvider.ProviderName, null)) ?? new List<ExternalProviderSettings>();

        existSettingsList.RemoveAll(x => x.Name == definition.Name);
        existSettingsList.Add(newSettings);

        if (CurrentTenant.IsAvailable)
        {
            await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.ExternalProviders, JsonSerializer.Serialize(existSettingsList));
        }
        else
        {
            await SettingManager.SetGlobalAsync(AccountSettingNames.ExternalProviders, JsonSerializer.Serialize(existSettingsList));
        }
    }

    protected virtual async Task<ExternalProviderSettings> GetSettingsAsync(
        ExternalProviderDefinition definition,
        string providerName,
        string providerKey)
    {
        var newSettings = CreateSettings(definition);

        var settingsList = await GetSettingsListOrNullAsync(providerName, providerKey);
        var existSettings = settingsList?.FirstOrDefault(x => x.Name == definition.Name);
        if (existSettings != null)
        {
            CloneSettings(existSettings, newSettings);
        }

        return newSettings;
    }

    protected virtual async Task<List<ExternalProviderSettings>> GetSettingsListOrNullAsync(
        string providerName,
        string providerKey)
    {
        var settings = await SettingManager.GetOrNullAsync(AccountSettingNames.ExternalProviders, providerName, providerKey, fallback: false);
        return settings.IsNullOrWhiteSpace() ? null : JsonSerializer.Deserialize<List<ExternalProviderSettings>>(settings);
    }

    protected virtual ExternalProviderSettings CreateSettings(ExternalProviderDefinition definition)
    {
        return new ExternalProviderSettings
        {
            Name = definition.Name,
            Enabled = false,
            Properties = definition.Properties.
                Where(x => !x.IsSecret).
                Select(x => new ExternalProviderSettingsProperty(x.PropertyName, null)).
                ToList(),

            SecretProperties = definition.Properties.
                Where(x => x.IsSecret).
                Select(x => new ExternalProviderSettingsProperty(x.PropertyName, null)).
                ToList()
        };
    }

    protected virtual void CloneSettings(ExternalProviderSettings source, ExternalProviderSettings dest)
    {
        dest.Name = source.Name;
        dest.Enabled = source.Enabled;
        foreach (var item in dest.Properties)
        {
            item.Value = source.Properties.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))?.Value;
        }

        foreach (var item in dest.SecretProperties)
        {
            item.Value = source.SecretProperties.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }
}
