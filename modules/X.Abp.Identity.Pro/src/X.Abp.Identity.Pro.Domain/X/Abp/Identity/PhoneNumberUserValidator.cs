// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;

namespace X.Abp.Identity;

public class PhoneNumberUserValidator : IUserValidator<IdentityUser>
{
    protected IIdentityUserRepository UserRepository { get; }

    protected IStringLocalizer<IdentityResource> Localizer { get; }

    public PhoneNumberUserValidator(
        IIdentityUserRepository userRepository,
        IStringLocalizer<IdentityResource> localizer)
    {
        UserRepository = userRepository;
        Localizer = localizer;
    }

    public virtual async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
    {
        var errors = new List<IdentityError>();

        await CheckIsNotDuplicateAsync(manager, user, errors);

        return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    protected virtual async Task CheckIsNotDuplicateAsync(UserManager<IdentityUser> userManager, IdentityUser user, ICollection<IdentityError> errors)
    {
        var phoneNumber = await userManager.GetPhoneNumberAsync(user);

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return;
        }

        var owner = await UserRepository.FindByPhoneNumberAsync(phoneNumber, false);

        if (owner != null &&
            !string.Equals(await userManager.GetUserIdAsync(owner), await userManager.GetUserIdAsync(user), StringComparison.Ordinal))
        {
            errors.Add(new IdentityError
            {
                Code = IdentityProErrorCodes.PhoneNumberAlreadyInUse,
                Description = Localizer[IdentityProErrorCodes.PhoneNumberAlreadyInUse, phoneNumber]
            });
        }
    }
}
