// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using JetBrains.Annotations;

namespace X.Abp.Account.Dtos;

public class SendPhoneNumberConfirmationTokenDto
{
    [Required]
    public Guid UserId { get; set; }

    [CanBeNull]
    public string PhoneNumber { get; set; }
}
