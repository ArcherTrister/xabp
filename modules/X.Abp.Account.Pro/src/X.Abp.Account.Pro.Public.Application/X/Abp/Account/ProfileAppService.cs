// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Settings;
using Volo.Abp.Users;

using X.Abp.Account.Dtos;
using X.Abp.Identity;
using X.Abp.Identity.Settings;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account;

[Authorize]
public class ProfileAppService : ApplicationService, IProfileAppService
{
    protected UrlEncoder UrlEncoder { get; }

    protected IdentityUserManager UserManager { get; }

    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }

    protected IdentityTwoFactorManager IdentityTwoFactorManager { get; }

    protected IOptions<IdentityOptions> IdentityOptions { get; }

    public ProfileAppService(
        UrlEncoder urlEncoder,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        IdentityTwoFactorManager identityTwoFactorManager,
        IOptions<IdentityOptions> identityOptions)
    {
        UrlEncoder = urlEncoder;
        UserManager = userManager;
        IdentitySecurityLogManager = identitySecurityLogManager;
        IdentityTwoFactorManager = identityTwoFactorManager;
        IdentityOptions = identityOptions;
    }

    public virtual async Task<ProfileDto> GetAsync()
    {
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        return ObjectMapper.Map<IdentityUser, ProfileDto>(currentUser);
    }

    public virtual async Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
    {
        await IdentityOptions.SetAsync();

        var user = await UserManager.GetByIdAsync(CurrentUser.GetId());

        user.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        if (!string.Equals(user.UserName, input.UserName, StringComparison.OrdinalIgnoreCase))
        {
            if (await SettingProvider.IsTrueAsync(IdentitySettingNames.User.IsUserNameUpdateEnabled))
            {
                (await UserManager.SetUserNameAsync(user, input.UserName)).CheckIdentityErrors();
                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                {
                    Identity = IdentitySecurityLogIdentityConsts.Identity,
                    Action = IdentitySecurityLogActionConsts.ChangeUserName
                });
            }
        }

        if (!string.Equals(user.Email, input.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await SettingProvider.IsTrueAsync(IdentitySettingNames.User.IsEmailUpdateEnabled))
            {
                (await UserManager.SetEmailAsync(user, input.Email)).CheckIdentityErrors();
                await CheckAsync(user);
                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                {
                    Identity = IdentitySecurityLogIdentityConsts.Identity,
                    Action = IdentitySecurityLogActionConsts.ChangeEmail
                });
            }
        }

        if (!string.Equals(user.PhoneNumber, input.PhoneNumber, StringComparison.OrdinalIgnoreCase))
        {
            (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckIdentityErrors();
            await CheckAsync(user);
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = IdentitySecurityLogActionConsts.ChangePhoneNumber
            });
        }

        user.Name = input.Name;
        user.Surname = input.Surname;

        input.MapExtraPropertiesTo(user);

        (await UserManager.UpdateAsync(user)).CheckIdentityErrors();

        await CurrentUnitOfWork.SaveChangesAsync();

        return ObjectMapper.Map<IdentityUser, ProfileDto>(user);
    }

    public virtual async Task ChangePasswordAsync(ChangePasswordInput input)
    {
        await IdentityOptions.SetAsync();

        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        if (currentUser.IsExternal)
        {
            throw new BusinessException(code: IdentityErrorCodes.ExternalUserPasswordChange);
        }

        if (currentUser.PasswordHash == null)
        {
            (await UserManager.AddPasswordAsync(currentUser, input.NewPassword)).CheckIdentityErrors();
        }
        else
        {
            (await UserManager.ChangePasswordAsync(currentUser, input.CurrentPassword, input.NewPassword)).CheckIdentityErrors();
        }

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePassword
        });
    }

    public virtual async Task<bool> GetTwoFactorEnabledAsync()
    {
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());
        return await UserManager.GetTwoFactorEnabledAsync(currentUser);
    }

    public virtual async Task SetTwoFactorEnabledAsync(bool enabled)
    {
        if (await IdentityTwoFactorManager.IsOptionalAsync())
        {
            if (await SettingProvider.GetAsync<bool>(IdentityProSettingNames.TwoFactor.UsersCanChange))
            {
                var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());
                if (currentUser.TwoFactorEnabled != enabled)
                {
                    if (enabled)
                    {
                        if (!await CanEnabledAsync(currentUser))
                        {
                            throw new UserFriendlyException(L["YouHaveToEnableAtLeastOneTwoFactorProvider"]);
                        }
                    }

                    (await UserManager.SetTwoFactorEnabledAsync(currentUser, enabled)).CheckIdentityErrors();
                }
            }
            else
            {
                throw new BusinessException(code: IdentityErrorCodes.UsersCanNotChangeTwoFactor);
            }
        }
        else
        {
            throw new BusinessException(code: IdentityErrorCodes.CanNotChangeTwoFactor);
        }
    }

    public virtual async Task<bool> CanEnableTwoFactorAsync()
    {
        var user = await UserManager.GetByIdAsync(CurrentUser.GetId());

        return await CanEnabledAsync(user);
    }

    protected virtual async Task<bool> CanEnabledAsync(IdentityUser user)
    {
        var providers = await UserManager.GetValidTwoFactorProvidersAsync(user);
        return providers.Count != 0 && (providers.Count != 1 || !providers.Contains("Authenticator"));
    }

    protected virtual async Task CheckAsync(IdentityUser user)
    {
        if (await CanEnabledAsync(user))
        {
            return;
        }

        (await UserManager.SetTwoFactorEnabledAsync(user, false)).CheckIdentityErrors();
    }
}
