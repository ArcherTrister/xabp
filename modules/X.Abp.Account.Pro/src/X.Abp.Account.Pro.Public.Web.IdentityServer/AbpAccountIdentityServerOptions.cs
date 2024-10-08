﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IdentityServer4.Validation;

using Microsoft.AspNetCore.Http;

using Volo.Abp.MultiTenancy;

namespace X.Abp.Account.Web;

public class AbpAccountIdentityServerOptions
{
    public string ImpersonationAuthenticationScheme { get; set; }

    public string LinkLoginAuthenticationScheme { get; set; }

    public bool IsTenantMultiDomain { get; set; }

    public Func<HttpContext, ExtensionGrantValidationContext, BasicTenantInfo, Task<string>> GetTenantDomain { get; set; }

    public Dictionary<string, string> ClientIdToDeviceMap { get; }

    public AbpAccountIdentityServerOptions()
    {
        ImpersonationAuthenticationScheme = "Bearer";
        LinkLoginAuthenticationScheme = "Bearer";
        GetTenantDomain = (context, _, _) => Task.FromResult(context.Request.Scheme + "://" + context.Request.Host);
        ClientIdToDeviceMap = new Dictionary<string, string>();
    }
}
