// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.SignalR;

using Volo.Abp.DependencyInjection;

using X.Abp.Notification.RealTime;

namespace X.Abp.Notification.SignalR.RealTime
{
    public interface ISignalROnlineClientInfoProvider : ITransientDependency
    {
        IOnlineClient CreateClientForCurrentConnection(HubCallerContext context);
    }
}
