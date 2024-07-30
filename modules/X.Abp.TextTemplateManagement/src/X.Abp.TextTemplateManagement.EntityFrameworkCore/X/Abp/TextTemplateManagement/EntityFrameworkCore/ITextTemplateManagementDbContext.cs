// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.TextTemplateManagement.TextTemplates;

namespace X.Abp.TextTemplateManagement.EntityFrameworkCore;

[ConnectionStringName(TextTemplateManagementDbProperties.ConnectionStringName)]
public interface ITextTemplateManagementDbContext : IEfCoreDbContext
{
    DbSet<TextTemplateContent> TextTemplateContents { get; }
}
