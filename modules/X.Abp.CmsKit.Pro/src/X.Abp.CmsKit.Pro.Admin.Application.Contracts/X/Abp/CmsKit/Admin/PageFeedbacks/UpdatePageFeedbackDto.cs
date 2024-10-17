// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Validation;

using X.Abp.CmsKit.PageFeedbacks;

namespace X.Abp.CmsKit.Admin.PageFeedbacks;

[Serializable]
public class UpdatePageFeedbackDto
{
    public bool IsHandled { get; set; }

    [DynamicMaxLength(typeof(PageFeedbackConst), nameof(PageFeedbackConst.MaxAdminNoteLength))]
    public string AdminNote { get; set; }
}
