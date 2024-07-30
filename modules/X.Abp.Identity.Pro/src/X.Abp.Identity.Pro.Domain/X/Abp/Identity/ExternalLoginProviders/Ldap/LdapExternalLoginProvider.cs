// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Ldap;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;

using X.Abp.Identity.Features;
using X.Abp.Identity.Settings;

namespace X.Abp.Identity.ExternalLoginProviders.Ldap;

public class LdapExternalLoginProvider : ExternalLoginProviderBase, ITransientDependency
{
    public const string Name = "Ldap";

    protected ILogger<LdapExternalLoginProvider> Logger { get; set; }

    protected OpenLdapManager LdapManager { get; }

    protected ILdapSettingProvider LdapSettingProvider { get; }

    protected IFeatureChecker FeatureChecker { get; }

    protected ISettingProvider SettingProvider { get; }

    public LdapExternalLoginProvider(IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IdentityUserManager userManager,
        IIdentityUserRepository identityUserRepository,
        OpenLdapManager ldapManager,
        ILdapSettingProvider ldapSettingProvider,
        IFeatureChecker featureChecker,
        ISettingProvider settingProvider,
        IOptions<IdentityOptions> identityOptions)
            : base(guidGenerator,
                currentTenant,
                userManager,
                identityUserRepository,
                identityOptions)
    {
        LdapManager = ldapManager;
        LdapSettingProvider = ldapSettingProvider;
        FeatureChecker = featureChecker;
        SettingProvider = settingProvider;

        Logger = NullLogger<LdapExternalLoginProvider>.Instance;
    }

    public override async Task<bool> TryAuthenticateAsync(string userName, string plainPassword)
    {
        Logger.LogInformation("Try to use LDAP for external authentication");

        return await IsEnabledAsync() && await LdapManager.AuthenticateAsync(await NormalizeUserNameAsync(userName), plainPassword);
    }

    public override async Task<bool> IsEnabledAsync()
    {
        if (!await FeatureChecker.IsEnabledAsync(IdentityProFeature.EnableLdapLogin))
        {
            Logger.LogWarning("Ldap login feature is not enabled!");
            return false;
        }

        if (!await SettingProvider.IsTrueAsync(IdentityProSettingNames.EnableLdapLogin))
        {
            Logger.LogWarning("Ldap login setting is not enabled!");
            return false;
        }

        return true;
    }

    protected override async Task<ExternalLoginUserInfo> GetUserInfoAsync(string userName)
    {
        var email = await LdapManager.GetUserEmailAsync(userName);
        return email.IsNullOrWhiteSpace() ? throw new Exception("Unable to get the email of ldap user!") : new ExternalLoginUserInfo(email);
    }

    protected virtual async Task<string> NormalizeUserNameAsync(string userName)
    {
        return $"uid={userName}, {await LdapSettingProvider.GetBaseDcAsync()}";
    }
}
