// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using OpenIddict.Validation.AspNetCore;

using Volo.Abp.MultiTenancy;

namespace X.Abp.Account.Web;

public class AbpAccountOpenIddictOptions
{
    public string ImpersonationAuthenticationScheme { get; set; }

    public string LinkLoginAuthenticationScheme { get; set; }

    public bool IsTenantMultiDomain { get; set; }

    public Func<HttpContext, BasicTenantInfo, Task<string>> GetTenantDomain { get; set; }

    public Dictionary<string, string> ClientIdToDeviceMap { get; }

    public AbpAccountOpenIddictOptions()
    {
        ImpersonationAuthenticationScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        LinkLoginAuthenticationScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        GetTenantDomain = (HttpContext context, BasicTenantInfo _) => Task.FromResult(context.Request.Scheme + "://" + context.Request.Host);
        ClientIdToDeviceMap = new Dictionary<string, string>();
    }
}
