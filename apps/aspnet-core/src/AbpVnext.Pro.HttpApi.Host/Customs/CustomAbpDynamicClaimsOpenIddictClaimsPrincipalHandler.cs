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
}
