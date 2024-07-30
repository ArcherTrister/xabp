// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

namespace X.Abp.Account.ExternalProviders;

public class ExternalProviderItemDto
{
    public string Name { get; set; }

    public bool Enabled { get; set; }

    public string Icon { get; set; }

    public List<ExternalProviderSettingsProperty> Properties { get; set; }
}
