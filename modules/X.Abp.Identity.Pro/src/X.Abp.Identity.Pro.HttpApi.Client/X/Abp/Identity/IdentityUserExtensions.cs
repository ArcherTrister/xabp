// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Users;

namespace X.Abp.Identity;

public static class IdentityUserDtoExtensions
{
    public static IUserData ToUserInfo(this IdentityUserDto user)
    {
        return new UserData(
            user.Id,
            user.UserName,
            user.Email,
            user.Name,
            user.Surname,
            user.EmailConfirmed,
            user.PhoneNumber,
            user.PhoneNumberConfirmed,
            user.TenantId
        );
    }
}
