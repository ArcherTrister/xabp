// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;

namespace X.Abp.Identity.Settings;

public class OAuthSettingProvider : IOAuthSettingProvider, ITransientDependency
{
    protected ISettingProvider SettingProvider { get; }

    public OAuthSettingProvider(ISettingProvider settingProvider)
    {
        SettingProvider = settingProvider;
    }

    public async Task<string> GetClientIdAsync()
    {
        return await SettingProvider.GetOrNullAsync(IdentityProSettingNames.OAuthLogin.ClientId);
    }

    public async Task<string> GetClientSecretAsync()
    {
        return await SettingProvider.GetOrNullAsync(IdentityProSettingNames.OAuthLogin.ClientSecret);
    }

    public async Task<string> GetAuthorityAsync()
    {
        return await SettingProvider.GetOrNullAsync(IdentityProSettingNames.OAuthLogin.Authority);
    }

    public async Task<string> GetScopeAsync()
    {
        return await SettingProvider.GetOrNullAsync(IdentityProSettingNames.OAuthLogin.Scope);
    }

    public async Task<bool> GetRequireHttpsMetadataAsync()
    {
        return (await SettingProvider.GetOrNullAsync(IdentityProSettingNames.OAuthLogin.RequireHttpsMetadata))?.To<bool>() ?? true;
    }
}
