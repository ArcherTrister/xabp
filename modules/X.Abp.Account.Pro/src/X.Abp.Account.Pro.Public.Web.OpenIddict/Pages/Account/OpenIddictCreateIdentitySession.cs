// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Principal;

using Microsoft.Extensions.Options;

using OpenIddict.Server;

using Volo.Abp.AspNetCore.WebClientInfo;

using X.Abp.Identity;

namespace X.Abp.Account.Web.Pages.Account
{
    [Obsolete("Use IdentitySessionAuthenticationService")]
    public class OpenIddictCreateIdentitySession :
      IOpenIddictServerHandler<OpenIddictServerEvents.ProcessSignInContext>
    {
        public static OpenIddictServerHandlerDescriptor Descriptor { get; } = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ProcessSignInContext>().UseSingletonHandler<OpenIddictCreateIdentitySession>().SetOrder(100000).SetType(OpenIddictServerHandlerType.Custom).Build();

        protected IdentitySessionManager IdentitySessionManager { get; }

        protected IWebClientInfoProvider WebClientInfoProvider { get; }

        protected IOptions<AbpAccountOpenIddictOptions> Options { get; }

        public OpenIddictCreateIdentitySession(
          IdentitySessionManager identitySessionManager,
          IWebClientInfoProvider webClientInfoProvider,
          IOptions<AbpAccountOpenIddictOptions> options)
        {
            IdentitySessionManager = identitySessionManager;
            WebClientInfoProvider = webClientInfoProvider;
            Options = options;
        }

        public async ValueTask HandleAsync(OpenIddictServerEvents.ProcessSignInContext context)
        {
            if (context == null)
            {
                ArgumentNullException.ThrowIfNull(context, nameof(context));
            }

            if (context.IsRequestHandled || context.IsRequestSkipped || context.IsRejected || context.Principal == null)
            {
                return;
            }

            string sessionId = context.Principal.FindSessionId();
            if (sessionId.IsNullOrWhiteSpace())
            {
                context.Logger.LogError("SessionId is null. It's not possible to save the session.");
            }
            else
            {
                if (context.ClientId.IsNullOrWhiteSpace() || !Options.Value.ClientIdToDeviceMap.TryGetValue(context.ClientId, out string device))
                {
                    device = "OAuth";
                }

                await IdentitySessionManager.CreateAsync(sessionId, device, WebClientInfoProvider.DeviceInfo, context.Principal.FindUserId().Value, context.Principal.FindTenantId(), context.ClientId, WebClientInfoProvider.ClientIpAddress);
            }
        }
    }
}
