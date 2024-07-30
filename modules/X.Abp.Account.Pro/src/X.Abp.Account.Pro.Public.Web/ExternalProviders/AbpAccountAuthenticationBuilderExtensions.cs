// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp;

using X.Abp.Account.ExternalProviders;

namespace X.Abp.Account.Public.Web.ExternalProviders;

public static class AbpAccountAuthenticationBuilderExtensions
{
    public static AuthenticationBuilder WithDynamicOptions<TOptions, THandler>(
        [NotNull] this AuthenticationBuilder authenticationBuilder,
        [NotNull] string authenticationSchema,
        [NotNull] Action<ExternalProviderDefinitionBuilder<TOptions>> buildAction)
        where TOptions : RemoteAuthenticationOptions, new()
        where THandler : RemoteAuthenticationHandler<TOptions>
    {
        Check.NotNull(authenticationBuilder, nameof(authenticationBuilder));
        Check.NotNullOrWhiteSpace(authenticationSchema, nameof(authenticationSchema));
        Check.NotNull(buildAction, nameof(buildAction));

        authenticationBuilder.AddAbpAccountDynamicOptions<TOptions, THandler>();

        authenticationBuilder.Services.AddDynamicExternalLoginProviderOptions(
            authenticationSchema,
            buildAction);

        return authenticationBuilder;
    }
}
