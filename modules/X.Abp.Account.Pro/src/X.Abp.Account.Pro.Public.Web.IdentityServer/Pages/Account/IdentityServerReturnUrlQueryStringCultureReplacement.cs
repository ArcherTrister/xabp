// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.DependencyInjection;

namespace X.Abp.Account.Web.Pages.Account;

[Dependency(ReplaceServices = true)]
public class IdentityServerReturnUrlQueryStringCultureReplacement : AbpAspNetCoreMvcQueryStringCultureReplacement
{
    public override async Task ReplaceAsync(QueryStringCultureReplacementContext context)
    {
        await base.ReplaceAsync(context);

        if (!string.IsNullOrWhiteSpace(context.ReturnUrl))
        {
            if (context.ReturnUrl.Contains("culture%3D", StringComparison.OrdinalIgnoreCase) &&
                context.ReturnUrl.Contains("ui-Culture%3D", StringComparison.OrdinalIgnoreCase))
            {
                context.ReturnUrl = Regex.Replace(
                    context.ReturnUrl,
                    "culture%3D[A-Za-z-]+",
                    $"culture%3D{context.RequestCulture.Culture}",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

                context.ReturnUrl = Regex.Replace(
                    context.ReturnUrl,
                    "ui-culture%3D[A-Za-z-]+",
                    $"ui-culture%3D{context.RequestCulture.UICulture}",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }

            if (context.ReturnUrl.Contains("Culture=", StringComparison.OrdinalIgnoreCase) &&
                context.ReturnUrl.Contains("UICulture=", StringComparison.OrdinalIgnoreCase))
            {
                context.ReturnUrl = Regex.Replace(
                    context.ReturnUrl,
                    "Culture=[A-Za-z-]+",
                    $"Culture={context.RequestCulture.Culture}",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

                context.ReturnUrl = Regex.Replace(
                    context.ReturnUrl,
                    "UICulture=[A-Za-z-]+",
                    $"UICulture={context.RequestCulture.UICulture}",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
        }
    }
}
