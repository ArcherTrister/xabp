// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account.Public.Web.Areas.Account.Controllers.Models;

public class ScanCodeLoginInfo
{
    public string QrCodeKey { get; set; }

    public string ScanCodeToken { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Scope { get; set; }

    public string ReturnUrl { get; set; }
}
