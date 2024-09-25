// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Principal;

using OpenIddict.Server;

using X.Abp.Identity;

namespace X.Abp.Account.Web.Pages.Account
{
    public class OpenIddictRevokeIdentitySessionOnRevocation :
      IOpenIddictServerHandler<OpenIddictServerEvents.HandleRevocationRequestContext>
    {
        public static OpenIddictServerHandlerDescriptor Descriptor { get; } = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.HandleRevocationRequestContext>().UseSingletonHandler<OpenIddictRevokeIdentitySessionOnRevocation>().SetOrder(OpenIddictServerHandlers.Revocation.RevokeToken.Descriptor.Order + 1000).SetType(OpenIddictServerHandlerType.Custom).Build();

        protected IdentitySessionManager IdentitySessionManager { get; }

        public OpenIddictRevokeIdentitySessionOnRevocation(IdentitySessionManager identitySessionManager)
        {
            IdentitySessionManager = identitySessionManager;
        }

        public virtual async ValueTask HandleAsync(OpenIddictServerEvents.HandleRevocationRequestContext context)
        {
            string sessionId = context != null ? context.Principal.FindSessionId() : throw new ArgumentNullException(nameof(context));
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
