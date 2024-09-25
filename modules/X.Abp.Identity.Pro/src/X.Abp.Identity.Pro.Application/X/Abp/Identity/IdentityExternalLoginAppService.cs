// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace X.Abp.Identity
{
    public class IdentityExternalLoginAppService :
      IdentityAppServiceBase,
      IIdentityExternalLoginAppService
    {
        protected IdentityUserManager UserManager { get; }

        public IdentityExternalLoginAppService(IdentityUserManager userManager)
        {
            UserManager = userManager;
        }

        [Authorize]
        public virtual async Task CreateOrUpdateAsync()
        {
            IdentityUser user = await UserManager.FindByIdAsync(CurrentUser.Id.ToString());
            if (user == null)
            {
                await CreateCurrentUserAsync();
            }
            else
            {
                await UpdateCurrentUserAsync(user);
            }
        }

        protected virtual async Task CreateCurrentUserAsync()
        {
            IdentityUser user = new IdentityUser(CurrentUser.GetId(), CurrentUser.UserName, CurrentUser.Email, CurrentTenant.Id);
            user.SetEmailConfirmed(CurrentUser.EmailVerified);
            if (!CurrentUser.PhoneNumber.IsNullOrEmpty())
            {
                user.SetPhoneNumber(CurrentUser.PhoneNumber, CurrentUser.PhoneNumberVerified);
            }

            user.Name = CurrentUser.Name;
            user.Surname = CurrentUser.SurName;
            (await UserManager.CreateAsync(user)).CheckIdentityErrors();
        }

        protected virtual async Task UpdateCurrentUserAsync(IdentityUser user)
        {
            if (!CurrentUser.UserName.IsNullOrWhiteSpace() && !string.Equals(user.UserName, CurrentUser.UserName, StringComparison.InvariantCultureIgnoreCase))
            {
                IdentityResult identityResult = await UserManager.SetUserNameAsync(user, CurrentUser.UserName);
            }

            if (!CurrentUser.Email.IsNullOrWhiteSpace() && !string.Equals(user.Email, CurrentUser.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                (await UserManager.SetEmailAsync(user, CurrentUser.Email)).CheckIdentityErrors();
                user.SetEmailConfirmed(CurrentUser.EmailVerified);
            }

            if (!CurrentUser.PhoneNumber.IsNullOrWhiteSpace() && !string.Equals(user.PhoneNumber, CurrentUser.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                (await UserManager.SetPhoneNumberAsync(user, CurrentUser.PhoneNumber)).CheckIdentityErrors();
                user.SetPhoneNumberConfirmed(CurrentUser.PhoneNumberVerified);
            }

            if (!CurrentUser.Name.IsNullOrWhiteSpace() && !string.Equals(user.Name, CurrentUser.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                user.Name = CurrentUser.Name;
            }

            if (!CurrentUser.SurName.IsNullOrWhiteSpace() && !string.Equals(user.Surname, CurrentUser.SurName, StringComparison.InvariantCultureIgnoreCase))
            {
                user.Surname = CurrentUser.SurName;
            }

            (await UserManager.UpdateAsync(user)).CheckIdentityErrors();
        }
    }
}
