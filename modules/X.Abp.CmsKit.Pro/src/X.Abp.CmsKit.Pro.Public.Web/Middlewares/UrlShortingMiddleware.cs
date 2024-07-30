// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.DependencyInjection;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.GlobalFeatures;
using X.Abp.CmsKit.Public.UrlShorting;

namespace X.Abp.CmsKit.Pro.Public.Web.Middlewares;

public class UrlShortingMiddleware : IMiddleware, ITransientDependency
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);
        if (GlobalFeatureManager.Instance.IsEnabled<UrlShortingFeature>() && context.Response.StatusCode == 404)
        {
            var shortenedUrlDto = await context.RequestServices.GetRequiredService<IUrlShortingPublicAppService>().FindBySourceAsync(context.Request.Path.ToString());
            if (shortenedUrlDto != null)
            {
                context.Response.Redirect(shortenedUrlDto.Target, true);
            }
        }
    }
}
