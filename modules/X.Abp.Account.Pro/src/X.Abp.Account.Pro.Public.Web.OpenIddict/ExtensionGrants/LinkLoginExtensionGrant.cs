// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;
using OpenIddict.Server;

using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Abp.Users;

using X.Abp.Account.Localization;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.ExtensionGrants;

public class LinkLoginExtensionGrant : TokenExtensionGrantBase
{
    public const string ExtensionGrantName = "LinkLogin";

    public const string TenantDomainParameterName = "tenant_domain";

    public override string Name => ExtensionGrantName;

    protected IdentityLinkUserManager IdentityLinkUserManager { get; set; }

    protected ICurrentTenant CurrentTenant { get; set; }

    protected ICurrentUser CurrentUser { get; set; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; set; }

    protected IdentityUserManager UserManager { get; set; }

    protected IUserClaimsPrincipalFactory<IdentityUser> UserClaimsPrincipalFactory { get; set; }

    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; set; }

    protected ILogger<LinkLoginExtensionGrant> Logger { get; set; }

    protected IStringLocalizer<AccountResource> Localizer { get; set; }

    protected AbpAccountOpenIddictOptions AccountOpenIddictOptions { get; set; }

    protected ITenantStore TenantStore { get; set; }

    [UnitOfWork]
    public override async Task<IActionResult> HandleAsync(ExtensionGrantContext context)
    {
        IdentityLinkUserManager = context.HttpContext.RequestServices.GetRequiredService<IdentityLinkUserManager>();
        CurrentTenant = context.HttpContext.RequestServices.GetRequiredService<ICurrentTenant>();
        CurrentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
        CurrentPrincipalAccessor = context.HttpContext.RequestServices.GetRequiredService<ICurrentPrincipalAccessor>();
        UserManager = context.HttpContext.RequestServices.GetRequiredService<IdentityUserManager>();
        UserClaimsPrincipalFactory = context.HttpContext.RequestServices.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>();
        IdentitySecurityLogManager = context.HttpContext.RequestServices.GetRequiredService<IdentitySecurityLogManager>();
        Logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<LinkLoginExtensionGrant>>();
        Localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<AccountResource>>();
        AccountOpenIddictOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AbpAccountOpenIddictOptions>>().Value;
        TenantStore = context.HttpContext.RequestServices.GetRequiredService<ITenantStore>();
        var transaction = await context.HttpContext.RequestServices.GetRequiredService<IOpenIddictServerFactory>().CreateTransactionAsync();
        transaction.EndpointType = OpenIddictServerEndpointType.Introspection;
        transaction.Request = new OpenIddictRequest()
        {
            ClientId = context.Request.ClientId,
            ClientSecret = context.Request.ClientSecret,
            Token = context.Request.AccessToken
        };

        var notification = new OpenIddictServerEvents.ProcessAuthenticationContext(transaction);
        await context.HttpContext.RequestServices.GetRequiredService<IOpenIddictServerDispatcher>().DispatchAsync(notification);
        if (notification.IsRejected)
        {
            Logger.LogError("Process authentication rejected");
            return new ForbidResult(new string[1]
            {
                "OpenIddict.Server.AspNetCore"
            },
            new AuthenticationProperties(new Dictionary<string, string>()
            {
                [".error"] = notification.Error ?? "invalid_request",
                [".error_description"] = notification.ErrorDescription,
                [".error_uri"] = notification.ErrorUri
            }));
        }

        var principal = notification.GenericTokenPrincipal;
        if (principal != null)
        {
            var request = context.HttpContext.GetOpenIddictServerRequest();
            using (CurrentPrincipalAccessorExtensions.Change(CurrentPrincipalAccessor, principal.Claims))
            {
                var linkUserId = await GetRawValueOrNullAsync(request, "LinkUserId");
                if (!linkUserId.HasValue)
                {
                    Logger.LogError("Invalid link user id");
                    return new ForbidResult(new string[]
                    {
                        "OpenIddict.Server.AspNetCore"
                    },
                    new AuthenticationProperties(new Dictionary<string, string>()
                    {
                        [".error"] = "invalid_request",
                        [".error_description"] = "Invalid link user id"
                    }));
                }

                var linkTenantId = await GetRawValueOrNullAsync(request, "LinkTenantId");
                if (!linkTenantId.HasValue && request.HasParameter("LinkTenantId"))
                {
                    Logger.LogError("Invalid link tenant id");
                    return new ForbidResult(new string[]
                    {
                        "OpenIddict.Server.AspNetCore"
                    },
                    new AuthenticationProperties(new Dictionary<string, string>()
                    {
                        [".error"] = "invalid_request",
                        [".error_description"] = "Invalid link tenant id"
                    }));
                }

                if (await IdentityLinkUserManager.IsLinkedAsync(new IdentityLinkUserInfo(CurrentUser.GetId(), CurrentTenant.Id), new IdentityLinkUserInfo(linkUserId.Value, linkTenantId), true))
                {
                    using (CurrentTenant.Change(linkTenantId, null))
                    {
                        var user = await UserManager.GetByIdAsync(linkUserId.Value);
                        var claimsPrincipal = await UserClaimsPrincipalFactory.CreateAsync(user);
                        var authenticationProperties = new AuthenticationProperties();
                        if (AccountOpenIddictOptions.IsTenantMultiDomain)
                        {
                            var basicTenantInfo = new BasicTenantInfo(null, null);
                            if (linkTenantId.HasValue)
                            {
                                var tenantConfiguration = await TenantStore.FindAsync(linkTenantId.Value);
                                basicTenantInfo = new BasicTenantInfo(tenantConfiguration.Id, tenantConfiguration.Name);
                            }

                            authenticationProperties.SetParameter(TenantDomainParameterName, await AccountOpenIddictOptions.GetTenantDomain(context.HttpContext, basicTenantInfo));
                        }

                        claimsPrincipal.SetScopes(principal.GetScopes());
                        claimsPrincipal.SetResources(await GetResourcesAsync(context, principal.GetScopes()));

                        await SetClaimsDestinationsAsync(context, claimsPrincipal);
                        return new Microsoft.AspNetCore.Mvc.SignInResult("OpenIddict.Server.AspNetCore", claimsPrincipal, authenticationProperties);
                    }
                }
                else
                {
                    Logger.LogError("The target user is not linked to you!");
                    return new ForbidResult(new string[]
                    {
                        "OpenIddict.Server.AspNetCore"
                    },
                    new AuthenticationProperties(new Dictionary<string, string>()
                    {
                        [".error"] = "invalid_request",
                        [".error_description"] = "The target user is not linked to you!"
                    }));
                }
            }
        }
        else
        {
            Logger.LogError("Process authentication principal is null");
            return new ForbidResult(new string[]
            {
                "OpenIddict.Server.AspNetCore"
            },
            new AuthenticationProperties(new Dictionary<string, string>()
            {
                [".error"] = "invalid_token"
            }));
        }
    }
}
