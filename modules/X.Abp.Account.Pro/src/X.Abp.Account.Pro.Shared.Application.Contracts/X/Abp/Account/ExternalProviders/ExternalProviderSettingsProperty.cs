// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;

namespace X.Abp.Account.ExternalProviders;

[Serializable]
public class ExternalProviderSettingsProperty : NameValue<string>
{
    public ExternalProviderSettingsProperty()
    {
    }

    public ExternalProviderSettingsProperty(string name, string value)
        : base(name, value)
    {
    }
}
