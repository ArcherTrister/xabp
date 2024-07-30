// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.Quartz.Jobs.Dtos;
public class AddJobInput
{
    [Required]
    public string JobGroup { get; set; }

    [Required]
    public string JobName { get; set; }

    public string JobType { get; set; }

    public bool Durable { get; set; }

    public bool RequestsRecovery { get; set; }

    public bool Replace { get; set; }
}
