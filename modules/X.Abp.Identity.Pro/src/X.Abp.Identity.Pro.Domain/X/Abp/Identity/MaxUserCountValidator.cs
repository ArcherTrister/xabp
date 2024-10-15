// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;

using X.Abp.Identity.Features;

namespace X.Abp.Identity;

public class MaxUserCountValidator : IUserValidator<IdentityUser>
{
    protected IFeatureChecker FeatureChecker { get; }

    protected IIdentityUserRepository UserRepository { get; }

    protected IStringLocalizer<IdentityResource> Localizer { get; }

    public MaxUserCountValidator(IFeatureChecker featureChecker, IIdentityUserRepository userRepository, IStringLocalizer<IdentityResource> localizer)
    {
        FeatureChecker = featureChecker;
        UserRepository = userRepository;
        Localizer = localizer;
    }

    public virtual async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
    {
        var errors = new List<IdentityError>();

        await CheckMaxUserCountAsync(user, errors);

        return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    protected virtual async Task CheckMaxUserCountAsync(IdentityUser user, ICollection<IdentityError> errors)
    {
        var maxUserCount = await FeatureChecker.GetAsync<int>(IdentityProFeature.MaxUserCount);
        if (maxUserCount <= 0)
        {
            return;
        }

        var existUser = await UserRepository.FindAsync(user.Id, false);
        if (existUser == null)
        {
            var currentUserCount = await UserRepository.GetCountAsync();
            if (currentUserCount >= maxUserCount)
            {
                errors.Add(new IdentityError
                {
                    Code = IdentityProErrorCodes.MaximumUserCount,
                    Description = Localizer[IdentityProErrorCodes.MaximumUserCount, maxUserCount]
                });
            }
        }
    }
}
