// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using JetBrains.Annotations;

using Volo.Abp;

using X.Abp.Account.ExternalProviders;

namespace Microsoft.Extensions.DependencyInjection;

public static class AbpServiceCollectionExternalProviderOptionsExtensions
{
    public static IServiceCollection AddDynamicExternalLoginProviderOptions<TOptions>(
        [NotNull] this IServiceCollection services,
        [NotNull] string authenticationSchema,
        [NotNull] Action<ExternalProviderDefinitionBuilder<TOptions>> buildAction)
        where TOptions : class, new()
    {
        Check.NotNull(services, nameof(services));
        Check.NotNullOrWhiteSpace(authenticationSchema, nameof(authenticationSchema));
        Check.NotNull(buildAction, nameof(buildAction));

        var builder = new ExternalProviderDefinitionBuilder<TOptions>(authenticationSchema);

        buildAction(builder);

        services.Configure<AbpExternalProviderOptions>(options =>
        {
            options.Definitions.Add(builder.Build());
        });

        return services;
    }
}
