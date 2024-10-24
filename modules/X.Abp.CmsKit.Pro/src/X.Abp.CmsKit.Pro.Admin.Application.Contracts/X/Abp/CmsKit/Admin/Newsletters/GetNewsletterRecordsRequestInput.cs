﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application.Dtos;

namespace X.Abp.CmsKit.Admin.Newsletters;

public class GetNewsletterRecordsRequestInput : PagedResultRequestDto
{
    public string Preference { get; set; }

    public string Source { get; set; }

    public string EmailAddress { get; set; }
}
