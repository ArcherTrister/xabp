// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using IdentityServer4.Configuration;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RequestLocalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.IdentityServer;
using Volo.Abp.IdentityServer.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account.Localization;
using X.Abp.Account.Public.Web;
using X.Abp.Account.Public.Web.Security.Claims;
using X.Abp.Account.Web.ExtensionGrantValidators;
using X.Abp.Account.Web.Pages.Account;
using X.Abp.Account.Web.Services;
using X.Abp.IdentityServer;

namespace X.Abp.Account.Web;

[DependsOn(
    typeof(AbpAccountPublicWebModule),
    typeof(AbpIdentityServerProDomainModule))]
public class AbpAccountPublicWebIdentityServerModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(AccountResource),
                typeof(AbpAccountPublicApplicationContractsModule).Assembly,
                typeof(AbpAccountPublicWebIdentityServerModule).Assembly);
        });

        PreConfigure<AbpIdentityAspNetCoreOptions>(options =>
        {
            options.ConfigureAuthentication = false;
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpAccountPublicWebIdentityServerModule).Assembly);
        });

        PreConfigure<IIdentityServerBuilder>(identityServerBuilder =>
        {
            // builder.AddPersistedGrantStore<CustomPersistedGrantStore>();
            // builder.AddResponseGenerators
            // AddResourceOwnerValidator
            // CustomTokenValidator
            identityServerBuilder.AddProfileService<ExtraClaimsProfileService>();
            identityServerBuilder.AddExtensionGrantValidator<LinkLoginExtensionGrantValidator>();
            identityServerBuilder.AddExtensionGrantValidator<ImpersonationExtensionGrantValidator>();
            identityServerBuilder.AddExtensionGrantValidator<SpaExternalLoginExtensionGrantValidator>();
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpAccountPublicWebIdentityServerModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AbpIdentityServerResource>()
                .AddVirtualJson("/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("X.Abp.IdentityServer", typeof(AbpIdentityServerResource));
        });

        Configure<IdentityServerOptions>(options =>
        {
            options.UserInteraction.ConsentUrl = "/Account/Consent";
            options.UserInteraction.ErrorUrl = "/Account/Error";
        });

        Configure<AbpRequestLocalizationOptions>(options =>
        {
            options.RequestLocalizationOptionConfigurators.Add((serviceProvider, localizationOptions) =>
            {
                localizationOptions.RequestCultureProviders.InsertAfter(
                    x => x.GetType() == typeof(QueryStringRequestCultureProvider),
                    new IdentityServerReturnUrlRequestCultureProvider());
                return Task.CompletedTask;
            });
        });

        Configure<AbpClaimsServiceOptions>(options =>
        {
            options.RequestedClaims.Add(AbpClaimTypes.SessionId);
            options.RequestedClaims.Add(ExtraClaimTypes.ProviderKey);
        });

        context.Services
            .AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        context.Services.Replace(
           ServiceDescriptor.Transient<IAuthenticationService, IdentitySessionAuthenticationService>());

        context.Services.AddHttpClient(ExternalLoginConsts.ExternalLoginHttpClientName,
            options =>
            {
                options.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                options.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            })

        // .AddTransientHttpErrorPolicy(
        //        p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)
        //    )
        // )
        .SetHandlerLifetime(TimeSpan.FromMinutes(5));
    }
}
