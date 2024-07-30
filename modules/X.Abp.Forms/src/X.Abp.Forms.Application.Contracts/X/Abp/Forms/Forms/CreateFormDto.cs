// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.Forms.Forms;

public class CreateFormDto
{
    [Required]
    [StringLength(FormConsts.MaxTitleLength)]
    public string Title { get; set; }

    [StringLength(FormConsts.MaxDescriptionLength)]
    public string Description { get; set; }
}
