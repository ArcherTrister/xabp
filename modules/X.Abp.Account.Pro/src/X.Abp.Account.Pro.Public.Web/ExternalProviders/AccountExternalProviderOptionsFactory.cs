// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Microsoft.Extensions.Options;

using Volo.Abp.Options;

namespace X.Abp.Account.Public.Web.ExternalProviders;

public class AccountExternalProviderOptionsFactory<TOptions> : AbpOptionsFactory<TOptions>
    where TOptions : class, new()
{
    public AccountExternalProviderOptionsFactory(
        IEnumerable<IConfigureOptions<TOptions>> setups,
        IEnumerable<IPostConfigureOptions<TOptions>> postConfigures)
        : base(setups, postConfigures)
    {
    }

    protected override void ValidateOptions(string name, TOptions options)
    {
        // We will dynamically configure options.
    }
}
