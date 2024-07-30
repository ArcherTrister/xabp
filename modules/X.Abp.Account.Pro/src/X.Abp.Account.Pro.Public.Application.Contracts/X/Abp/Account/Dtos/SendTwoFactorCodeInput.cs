// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

namespace X.Abp.Account.Dtos;

public class SendTwoFactorCodeInput
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Provider { get; set; }

    [Required]
    public string Token { get; set; }
}
