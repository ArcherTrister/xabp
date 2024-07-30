// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RequestLocalization;

using OpenIddict.Server;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account.Localization;
using X.Abp.Account.Public.Web.ExtensionGrants;
using X.Abp.Account.Public.Web.Pages.Account;

namespace X.Abp.Account.Public.Web;

[DependsOn(
    typeof(AbpAccountPublicWebModule),
    typeof(AbpOpenIddictAspNetCoreModule))]
public class AbpAccountPublicWebOpenIddictModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>((options) =>
        {
            options.AddAssemblyResource(
                typeof(AccountResource),
                typeof(AbpAccountPublicApplicationContractsModule).Assembly,
                typeof(AbpAccountPublicWebOpenIddictModule).Assembly);
        });

        PreConfigure<IMvcBuilder>((mvcBuilder) =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpAccountPublicWebOpenIddictModule).Assembly);
        });

        PreConfigure<OpenIddictServerBuilder>((builder) =>
        {
            builder.AddEventHandler(OpenIddictImpersonateInferEndpointType.Descriptor);
            builder.AddEventHandler(LinkLoginExtensionGrantProcessJsonResponse.Descriptor);
            builder.Configure((openIddictServerOptions) =>
            {
                openIddictServerOptions.GrantTypes.Add(LinkLoginExtensionGrant.ExtensionGrantName);
                openIddictServerOptions.GrantTypes.Add(ImpersonationExtensionGrant.ExtensionGrantName);
            });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>((options) =>
        {
            options.FileSets.AddEmbedded<AbpAccountPublicWebOpenIddictModule>();
        });

        Configure<AbpOpenIddictExtensionGrantsOptions>((options) =>
        {
            options.Grants.Add(LinkLoginExtensionGrant.ExtensionGrantName, new LinkLoginExtensionGrant());
            options.Grants.Add(ImpersonationExtensionGrant.ExtensionGrantName, new ImpersonationExtensionGrant());
        });

        Configure<AbpRequestLocalizationOptions>((options) =>
        {
            options.RequestLocalizationOptionConfigurators.Add((serviceProvider, localizationOptions) =>
            {
                localizationOptions.RequestCultureProviders.InsertAfter(x => x.GetType() == typeof(QueryStringRequestCultureProvider), new OpenIddictReturnUrlRequestCultureProvider());
                return Task.CompletedTask;
            });
        });

        Configure<OpenIddictServerOptions>((options) =>
        {
            options.AuthorizationEndpointUris.Add(new Uri("/Account/ImpersonateTenant", UriKind.RelativeOrAbsolute));
            options.AuthorizationEndpointUris.Add(new Uri("/Account/ImpersonateUser", UriKind.RelativeOrAbsolute));
            options.AuthorizationEndpointUris.Add(new Uri("/Account/BackToImpersonator", UriKind.RelativeOrAbsolute));
        });

        Configure<AbpOpenIddictClaimsPrincipalOptions>((options) =>
        {
            options.ClaimsPrincipalHandlers.Add<OpenIddictImpersonateClaimsPrincipalHandler>();
        });
    }
}
