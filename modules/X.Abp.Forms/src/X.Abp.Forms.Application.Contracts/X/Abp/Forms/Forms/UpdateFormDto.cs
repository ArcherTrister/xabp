// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Domain.Entities;

namespace X.Abp.Forms.Forms;

public class UpdateFormDto : IHasConcurrencyStamp
{
    [Required]
    [StringLength(FormConsts.MaxTitleLength)]
    public string Title { get; set; }

    [StringLength(FormConsts.MaxDescriptionLength)]
    public string Description { get; set; }

    public string ConcurrencyStamp { get; set; }
}
