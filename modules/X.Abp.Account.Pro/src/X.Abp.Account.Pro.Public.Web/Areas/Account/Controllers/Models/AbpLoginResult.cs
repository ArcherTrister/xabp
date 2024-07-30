// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account.Public.Web.Areas.Account.Controllers.Models;

public class AbpLoginResult
{
    public AbpLoginResult(LoginResultType result)
    {
        Result = result;
    }

    public LoginResultType Result { get; }

    public string Description => Result.ToString();
}
