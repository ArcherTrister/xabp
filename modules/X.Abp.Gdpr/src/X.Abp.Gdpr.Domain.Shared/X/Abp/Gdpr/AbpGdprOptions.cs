// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Gdpr;

public class AbpGdprOptions
{
    public TimeSpan RequestTimeInterval { get; set; } = TimeSpan.FromDays(1.0);

    public int MinutesForDataPreparation { get; set; } = 60;
}
