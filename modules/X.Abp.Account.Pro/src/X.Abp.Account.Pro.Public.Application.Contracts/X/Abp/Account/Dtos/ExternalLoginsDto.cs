// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

namespace X.Abp.Account.Dtos;
public class ExternalLoginsDto
{
    public IList<UserLoginInfoDto> CurrentLogins { get; set; }

    public IList<AuthenticationSchemeDto> OtherLogins { get; set; }

    public bool ShowRemoveButton { get; set; }
}
