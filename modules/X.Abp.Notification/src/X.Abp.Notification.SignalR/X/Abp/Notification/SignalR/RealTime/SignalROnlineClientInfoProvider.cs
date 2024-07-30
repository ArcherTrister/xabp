// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Security.Principal;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using Volo.Abp.AspNetCore.WebClientInfo;
using Volo.Abp.Security.Claims;

using X.Abp.Notification.RealTime;

namespace X.Abp.Notification.SignalR.RealTime
{
    public class SignalROnlineClientInfoProvider : ISignalROnlineClientInfoProvider
    {
        protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

        protected IWebClientInfoProvider ClientInfoProvider { get; }

        protected ILogger Logger { get; }

        public SignalROnlineClientInfoProvider(
            ICurrentPrincipalAccessor currentPrincipalAccessor,
            IWebClientInfoProvider clientInfoProvider,
            ILogger<SignalROnlineClientInfoProvider> logger)
        {
            CurrentPrincipalAccessor = currentPrincipalAccessor;
            ClientInfoProvider = clientInfoProvider;
            Logger = logger;
        }

        public IOnlineClient CreateClientForCurrentConnection(HubCallerContext context)
        {
            return new OnlineClient(
                context.ConnectionId,
                GetIpAddressOfClient(context),
                CurrentPrincipalAccessor.Principal.FindTenantId(),
                CurrentPrincipalAccessor.Principal.FindUserId());
        }

        private string GetIpAddressOfClient(HubCallerContext context)
        {
            try
            {
                return ClientInfoProvider.ClientIpAddress;
            }
            catch (Exception ex)
            {
                Logger.LogError("Can not find IP address of the client! connectionId: " + context.ConnectionId);
                Logger.LogError(ex.Message, ex);
                return null;
            }
        }
    }
}
