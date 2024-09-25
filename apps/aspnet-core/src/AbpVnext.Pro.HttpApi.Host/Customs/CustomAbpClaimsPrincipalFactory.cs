// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace AbpVnext.Pro.Customs;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(AbpClaimsPrincipalFactory))]
public class CustomAbpClaimsPrincipalFactory : AbpClaimsPrincipalFactory
{
    public CustomAbpClaimsPrincipalFactory(IServiceScopeFactory serviceScopeFactory, IOptions<AbpClaimsPrincipalFactoryOptions> abpClaimOptions)
        : base(serviceScopeFactory, abpClaimOptions)
    {
    }

    public override async Task<ClaimsPrincipal> InternalCreateAsync(AbpClaimsPrincipalFactoryOptions options, ClaimsPrincipal existsClaimsPrincipal = null, bool isDynamic = false)
    {
        using IServiceScope scope = ServiceScopeFactory.CreateScope();
        ClaimsPrincipal claimsIdentity = existsClaimsPrincipal ?? new ClaimsPrincipal(new ClaimsIdentity(AuthenticationType, AbpClaimTypes.UserName, AbpClaimTypes.Role));
        AbpClaimsPrincipalContributorContext context = new AbpClaimsPrincipalContributorContext(claimsIdentity, scope.ServiceProvider);
        if (!isDynamic)
        {
            foreach (Type contributor in options.Contributors)
            {
                await ((IAbpClaimsPrincipalContributor)scope.ServiceProvider.GetRequiredService(contributor)).ContributeAsync(context);
            }
        }
        else
        {
            foreach (Type dynamicContributor in options.DynamicContributors)
            {
                await ((IAbpDynamicClaimsPrincipalContributor)scope.ServiceProvider.GetRequiredService(dynamicContributor)).ContributeAsync(context);
            }
        }

        return context.ClaimsPrincipal;
    }
}
