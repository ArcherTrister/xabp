// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Http;

using OpenIddict.Abstractions;

namespace X.Abp.OpenIddict.ExtensionGrantTypes;

public class ExtensionGrantContext
{
    public HttpContext HttpContext { get; }

    public OpenIddictRequest Request { get; }

    public ExtensionGrantContext(HttpContext httpContext, OpenIddictRequest request)
    {
        HttpContext = httpContext;
        Request = request;
    }
}
