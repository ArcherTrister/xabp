// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace X.Abp.Account.Public.Web.Impersonation;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule))]
public class AbpAccountPublicWebImpersonationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, AbpAccountImpersonationJwtBearerConfigureOptions>();
        context.Services.AddSingleton<IPostConfigureOptions<OpenIdConnectOptions>, AbpAccountImpersonationOpenIdConnectConfigureOptions>();
    }
}
