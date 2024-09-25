// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Claims;
using System.Security.Principal;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Abp.Users;

using X.Abp.Account.Localization;
using X.Abp.Account.Public.Web;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.ExtensionGrants
{
    public class ImpersonationExtensionGrant : TokenExtensionGrantBase
    {
        public const string ExtensionGrantName = "Impersonation";

        public override string Name => "Impersonation";

        protected IPermissionChecker PermissionChecker { get; set; }

        protected ICurrentTenant CurrentTenant { get; set; }

        protected ICurrentUser CurrentUser { get; set; }

        protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; set; }

        protected IdentityUserManager UserManager { get; set; }

        protected IdentitySecurityLogManager IdentitySecurityLogManager { get; set; }

        protected IdentityUserDelegationManager IdentityUserDelegationManager { get; set; }

        protected ILogger<ImpersonationExtensionGrant> Logger { get; set; }

        protected AbpAccountOptions AbpAccountOptions { get; set; }

        protected IUserClaimsPrincipalFactory<IdentityUser> UserClaimsPrincipalFactory { get; set; }

        protected IStringLocalizer<AccountResource> Localizer { get; set; }

        [UnitOfWork]
        public override async Task<IActionResult> HandleAsync(ExtensionGrantContext context)
        {
            using (CultureHelper.Use("en"))
            {
                PermissionChecker = context.HttpContext.RequestServices.GetRequiredService<IPermissionChecker>();
                CurrentTenant = context.HttpContext.RequestServices.GetRequiredService<ICurrentTenant>();
                CurrentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
                CurrentPrincipalAccessor = context.HttpContext.RequestServices.GetRequiredService<ICurrentPrincipalAccessor>();
                UserManager = context.HttpContext.RequestServices.GetRequiredService<IdentityUserManager>();
                IdentitySecurityLogManager = context.HttpContext.RequestServices.GetRequiredService<IdentitySecurityLogManager>();
                IdentityUserDelegationManager = context.HttpContext.RequestServices.GetRequiredService<IdentityUserDelegationManager>();
                Logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ImpersonationExtensionGrant>>();
                AbpAccountOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AbpAccountOptions>>().Value;
                UserClaimsPrincipalFactory = context.HttpContext.RequestServices.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>();
                Localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<AccountResource>>();

                OpenIddictServerTransaction transaction = await context.HttpContext.RequestServices.GetRequiredService<IOpenIddictServerFactory>().CreateTransactionAsync();
                transaction.EndpointType = OpenIddictServerEndpointType.Introspection;
                transaction.Request = new OpenIddictRequest()
                {
                    ClientId = context.Request.ClientId,
                    ClientSecret = context.Request.ClientSecret,
                    Token = context.Request.AccessToken
                };
                OpenIddictServerEvents.ProcessAuthenticationContext authorizationRequestContext = new OpenIddictServerEvents.ProcessAuthenticationContext(transaction);
                await context.HttpContext.RequestServices.GetRequiredService<IOpenIddictServerDispatcher>().DispatchAsync(authorizationRequestContext);
                if (authorizationRequestContext.IsRejected)
                {
                    Logger.LogError("Process authentication rejected");
                    return new ForbidResult(new string[1]
                    {
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    },
                    new AuthenticationProperties(new Dictionary<string, string>()
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = authorizationRequestContext.Error ?? OpenIddictConstants.Errors.InvalidRequest,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = authorizationRequestContext.ErrorDescription,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorUri] = authorizationRequestContext.ErrorUri
                    },
                    new Dictionary<string, object>()
                    {
                        [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                    }));
                }

                ClaimsPrincipal principal = authorizationRequestContext.GenericTokenPrincipal;
                if (principal != null)
                {
                    OpenIddictRequest request = context.HttpContext.GetOpenIddictServerRequest();
                    Check.NotNull(request, nameof(request));
                    using (CurrentPrincipalAccessor.Change(principal.Claims))
                    {
                        Guid? tenantId = await GetRawValueOrNullAsync(request, "TenantId");
                        Guid? userId = await GetRawValueOrNullAsync(request, "UserId");
                        Guid? userDelegationId = await GetRawValueOrNullAsync(request, "UserDelegationId");
                        Guid? impersonatorUserId = CurrentPrincipalAccessor.Principal.FindImpersonatorUserId();
                        if (!userId.HasValue && !tenantId.HasValue && !userDelegationId.HasValue && impersonatorUserId.HasValue)
                        {
                            return await BackToImpersonatorAsync(context, principal, CurrentPrincipalAccessor.Principal.FindImpersonatorTenantId(), impersonatorUserId.Value);
                        }

                        if (impersonatorUserId.HasValue)
                        {
                            return new ForbidResult(new string[1]
                            {
                                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                            },
                            new AuthenticationProperties(new Dictionary<string, string>()
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:NestedImpersonationIsNotAllowed"]
                            },
                            new Dictionary<string, object>()
                            {
                                [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                            }));
                        }

                        if (userDelegationId.HasValue)
                        {
                            return await DelegatedImpersonateAsync(context, principal, userDelegationId.Value);
                        }

                        if (CurrentTenant.IsAvailable)
                        {
                            if (userId.HasValue)
                            {
                                return await ImpersonateUserAsync(context, principal, CurrentTenant.Id, userId.Value);
                            }
                        }
                        else
                        {
                            if (!userId.HasValue && tenantId.HasValue)
                            {
                                return await ImpersonateTenantAsync(context, principal, tenantId.Value, request.GetParameter("TenantUserName").ToString());
                            }

                            if (userId.HasValue && !tenantId.HasValue)
                            {
                                return await ImpersonateUserAsync(context, principal, null, userId.Value);
                            }
                        }

                        return new ForbidResult(new string[1]
                        {
                            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                        },
                        new AuthenticationProperties(new Dictionary<string, string>()
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:InvalidTenantIdOrUserId"]
                        },
                        new Dictionary<string, object>()
                        {
                            [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                        }));
                    }
                }
                else
                {
                    Logger.LogError("Process authentication principal is null");
                    return new ForbidResult(new string[1]
                    {
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    },
                    new AuthenticationProperties(new Dictionary<string, string>()
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:InvalidAccessToken"]
                    },
                    new Dictionary<string, object>()
                    {
                        [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                    }));
                }
            }
        }

        protected virtual async Task<IActionResult> ImpersonateTenantAsync(
          ExtensionGrantContext context,
          ClaimsPrincipal principal,
          Guid tenantId,
          string tenantUserName)
        {
            bool flag = false;
            if (!AbpAccountOptions.ImpersonationTenantPermission.IsNullOrWhiteSpace())
            {
                flag = !await PermissionChecker.IsGrantedAsync(AbpAccountOptions.ImpersonationTenantPermission);
            }

            if (!flag)
            {
                using (CurrentTenant.Change(tenantId))
                {
                    tenantUserName = !tenantUserName.IsNullOrEmpty() ? tenantUserName : AbpAccountOptions.TenantAdminUserName;
                    IdentityUser user = await UserManager.FindByNameAsync(tenantUserName);
                    if (user == null)
                    {
                        Logger.LogError(Localizer["Volo.Account:ThereIsNoUserWithUserName"].ToString().Replace("{UserName}", tenantUserName));
                        return new ForbidResult(new string[1]
                        {
                            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                        },
                        new AuthenticationProperties(new Dictionary<string, string>()
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:ThereIsNoUserWithUserName"].ToString().Replace("{UserName}", tenantUserName)
                        },
                        new Dictionary<string, object>()
                        {
                            [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                        }));
                    }

                    ClaimsPrincipal claimsPrincipal = await UserClaimsPrincipalFactory.CreateAsync(user);
                    List<Claim> claimList = new List<Claim>()
                    {
                        new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()),
                        new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName)
                    };
                    Claim claim = principal.Claims.FirstOrDefault(x => x.Type == AbpClaimTypes.RememberMe);
                    if (claim != null)
                    {
                        claimList.Add(claim);
                    }

                    claimsPrincipal.Identities.First().AddClaims(claimList);
                    using (CurrentPrincipalAccessor.Change(claimsPrincipal))
                    {
                        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                        {
                            Identity = IdentitySecurityLogIdentityConsts.Identity,
                            Action = "ImpersonateUser"
                        });
                    }

                    await CreateSessionAsync(context, claimsPrincipal);
                    claimsPrincipal.SetScopes(principal.GetScopes());
                    claimsPrincipal.SetResources(await GetResourcesAsync(context, principal.GetScopes()));

                    await SetClaimsDestinationsAsync(context, claimsPrincipal);
                    return new Microsoft.AspNetCore.Mvc.SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal);
                }
            }
            else
            {
                Logger.LogError(Localizer["Volo.Account:RequirePermissionToImpersonateTenant"].ToString().Replace("{PermissionName}", AbpAccountOptions.ImpersonationTenantPermission));
                return new ForbidResult(new string[1]
                {
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                },
                new AuthenticationProperties(new Dictionary<string, string>()
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:RequirePermissionToImpersonateTenant"].ToString().Replace("{PermissionName}", AbpAccountOptions.ImpersonationTenantPermission)
                },
                new Dictionary<string, object>()
                {
                    [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                }));
            }
        }

        protected virtual async Task<IActionResult> ImpersonateUserAsync(
          ExtensionGrantContext context,
          ClaimsPrincipal principal,
          Guid? tenantId,
          Guid userId)
        {
            if (CurrentUser.Id.HasValue && (userId == CurrentUser.Id.GetValueOrDefault()))
            {
                Logger.LogError(Localizer["Volo.Account:YouCanNotImpersonateYourself"]);
                return new ForbidResult(new string[1]
                {
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                },
                new AuthenticationProperties(new Dictionary<string, string>()
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:YouCanNotImpersonateYourself"]
                },
                new Dictionary<string, object>()
                {
                    [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                }));
            }

            bool flag;
            if (!(flag = AbpAccountOptions.ImpersonationUserPermission.IsNullOrWhiteSpace()))
            {
                flag = !await PermissionChecker.IsGrantedAsync(AbpAccountOptions.ImpersonationUserPermission);
            }

            if (!flag)
            {
                using (CurrentTenant.Change(tenantId))
                {
                    IdentityUser user = await UserManager.FindByIdAsync(userId.ToString());
                    if (user == null)
                    {
                        Logger.LogError(Localizer["Volo.Account:ThereIsNoUserWithId"].ToString().Replace("{UserId}", userId.ToString()));
                        return new ForbidResult(new string[1]
                        {
                            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                        },
                        new AuthenticationProperties(new Dictionary<string, string>()
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:ThereIsNoUserWithId"].ToString().Replace("{UserId}", userId.ToString())
                        },
                        new Dictionary<string, object>()
                        {
                            [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                        }));
                    }

                    ClaimsPrincipal claimsPrincipal = await UserClaimsPrincipalFactory.CreateAsync(user);
                    List<Claim> additionalClaims = new List<Claim>();

                    if (CurrentUser.Id?.ToString() != CurrentUser.FindClaim(AbpClaimTypes.ImpersonatorUserId)?.Value)
                    {
                        additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()));
                        additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName));
                        if (CurrentTenant.IsAvailable)
                        {
                            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantId, CurrentTenant.Id.ToString()));
                            ITenantStore tenantStore = context.HttpContext.RequestServices.GetRequiredService<ITenantStore>();
                            TenantConfiguration tenantConfiguration = await tenantStore.FindAsync(CurrentTenant.Id.Value);
                            if (tenantConfiguration != null && !tenantConfiguration.Name.IsNullOrWhiteSpace())
                            {
                                additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantName, tenantConfiguration.Name));
                            }
                        }
                    }

                    Claim claim = principal.Claims.FirstOrDefault(x => x.Type == AbpClaimTypes.RememberMe);
                    if (claim != null)
                    {
                        additionalClaims.Add(claim);
                    }

                    claimsPrincipal.Identities.First().AddClaims(additionalClaims);
                    using (CurrentPrincipalAccessor.Change(claimsPrincipal))
                    {
                        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                        {
                            Identity = IdentitySecurityLogIdentityConsts.Identity,
                            Action = "ImpersonateUser"
                        });
                    }

                    await CreateSessionAsync(context, claimsPrincipal);
                    claimsPrincipal.SetScopes(principal.GetScopes());
                    claimsPrincipal.SetResources(await GetResourcesAsync(context, principal.GetScopes()));

                    await SetClaimsDestinationsAsync(context, claimsPrincipal);
                    return new Microsoft.AspNetCore.Mvc.SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal);
                }
            }
            else
            {
                Logger.LogError(Localizer["Volo.Account:RequirePermissionToImpersonateUser"].ToString().Replace("{PermissionName}", AbpAccountOptions.ImpersonationUserPermission));
                return new ForbidResult(new string[1]
                {
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                },
                new AuthenticationProperties(new Dictionary<string, string>()
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:RequirePermissionToImpersonateUser"].ToString().Replace("{PermissionName}", AbpAccountOptions.ImpersonationUserPermission)
                },
                new Dictionary<string, object>()
                {
                    [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                }));
            }
        }

        protected virtual async Task<IActionResult> DelegatedImpersonateAsync(
          ExtensionGrantContext context,
          ClaimsPrincipal principal,
          Guid userDelegationId)
        {
            IdentityUserDelegation userDelegation = await IdentityUserDelegationManager.FindActiveDelegationByIdAsync(userDelegationId);
            if (userDelegation != null)
            {
                if (!CurrentUser.Id.HasValue || userDelegation.TargetUserId == CurrentUser.Id.GetValueOrDefault())
                {
                    if (CurrentUser.Id.HasValue && (userDelegation.SourceUserId == CurrentUser.Id.GetValueOrDefault()))
                    {
                        Logger.LogError(Localizer["Volo.Account:YouCanNotImpersonateYourself"]);
                        return new ForbidResult(new string[1]
                        {
                            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                        },
                        new AuthenticationProperties(new Dictionary<string, string>()
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:YouCanNotImpersonateYourself"]
                        },
                        new Dictionary<string, object>()
                        {
                            [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                        }));
                    }

                    IdentityUser user = await UserManager.FindByIdAsync(userDelegation.SourceUserId.ToString());
                    if (user == null)
                    {
                        Logger.LogError(Localizer["Volo.Account:ThereIsNoUserWithId"].ToString().Replace("{UserId}", userDelegation.SourceUserId.ToString()));
                        return new ForbidResult(new string[1]
                        {
                            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                        },
                        new AuthenticationProperties(new Dictionary<string, string>()
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:ThereIsNoUserWithId"].ToString().Replace("{UserId}", userDelegation.SourceUserId.ToString())
                        },
                        new Dictionary<string, object>()
                        {
                            [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                        }));
                    }

                    ClaimsPrincipal claimsPrincipal = await UserClaimsPrincipalFactory.CreateAsync(user);
                    List<Claim> claimList = new List<Claim>();

                    if (CurrentUser.Id?.ToString() != CurrentUser.FindClaim(AbpClaimTypes.ImpersonatorUserId)?.Value)
                    {
                        claimList.Add(new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()));
                        claimList.Add(new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName));
                        if (CurrentTenant.IsAvailable)
                        {
                            claimList.Add(new Claim(AbpClaimTypes.ImpersonatorTenantId, CurrentTenant.Id.ToString()));
                        }
                    }

                    Claim claim = principal.Claims.FirstOrDefault(x => x.Type == AbpClaimTypes.RememberMe);
                    if (claim != null)
                    {
                        claimList.Add(claim);
                    }

                    claimsPrincipal.Identities.First().AddClaims(claimList);
                    using (CurrentPrincipalAccessor.Change(claimsPrincipal))
                    {
                        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                        {
                            Identity = IdentitySecurityLogIdentityConsts.Identity,
                            Action = "DelegatedImpersonate"
                        });
                    }

                    await CreateSessionAsync(context, claimsPrincipal);
                    claimsPrincipal.SetScopes(principal.GetScopes());
                    claimsPrincipal.SetResources(await GetResourcesAsync(context, principal.GetScopes()));

                    await SetClaimsDestinationsAsync(context, claimsPrincipal);
                    return new Microsoft.AspNetCore.Mvc.SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal);
                }
            }

            return new ForbidResult(new string[1]
            {
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
            },
            new AuthenticationProperties(new Dictionary<string, string>()
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:InvalidUserDelegationId"]
            },
            new Dictionary<string, object>()
            {
                [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
            }));
        }

        protected virtual async Task<IActionResult> BackToImpersonatorAsync(
          ExtensionGrantContext context,
          ClaimsPrincipal principal,
          Guid? tenantId,
          Guid userId)
        {
            using (CurrentTenant.Change(tenantId))
            {
                IdentityUser user = await UserManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return new ForbidResult(new string[1]
                    {
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    },
                    new AuthenticationProperties(new Dictionary<string, string>()
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = Localizer["Volo.Account:InvalidAccessToken"]
                    },
                    new Dictionary<string, object>()
                    {
                        [OpenIddictConstants.Parameters.GrantType] = context.Request.GrantType
                    }));
                }

                ClaimsPrincipal claimsPrincipal = await UserClaimsPrincipalFactory.CreateAsync(user);
                await CreateSessionAsync(context, claimsPrincipal);
                claimsPrincipal.SetScopes(principal.GetScopes());
                claimsPrincipal.SetResources(await GetResourcesAsync(context, principal.GetScopes()));

                await SetClaimsDestinationsAsync(context, claimsPrincipal);
                return new Microsoft.AspNetCore.Mvc.SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal);
            }
        }
    }
}
