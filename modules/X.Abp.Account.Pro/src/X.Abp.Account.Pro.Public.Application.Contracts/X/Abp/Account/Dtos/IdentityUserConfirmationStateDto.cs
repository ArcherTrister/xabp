// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account.Dtos;

public class IdentityUserConfirmationStateDto
{
    public bool EmailConfirmed { get; set; }

    public bool PhoneNumberConfirmed { get; set; }
}
