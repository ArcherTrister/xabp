// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Http;

using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

using Volo.Abp;

namespace X.Abp.OpenIddict;

public static class AbpOpenIddictHttpContextExtensions
{
    public static OpenIddictServerTransaction GetOpenIddictServerTransaction(this HttpContext context)
    {
        Check.NotNull(context, nameof(context));
        return context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction;
    }
}
