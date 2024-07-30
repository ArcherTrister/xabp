// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account.Public.Web.Areas.Account.Controllers.Models;

public enum LoginResultType : int
{
    None = 0,

    Success = 1,

    InvalidUserNameOrPassword = 2,

    NotAllowed = 3,

    LockedOut = 4,

    RequiresTwoFactor = 5,

    NotLinked = 6
}
