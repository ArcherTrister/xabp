// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Volo.Abp;

namespace X.Abp.CmsKit.Contact;

public class CmsKitContactConfigOptions
{
    public Dictionary<string, ContactConfig> ContactConfigs { get; }

    public CmsKitContactConfigOptions()
    {
        ContactConfigs = new Dictionary<string, ContactConfig>();
    }

    public void AddContact(string name, string receiverEmailAddress)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        ContactConfig contactConfig = new ContactConfig(receiverEmailAddress);
        ContactConfigs.Add(name, contactConfig);
    }
}
