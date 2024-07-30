// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Identity;

public class ExternalLoginProviderDto
{
    public string Name { get; set; }

    public bool CanObtainUserInfoWithoutPassword { get; set; }

    public ExternalLoginProviderDto(string name, bool canObtainUserInfoWithoutPassword)
    {
        Name = name;
        CanObtainUserInfoWithoutPassword = canObtainUserInfoWithoutPassword;
    }
}
