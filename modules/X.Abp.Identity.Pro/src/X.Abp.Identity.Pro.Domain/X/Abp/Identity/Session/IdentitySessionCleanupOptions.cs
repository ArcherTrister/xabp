// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Identity.Session;

public class IdentitySessionCleanupOptions
{
    public bool IsCleanupEnabled { get; set; } = true;

    public int CleanupPeriod { get; set; } = 3600000;

    public TimeSpan InactiveTimeSpan { get; set; } = TimeSpan.FromDays(30.0);
}
