// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using OpenIddict.Server;

using Volo.Abp.Text.Formatting;

namespace X.Abp.OpenIddict.WildcardDomains;

public abstract class AbpOpenIddictWildcardDomainBase<THandler, TContext> : IOpenIddictServerHandler<TContext>
    where THandler : class
    where TContext : OpenIddictServerEvents.BaseContext
{
    protected THandler Handler { get; set; }

    protected AbpOpenIddictWildcardDomainOptions WildcardDomainOptions { get; }

    protected AbpOpenIddictWildcardDomainBase(IOptions<AbpOpenIddictWildcardDomainOptions> wildcardDomainOptions, THandler handler)
    {
        WildcardDomainOptions = wildcardDomainOptions.Value;
        Handler = handler;
    }

    public abstract ValueTask HandleAsync(TContext context);

    protected virtual Task<bool> CheckWildcardDomainAsync(string url)
    {
        foreach (var domainFormat in WildcardDomainOptions.WildcardDomainsFormat)
        {
            var extractResult = FormattedStringValueExtracter.Extract(url, domainFormat, ignoreCase: true);
            if (extractResult.IsMatch)
            {
                return Task.FromResult(true);
            }
        }

        foreach (var domainFormat in WildcardDomainOptions.WildcardDomainsFormat)
        {
            if (domainFormat.Replace("{0}.", string.Empty).Equals(url, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
