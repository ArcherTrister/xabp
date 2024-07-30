// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Volo.Abp.AspNetCore.SignalR;

using X.Abp.Notification.RealTime;

namespace X.Abp.Notification.SignalR.RealTime
{
    /// <summary>
    /// Notification Hub of ABP.
    /// </summary>
    [Authorize]
    public class NotificationHub : AbpHub
    {
        protected IOnlineClientManager OnlineClientManager { get; }

        protected ISignalROnlineClientInfoProvider SignalROnlineClientInfoProvider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHub"/> class.
        /// </summary>
        public NotificationHub(
            IOnlineClientManager onlineClientManager,
            ISignalROnlineClientInfoProvider signalROnlineClientInfoProvider)
        {
            OnlineClientManager = onlineClientManager;
            SignalROnlineClientInfoProvider = signalROnlineClientInfoProvider;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var client = SignalROnlineClientInfoProvider.CreateClientForCurrentConnection(Context);

            Logger.LogDebug("A client is connected: " + client);

            await OnlineClientManager.AddAsync(client);
        }

        /*
        public override async Task OnReconnected()
        {
            await base.OnReconnected();

            var client = await _onlineClientManager.GetByConnectionIdOrNullAsync(Context.ConnectionId);
            if (client == null)
            {
                client = _onlineClientInfoProvider.CreateClientForCurrentConnection(Context);
                await _onlineClientManager.AddAsync(client);
                Logger.LogDebug("A client is connected (on reconnected event): " + client);
            }
            else
            {
                Logger.LogDebug("A client is reconnected: " + client);
            }
        }
        */

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);

            Logger.LogDebug("A client is disconnected: " + Context.ConnectionId);

            try
            {
                await OnlineClientManager.RemoveAsync(Context.ConnectionId);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString(), ex);
            }
        }
    }
}
