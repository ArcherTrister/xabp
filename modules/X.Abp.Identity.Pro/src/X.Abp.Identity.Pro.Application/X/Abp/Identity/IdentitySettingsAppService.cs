// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Volo.Abp.Features;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Ldap;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

using X.Abp.Identity.Features;
using X.Abp.Identity.Permissions;
using X.Abp.Identity.Settings;

namespace X.Abp.Identity;

[Authorize(AbpIdentityProPermissions.SettingManagement)]
public class IdentitySettingsAppService : IdentityAppServiceBase, IIdentitySettingsAppService
{
    protected ISettingManager SettingManager { get; }

    protected IOptions<IdentityOptions> IdentityOptions { get; }

    public IdentitySettingsAppService(ISettingManager settingManager, IOptions<IdentityOptions> identityOptions)
    {
        SettingManager = settingManager;
        IdentityOptions = identityOptions;
    }

    public virtual async Task<IdentitySettingsDto> GetAsync()
    {
        await IdentityOptions.SetAsync();

        return new IdentitySettingsDto
        {
            Password = new IdentityPasswordSettingsDto
            {
                RequiredLength = IdentityOptions.Value.Password.RequiredLength,
                RequiredUniqueChars = IdentityOptions.Value.Password.RequiredUniqueChars,
                RequireNonAlphanumeric = IdentityOptions.Value.Password.RequireNonAlphanumeric,
                RequireLowercase = IdentityOptions.Value.Password.RequireLowercase,
                RequireUppercase = IdentityOptions.Value.Password.RequireUppercase,
                RequireDigit = IdentityOptions.Value.Password.RequireDigit
            },
            Lockout = new IdentityLockoutSettingsDto
            {
                AllowedForNewUsers = IdentityOptions.Value.Lockout.AllowedForNewUsers,
                LockoutDuration = (int)IdentityOptions.Value.Lockout.DefaultLockoutTimeSpan.TotalSeconds,
                MaxFailedAccessAttempts = IdentityOptions.Value.Lockout.MaxFailedAccessAttempts
            },
            SignIn = new IdentitySignInSettingsDto
            {
                RequireConfirmedEmail = IdentityOptions.Value.SignIn.RequireConfirmedEmail,
                EnablePhoneNumberConfirmation = await SettingProvider.GetAsync(IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation, true),
                RequireConfirmedPhoneNumber = IdentityOptions.Value.SignIn.RequireConfirmedPhoneNumber
            },
            User = new IdentityUserSettingsDto
            {
                IsEmailUpdateEnabled = await SettingProvider.GetAsync(IdentitySettingNames.User.IsEmailUpdateEnabled, true),
                IsUserNameUpdateEnabled = await SettingProvider.GetAsync(IdentitySettingNames.User.IsUserNameUpdateEnabled, true)
            },
            OrganizationUnit = new IdentityOrganizationUnitSettingsDto
            {
                MaxUserMembershipCount = await SettingProvider.GetAsync(IdentitySettingNames.OrganizationUnit.MaxUserMembershipCount, int.MaxValue),
            }
        };
    }

    public virtual async Task UpdateAsync(IdentitySettingsDto input)
    {
        await IdentityOptions.SetAsync();

        if (input.Password != null)
        {
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.Password.RequiredLength, input.Password.RequiredLength.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.Password.RequiredUniqueChars, input.Password.RequiredUniqueChars.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.Password.RequireNonAlphanumeric, input.Password.RequireNonAlphanumeric.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.Password.RequireLowercase, input.Password.RequireLowercase.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.Password.RequireUppercase, input.Password.RequireUppercase.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.Password.RequireDigit, input.Password.RequireDigit.ToString());
        }

