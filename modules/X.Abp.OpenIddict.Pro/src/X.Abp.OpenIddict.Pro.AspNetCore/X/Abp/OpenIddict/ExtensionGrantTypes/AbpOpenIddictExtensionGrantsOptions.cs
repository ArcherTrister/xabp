// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;

namespace X.Abp.OpenIddict.ExtensionGrantTypes;

public class AbpOpenIddictExtensionGrantsOptions
{
    public Dictionary<string, IExtensionGrant> Grants { get; }

    public AbpOpenIddictExtensionGrantsOptions()
    {
        Grants = new Dictionary<string, IExtensionGrant>();
    }

    public TExtensionGrantType Find<TExtensionGrantType>(string name)
        where TExtensionGrantType : IExtensionGrant
    {
        return (TExtensionGrantType)Grants.FirstOrDefault(x => x.Key == name && x.Value is TExtensionGrantType).Value;
    }
}
