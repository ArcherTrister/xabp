using System;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Middleware;
using Volo.Abp.AspNetCore.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict.Controllers;
using Volo.Abp.Security.Claims;

namespace AbpVnext.Pro.Customs;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(AbpDynamicClaimsMiddleware))]
public class CustomAbpDynamicClaimsMiddleware : AbpDynamicClaimsMiddleware
{
    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if ((context.User.Identity?.IsAuthenticated ?? false) && context.RequestServices.GetRequiredService<IOptions<AbpClaimsPrincipalFactoryOptions>>().Value.IsDynamicClaimsEnabled)
        {
            string authenticationType = context.User.Identity.AuthenticationType;
            IAbpClaimsPrincipalFactory requiredService = context.RequestServices.GetRequiredService<IAbpClaimsPrincipalFactory>();
            context.User = await requiredService.CreateDynamicAsync(context.User);
            IIdentity identity = context.User.Identity;
            if (identity != null && !identity.IsAuthenticated)
            {
                IAuthenticationSchemeProvider requiredService2 = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
                if (!authenticationType.IsNullOrWhiteSpace())
                {
                    AuthenticationScheme authenticationScheme = await requiredService2.GetSchemeAsync(authenticationType);
                    if (authenticationScheme != null && typeof(IAuthenticationSignOutHandler).IsAssignableFrom(authenticationScheme.HandlerType))
                    {
                        await context.SignOutAsync(authenticationScheme.Name);
                    }
                }
            }
        }

        await next(context);
    }
}

//public class CustomAbpDynamicClaimsMiddleware : AbpMiddlewareBase, ITransientDependency
//{
//    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
//    {
//        if ((context.User.Identity?.IsAuthenticated ?? false) && context.RequestServices.GetRequiredService<IOptions<AbpClaimsPrincipalFactoryOptions>>().Value.IsDynamicClaimsEnabled)
//        {
//            string authenticationType = context.User.Identity.AuthenticationType;
//            IAbpClaimsPrincipalFactory requiredService = context.RequestServices.GetRequiredService<IAbpClaimsPrincipalFactory>();
//            context.User = await requiredService.CreateDynamicAsync(context.User);
//            IIdentity? identity = context.User.Identity;
//            if (identity != null && !identity.IsAuthenticated)
//            {
//                IAuthenticationSchemeProvider requiredService2 = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
//                if (!authenticationType.IsNullOrWhiteSpace())
//                {
//                    AuthenticationScheme authenticationScheme = await requiredService2.GetSchemeAsync(authenticationType);
//                    if (authenticationScheme != null && typeof(IAuthenticationSignOutHandler).IsAssignableFrom(authenticationScheme.HandlerType))
//                    {
//                        await context.SignOutAsync(authenticationScheme.Name);
//                    }
//                }
//            }
//        }

//        await next(context);
//    }
//}
