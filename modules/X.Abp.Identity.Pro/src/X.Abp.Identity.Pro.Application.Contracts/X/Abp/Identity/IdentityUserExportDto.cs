// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Identity;
public class IdentityUserExportDto
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Roles { get; set; }

    public string PhoneNumber { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Active { get; set; }

    public string AccountLookout { get; set; }

    public string EmailConfirmed { get; set; }

    public string TwoFactorEnabled { get; set; }

    public string AccessFailedCount { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? LastModificationTime { get; set; }
}
