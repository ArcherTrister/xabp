using System;
using System.Threading.Tasks;

using IdentityServer4.Events;
using IdentityServer4.Services;

namespace X.Abp.Account.Web.Services;

public class CustomEventService : IEventService
{
    public bool CanRaiseEventType(EventTypes evtType)
    {
        throw new NotImplementedException();
    }

    public Task RaiseAsync(Event evt)
    {
        if (evt is UserLoginSuccessEvent userLoginSuccessEvent)
        {
            // 在这里处理 UserLoginSuccessEvent
            HandleUserLoginSuccess(userLoginSuccessEvent);
        }

        // 对于其他类型的事件，你可以在这里添加更多的条件分支

        return Task.CompletedTask;
    }

    private void HandleUserLoginSuccess(UserLoginSuccessEvent evt)
    {
        // 自定义处理逻辑，例如记录日志
        Console.WriteLine($"用户 {evt.Username} 登录成功！");
    }
}
