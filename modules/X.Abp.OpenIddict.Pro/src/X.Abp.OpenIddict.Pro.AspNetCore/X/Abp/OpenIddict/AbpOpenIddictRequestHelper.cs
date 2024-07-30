// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using OpenIddict.Abstractions;

using Volo.Abp.DependencyInjection;

namespace X.Abp.OpenIddict;

public class AbpOpenIddictRequestHelper : ITransientDependency
{
    public virtual Task<OpenIddictRequest> GetFromReturnUrlAsync(string returnUrl)
    {
        if (!returnUrl.IsNullOrWhiteSpace())
        {
            var qm = returnUrl.IndexOf("?", StringComparison.Ordinal);
            if (qm > 0)
            {
                return Task.FromResult(new OpenIddictRequest(returnUrl[(qm + 1)..]
                    .Split("&")
                    .Select(x =>
                        x.Split("=").Length == 2
                            ? new KeyValuePair<string, string>(x.Split("=")[0], WebUtility.UrlDecode(x.Split("=")[1]))
                            : new KeyValuePair<string, string>(null, null))
                    .Where(x => x.Key != null)));
            }
        }

        return Task.FromResult<OpenIddictRequest>(null);
    }
}
