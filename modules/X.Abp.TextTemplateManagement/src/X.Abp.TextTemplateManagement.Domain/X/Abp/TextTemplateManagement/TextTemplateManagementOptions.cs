// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.TextTemplateManagement;

public class TextTemplateManagementOptions
{
    public TimeSpan MinimumCacheDuration { get; set; } = TimeSpan.FromHours(1.0);

    public bool SaveStaticTemplatesToDatabase { get; set; } = true;

    public bool IsDynamicTemplateStoreEnabled { get; set; }
}
