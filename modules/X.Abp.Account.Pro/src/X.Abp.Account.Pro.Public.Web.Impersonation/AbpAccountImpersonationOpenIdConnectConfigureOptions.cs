// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace X.Abp.Account.Public.Web.Impersonation;

public class AbpAccountImpersonationOpenIdConnectConfigureOptions : IPostConfigureOptions<OpenIdConnectOptions>
{
    public void PostConfigure(string name, OpenIdConnectOptions options)
    {
        options.Events ??= new OpenIdConnectEvents();

        var previousOnRedirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
        options.Events.OnRedirectToIdentityProvider = async redirectContext =>
        {
            if (redirectContext.Properties.Items.TryGetValue(AbpAccountImpersonationConsts.Impersonation, out string _))
            {
                redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.AccessToken, await redirectContext.HttpContext.GetTokenAsync(AbpAccountImpersonationConsts.AccessToken));
                if (redirectContext.Properties.Items.TryGetValue(AbpAccountImpersonationConsts.UserId, out string userId))
                {
                    redirectContext.ProtocolMessage.IssuerAddress = options.Authority.EnsureEndsWith('/') + AbpAccountImpersonationConsts.ImpersonateUserEndpoint;
                    redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.UserId, userId);
                }
                else if (redirectContext.Properties.Items.TryGetValue(AbpAccountImpersonationConsts.TenantId, out string tenantId))
                {
                    redirectContext.ProtocolMessage.IssuerAddress = options.Authority.EnsureEndsWith('/') + AbpAccountImpersonationConsts.ImpersonateTenantEndpoint;
                    redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.TenantId, tenantId);
                    redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.TenantUserName, redirectContext.Properties.Items[AbpAccountImpersonationConsts.TenantUserName]);
                    redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.ReturnUrl, redirectContext.Properties.Items[AbpAccountImpersonationConsts.TenantUserName]);
                }
            }
            else if (redirectContext.Properties.Items.TryGetValue(AbpAccountImpersonationConsts.BackToImpersonator, out var _))
            {
                redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.AccessToken, await redirectContext.HttpContext.GetTokenAsync(AbpAccountImpersonationConsts.AccessToken));
                redirectContext.ProtocolMessage.IssuerAddress = options.Authority.EnsureEndsWith('/') + AbpAccountImpersonationConsts.BackToImpersonatorEndpoint;
            }
            else if (redirectContext.Properties.Items.TryGetValue(AbpAccountImpersonationConsts.DelegatedImpersonate, out var _))
            {
                redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.AccessToken, await redirectContext.HttpContext.GetTokenAsync(AbpAccountImpersonationConsts.AccessToken));
                redirectContext.ProtocolMessage.IssuerAddress = options.Authority.EnsureEndsWith('/', StringComparison.Ordinal) + AbpAccountImpersonationConsts.DelegatedImpersonateEndpoint;
                redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.UserDelegationId, redirectContext.Properties.Items[AbpAccountImpersonationConsts.UserDelegationId]);
            }

            if (previousOnRedirectToIdentityProvider != null)
            {
                await previousOnRedirectToIdentityProvider(redirectContext);
            }
        };
    }
}
