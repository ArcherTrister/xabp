// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Authentication;

using OpenIddict.Validation.AspNetCore;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderAbpOpenIddictMiddlewareExtension
{
    public static IApplicationBuilder UseAbpOpenIddictValidation(this IApplicationBuilder app, string schema = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
    {
        return app.Use(async (ctx, next) =>
        {
            if (ctx.User.Identity?.IsAuthenticated != true)
            {
                var result = await ctx.AuthenticateAsync(schema);
                if (result.Succeeded && result.Principal != null)
                {
                    ctx.User = result.Principal;
                }
            }

            await next();
        });
    }
}
