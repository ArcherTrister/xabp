// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Volo.Abp;

namespace X.Abp.Account.Public.Web.ExternalProviders;

public class AbpAccountAuthenticationRequestHandler<TOptions, THandler> : IAuthenticationRequestHandler
    where TOptions : RemoteAuthenticationOptions, new()
    where THandler : RemoteAuthenticationHandler<TOptions>
{
    protected THandler InnerHandler { get; }

    protected IOptions<TOptions> OptionsManager { get; }

    public AbpAccountAuthenticationRequestHandler(THandler innerHandler, IOptions<TOptions> optionsManager)
    {
        InnerHandler = innerHandler;
        OptionsManager = optionsManager;
    }

    public virtual async Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        await InnerHandler.InitializeAsync(scheme, context);
    }

    public virtual async Task<AuthenticateResult> AuthenticateAsync()
    {
        return await InnerHandler.AuthenticateAsync();
    }

    public virtual async Task ChallengeAsync(AuthenticationProperties properties)
    {
        await OptionsManager.SetAsync(InnerHandler.Scheme.Name);
        ObjectHelper.TrySetProperty(InnerHandler, handler => handler.Options, () => OptionsManager.Value);

        await InnerHandler.ChallengeAsync(properties);
    }

    public virtual async Task ForbidAsync(AuthenticationProperties properties)
    {
        await InnerHandler.ForbidAsync(properties);
    }

    public virtual async Task<bool> HandleRequestAsync()
    {
        if (await InnerHandler.ShouldHandleRequestAsync())
        {
            await OptionsManager.SetAsync(InnerHandler.Scheme.Name);
            ObjectHelper.TrySetProperty(InnerHandler, handler => handler.Options, () => OptionsManager.Value);
        }

        return await InnerHandler.HandleRequestAsync();
    }

    public virtual THandler GetHandler()
    {
        return InnerHandler;
    }
}
