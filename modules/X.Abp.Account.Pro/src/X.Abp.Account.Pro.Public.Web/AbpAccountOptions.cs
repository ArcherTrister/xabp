// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Volo.Abp.MultiTenancy;

namespace X.Abp.Account.Public.Web;

public class AbpAccountOptions
{
    /// <summary>
    /// Default value: "Windows".
    /// </summary>
    public string WindowsAuthenticationSchemeName { get; set; }

    /// <summary>
    /// Default value: "admin".
    /// </summary>
    public string TenantAdminUserName { get; set; }

    public string ImpersonationTenantPermission { get; set; }

    public string ImpersonationUserPermission { get; set; }

    public Dictionary<string, string> ExternalProviderIconMap { get; }

    public bool IsTenantMultiDomain { get; set; }

    public Func<HttpContext, BasicTenantInfo, Task<string>> GetTenantDomain { get; set; }

    public AbpAccountOptions()
    {
        TenantAdminUserName = "admin";

        // Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;
        WindowsAuthenticationSchemeName = "Windows";

        GetTenantDomain = (context, _) => Task.FromResult(context.Request.Scheme + "://" + context.Request.Host);

        ExternalProviderIconMap = new Dictionary<string, string>
        {
            { "Amazon", "fa fa-amazon" },
            { "Apple", "fa fa-apple" },
            { "BattleNet", "fab fa-battle-net" },
            { "Discord", "fab fa-discord" },
            { "Dropbox", "fa fa-dropbox" },
            { "Facebook", "fa fa-facebook" },
            { "GitHub", "fa fa-github" },
            { "Google", "fa fa-google" },
            { "Instagram", "fa fa-instagram" },
            { "LinkedIn", "fa fa-linkedin" },
            { "Microsoft", "fa fa-windows" },
            { "Twitch", "fa fa-twitch" },
            { "Twitter", "fa fa-twitter" },
            { "Yandex", "fab fa-yandex-international" },
            { "Weibo", "fa fa-weibo" }
        };
    }
}
