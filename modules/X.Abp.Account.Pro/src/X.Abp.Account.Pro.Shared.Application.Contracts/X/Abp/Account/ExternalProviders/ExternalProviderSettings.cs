﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;

namespace X.Abp.Account.ExternalProviders;

[Serializable]
public class ExternalProviderSettings
{
    public string Name { get; set; }

    public bool Enabled { get; set; }

    public List<ExternalProviderSettingsProperty> Properties { get; set; }

    public List<ExternalProviderSettingsProperty> SecretProperties { get; set; }

    public ExternalProviderSettings()
    {
        Properties = new List<ExternalProviderSettingsProperty>();
        SecretProperties = new List<ExternalProviderSettingsProperty>();
    }

    public bool IsValid()
    {
        return (Properties != null && Properties.Any(x => !x.Value.IsNullOrWhiteSpace())) ||
               (SecretProperties != null && SecretProperties.Any(x => !x.Value.IsNullOrWhiteSpace()));
    }
}
