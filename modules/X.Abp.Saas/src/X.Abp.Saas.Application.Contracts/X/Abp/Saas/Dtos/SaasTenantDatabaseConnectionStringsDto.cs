// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Application.Dtos;

using Volo.Abp.Auditing;

namespace X.Abp.Saas.Dtos;

public class SaasTenantDatabaseConnectionStringsDto : ExtensibleEntityDto
{
    public string DatabaseName { get; set; }

    [StringLength(1024)]
    [DisableAuditing]
    public string ConnectionString { get; set; }
}
