// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace X.Abp.Account.Public.Web.Areas.Account.Controllers.Models;

public class UserLoginInfo
{
    [Required]
    [StringLength(255)]
    public string UserNameOrEmailAddress { get; set; }

    [Required]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    [DataType(DataType.Password)]
    [DisableAuditing]
    public string Password { get; set; }

    public bool RememberMe { get; set; }

    public Guid? TenanId { get; set; }
}
