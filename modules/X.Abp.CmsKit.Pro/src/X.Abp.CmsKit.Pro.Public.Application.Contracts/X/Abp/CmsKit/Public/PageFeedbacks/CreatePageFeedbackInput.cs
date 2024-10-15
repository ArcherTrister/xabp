// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.Validation;

using X.Abp.CmsKit.PageFeedbacks;

namespace X.Abp.CmsKit.Public.PageFeedbacks;

[Serializable]
public class CreatePageFeedbackInput
{
    [DynamicStringLength(typeof(PageFeedbackConst), nameof(PageFeedbackConst.MaxUserNoteLength))]
    public string Url { get; set; }

    [Required]
    public bool IsUseful { get; set; }

    [DynamicStringLength(typeof(PageFeedbackConst), nameof(PageFeedbackConst.MaxUserNoteLength))]
    public string UserNote { get; set; }

    [DynamicStringLength(typeof(PageFeedbackConst), nameof(PageFeedbackConst.MaxEntityTypeLength))]
    [Required]
    public string EntityType { get; set; }

    [DynamicStringLength(typeof(PageFeedbackConst), nameof(PageFeedbackConst.MaxEntityIdLength))]
    public string EntityId { get; set; }
}
