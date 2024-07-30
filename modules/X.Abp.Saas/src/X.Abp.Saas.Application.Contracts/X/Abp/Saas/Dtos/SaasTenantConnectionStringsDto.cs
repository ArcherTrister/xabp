// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace X.Abp.Saas.Dtos;

public class SaasTenantConnectionStringsDto : ExtensibleEntityDto
{
    [DisableAuditing]
    [StringLength(1024)]
    public string Default { get; set; }

    public List<SaasTenantDatabaseConnectionStringsDto> Databases { get; set; }
}
