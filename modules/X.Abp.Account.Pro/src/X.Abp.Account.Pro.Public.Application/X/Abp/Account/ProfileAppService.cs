// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
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
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Timing;
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

    protected IdentityProTwoFactorManager IdentityProTwoFactorManager { get; }

    protected IOptions<IdentityOptions> IdentityOptions { get; }

    protected IdentityUserTwoFactorChecker IdentityUserTwoFactorChecker { get; }

    protected ITimezoneProvider TimezoneProvider { get; }

    protected ISettingManager SettingManager { get; }

    public ProfileAppService(
        UrlEncoder urlEncoder,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        IdentityProTwoFactorManager identityProTwoFactorManager,
        IOptions<IdentityOptions> identityOptions,
        IdentityUserTwoFactorChecker identityUserTwoFactorChecker,
        ITimezoneProvider timezoneProvider,
        ISettingManager settingManager)
    {
        UrlEncoder = urlEncoder;
        UserManager = userManager;
        IdentitySecurityLogManager = identitySecurityLogManager;
        IdentityProTwoFactorManager = identityProTwoFactorManager;
        IdentityOptions = identityOptions;
        IdentityUserTwoFactorChecker = identityUserTwoFactorChecker;
        TimezoneProvider = timezoneProvider;
        SettingManager = settingManager;
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
                await IdentityUserTwoFactorChecker.CheckAsync(user);
                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                {
                    Identity = IdentitySecurityLogIdentityConsts.Identity,
                    Action = IdentitySecurityLogActionConsts.ChangeEmail
                });
            }
        }

        if (user.PhoneNumber.IsNullOrWhiteSpace() && input.PhoneNumber.IsNullOrWhiteSpace())
        {
            input.PhoneNumber = user.PhoneNumber;
        }

        if (!string.Equals(user.PhoneNumber, input.PhoneNumber, StringComparison.OrdinalIgnoreCase))
        {
            (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckIdentityErrors();
            await IdentityUserTwoFactorChecker.CheckAsync(user);
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

        await SettingManager.SetForCurrentUserAsync(TimingSettingNames.TimeZone, input.Timezone);

        ProfileDto profileDto = await SetTimezoneInfoAsync(ObjectMapper.Map<IdentityUser, ProfileDto>(user));

        await CurrentUnitOfWork.SaveChangesAsync();

        return profileDto;
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
        if (await IdentityProTwoFactorManager.IsOptionalAsync())
        {
            if (await SettingProvider.GetAsync<bool>(IdentityProSettingNames.TwoFactor.UsersCanChange))
            {
                var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());
                if (currentUser.TwoFactorEnabled != enabled)
                {
                    if (enabled)
                    {
                        if (!await IdentityUserTwoFactorChecker.CanEnabledAsync(currentUser))
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

        return await IdentityUserTwoFactorChecker.CanEnabledAsync(user);
    }

    public virtual Task<List<NameValue>> GetTimezonesAsync()
    {
        return Task.FromResult(TimeZoneHelper.GetTimezones(TimezoneProvider.GetWindowsTimezones()));
    }

    protected virtual async Task<ProfileDto> SetTimezoneInfoAsync(
      ProfileDto profileDto)
    {
        profileDto.SupportsMultipleTimezone = Clock.SupportsMultipleTimezone;
        profileDto.Timezone = await SettingProvider.GetOrNullAsync(TimingSettingNames.TimeZone);

        return profileDto;
    }
}
