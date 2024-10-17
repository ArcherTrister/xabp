// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application.Dtos;

namespace X.Abp.CmsKit.Admin.PageFeedbacks;

public class GetPageFeedbackListInput : PagedAndSortedResultRequestDto
{
    public string EntityType { get; set; }

    public string EntityId { get; set; }

    public bool? IsHandled { get; set; }

    public bool? IsUseful { get; set; }

    public string Url { get; set; }
}
