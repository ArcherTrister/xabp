// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.
/*
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict;
using Volo.Abp.Security.Claims;

namespace AbpVnext.Pro.Customs;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(AbpDynamicClaimsOpenIddictClaimsPrincipalHandler))]
public class CustomAbpDynamicClaimsOpenIddictClaimsPrincipalHandler : AbpDynamicClaimsOpenIddictClaimsPrincipalHandler
{
    public override async Task HandleAsync(AbpOpenIddictClaimsPrincipalHandlerContext context)
    {
        IAbpClaimsPrincipalFactory requiredService = context.ScopeServiceProvider.GetRequiredService<IAbpClaimsPrincipalFactory>();
        await requiredService.CreateDynamicAsync(context.Principal);
    }
}*/