        if (input.Lockout != null)
        {
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.Lockout.AllowedForNewUsers, input.Lockout.AllowedForNewUsers.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.Lockout.MaxFailedAccessAttempts, input.Lockout.MaxFailedAccessAttempts.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.Lockout.LockoutDuration, input.Lockout.LockoutDuration.ToString());
        }

        if (input.SignIn != null)
        {
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail, input.SignIn.RequireConfirmedEmail.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber, input.SignIn.RequireConfirmedPhoneNumber.ToString());

            var enablePhoneNumberConfirmationValue = input.SignIn.EnablePhoneNumberConfirmation || input.SignIn.RequireConfirmedPhoneNumber;
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation, enablePhoneNumberConfirmationValue.ToString());
        }

        if (input.User != null)
        {
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.User.IsUserNameUpdateEnabled, input.User.IsUserNameUpdateEnabled.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.User.IsEmailUpdateEnabled, input.User.IsEmailUpdateEnabled.ToString());
        }

        if (input.OrganizationUnit != null)
        {
            await SettingManager.SetForCurrentTenantAsync(IdentitySettingNames.OrganizationUnit.MaxUserMembershipCount, input.OrganizationUnit.MaxUserMembershipCount.ToString());
        }
    }

    [RequiresFeature(IdentityProFeature.EnableLdapLogin)]
    public virtual async Task<IdentityLdapSettingsDto> GetLdapAsync()
    {
        return new IdentityLdapSettingsDto
        {
            EnableLdapLogin = await SettingProvider.GetAsync<bool>(IdentityProSettingNames.EnableLdapLogin),
            LdapServerHost = await SettingProvider.GetOrNullAsync(LdapSettingNames.ServerHost),
            LdapServerPort = await SettingProvider.GetOrNullAsync(LdapSettingNames.ServerPort),
            LdapBaseDc = await SettingProvider.GetOrNullAsync(LdapSettingNames.BaseDc),
            LdapDomain = await SettingProvider.GetOrNullAsync(LdapSettingNames.Domain),
            LdapUserName = await SettingProvider.GetOrNullAsync(LdapSettingNames.UserName),
            LdapPassword = await SettingProvider.GetOrNullAsync(LdapSettingNames.Password)
        };
    }

    [RequiresFeature(IdentityProFeature.EnableLdapLogin)]
    public virtual async Task UpdateLdapAsync(IdentityLdapSettingsDto input)
    {
        if (input != null)
        {
            await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.EnableLdapLogin, input.EnableLdapLogin.ToString());
            await SettingManager.SetForCurrentTenantAsync(LdapSettingNames.ServerHost, input.LdapServerHost);
            await SettingManager.SetForCurrentTenantAsync(LdapSettingNames.ServerPort, input.LdapServerPort);
            await SettingManager.SetForCurrentTenantAsync(LdapSettingNames.BaseDc, input.LdapBaseDc);
            await SettingManager.SetForCurrentTenantAsync(LdapSettingNames.Domain, input.LdapDomain);
            await SettingManager.SetForCurrentTenantAsync(LdapSettingNames.UserName, input.LdapUserName);
            if (!input.LdapPassword.IsNullOrWhiteSpace())
            {
                await SettingManager.SetForCurrentTenantAsync(LdapSettingNames.Password, input.LdapPassword);
            }
        }
    }

    [RequiresFeature(IdentityProFeature.EnableOAuthLogin)]
    public virtual async Task<IdentityOAuthSettingsDto> GetOAuthAsync()
    {
        return new IdentityOAuthSettingsDto
        {
            EnableOAuthLogin = await SettingProvider.GetAsync<bool>(IdentityProSettingNames.EnableOAuthLogin),
            ClientId = await SettingProvider.GetOrNullAsync(IdentityProSettingNames.OAuthLogin.ClientId),
            ClientSecret = await SettingProvider.GetOrNullAsync(IdentityProSettingNames.OAuthLogin.ClientSecret),
            RequireHttpsMetadata = await SettingProvider.GetAsync<bool>(IdentityProSettingNames.OAuthLogin.RequireHttpsMetadata),
            Scope = await SettingProvider.GetOrNullAsync(IdentityProSettingNames.OAuthLogin.Scope),
            Authority = await SettingProvider.GetOrNullAsync(IdentityProSettingNames.OAuthLogin.Authority)
        };
    }

    [RequiresFeature(IdentityProFeature.EnableOAuthLogin)]
    public virtual async Task UpdateOAuthAsync(IdentityOAuthSettingsDto input)
    {
        if (input != null)
        {
            await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.EnableOAuthLogin, input.EnableOAuthLogin.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.OAuthLogin.ClientId, input.ClientId);
            await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.OAuthLogin.ClientSecret, input.ClientSecret);
            await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.OAuthLogin.RequireHttpsMetadata, input.RequireHttpsMetadata.ToString());
            await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.OAuthLogin.Scope, input.Scope);
            await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.OAuthLogin.Authority, input.Authority);
        }
    }
}
