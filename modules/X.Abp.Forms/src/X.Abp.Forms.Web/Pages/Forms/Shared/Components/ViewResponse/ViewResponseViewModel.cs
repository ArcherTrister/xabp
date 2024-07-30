// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using X.Abp.Forms.Forms;
using X.Abp.Forms.Responses;

namespace X.Abp.Forms.Web.Pages.Forms.Shared.Components.ViewResponse;

public class ViewResponseViewModel
{
    public string ViewFormUrl { get; set; }

    public FormDto Form { get; set; }

    public FormResponseDto FormResponse { get; set; }
}
