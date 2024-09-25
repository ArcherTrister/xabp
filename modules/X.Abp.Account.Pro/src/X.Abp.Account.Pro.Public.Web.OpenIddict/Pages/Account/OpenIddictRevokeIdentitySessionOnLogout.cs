// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Claims;
using System.Security.Principal;

using OpenIddict.Server;

using X.Abp.Identity;

namespace X.Abp.Account.Web.Pages.Account
{
    public class OpenIddictRevokeIdentitySessionOnLogout :
      IOpenIddictServerHandler<OpenIddictServerEvents.HandleLogoutRequestContext>
    {
        public static OpenIddictServerHandlerDescriptor Descriptor { get; } = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.HandleLogoutRequestContext>().UseSingletonHandler<OpenIddictRevokeIdentitySessionOnLogout>().SetOrder(OpenIddictServerHandlers.Session.AttachPrincipal.Descriptor.Order + 1000).SetType(OpenIddictServerHandlerType.BuiltIn).Build();

        protected IdentitySessionManager IdentitySessionManager { get; }

        public OpenIddictRevokeIdentitySessionOnLogout(IdentitySessionManager identitySessionManager) => IdentitySessionManager = identitySessionManager;

        public virtual async ValueTask HandleAsync(OpenIddictServerEvents.HandleLogoutRequestContext context)
        {
            ClaimsPrincipal principal = context != null ? context.IdentityTokenHintPrincipal : throw new ArgumentNullException(nameof(context));
            string sessionId = principal != null ? principal.FindSessionId() : null;
            if (!sessionId.IsNullOrWhiteSpace())
            {
                context.Logger.LogInformation("Revoking the sessionid: " + sessionId + ".");
                await IdentitySessionManager.RevokeAsync(sessionId);
            }
            else
            {
                context.Logger.LogWarning("No SessionId was found in the token.");
            }
        }
    }
}
