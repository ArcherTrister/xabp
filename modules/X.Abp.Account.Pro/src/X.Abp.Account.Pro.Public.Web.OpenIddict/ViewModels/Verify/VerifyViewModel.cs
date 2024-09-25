// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace X.Abp.Account.Web.ViewModels.Verify;

public class VerifyViewModel
{
    public string ApplicationName { get; set; }

    [BindNever]
    public string Error { get; set; }

    [BindNever]
    public string ErrorDescription { get; set; }

    public string Scope { get; set; }

    [FromQuery(Name = "user_code")]
    public string UserCode { get; set; }
}
