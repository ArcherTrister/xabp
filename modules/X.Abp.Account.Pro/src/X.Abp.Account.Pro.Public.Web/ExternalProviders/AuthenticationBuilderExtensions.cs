// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace X.Abp.Account.Public.Web.ExternalProviders;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddAbpAccountDynamicOptions<TOptions, THandler>(this AuthenticationBuilder authenticationBuilder)
        where TOptions : RemoteAuthenticationOptions, new()
        where THandler : RemoteAuthenticationHandler<TOptions>
    {
        authenticationBuilder.Services.AddScoped(typeof(AccountExternalProviderOptionsManager<TOptions>));

        authenticationBuilder.Services.Replace(ServiceDescriptor.Scoped(typeof(IOptions<TOptions>),
            provider => provider.GetRequiredService<AccountExternalProviderOptionsManager<TOptions>>()));
        authenticationBuilder.Services.Replace(ServiceDescriptor.Scoped(typeof(IOptionsSnapshot<TOptions>),
            provider => provider.GetRequiredService<AccountExternalProviderOptionsManager<TOptions>>()));
        authenticationBuilder.Services.Replace(ServiceDescriptor.Scoped(typeof(IOptionsMonitor<TOptions>),
            provider => provider.GetRequiredService<AccountExternalProviderOptionsManager<TOptions>>()));

        authenticationBuilder.Services.Replace(ServiceDescriptor.Transient<IOptionsFactory<TOptions>, AccountExternalProviderOptionsFactory<TOptions>>());

        var handler = authenticationBuilder.Services.LastOrDefault(x => x.ServiceType == typeof(THandler));
        authenticationBuilder.Services.Replace(new ServiceDescriptor(
            typeof(THandler),
            provider => new AbpAccountAuthenticationRequestHandler<TOptions, THandler>(
                (THandler)ActivatorUtilities.CreateInstance(provider, typeof(THandler)),
                provider.GetRequiredService<IOptions<TOptions>>()),
            handler?.Lifetime ?? ServiceLifetime.Transient));

        return authenticationBuilder;
    }
}
