// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace X.Abp.CmsKit.Admin.Newsletters;

public class NewsletterRecordWithDetailsDto : EntityDto<Guid>, IHasCreationTime
{
    public string EmailAddress { get; set; }

    public ICollection<NewsletterPreferenceDto> Preferences { get; set; }

    public DateTime CreationTime { get; set; }
}
