// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.Forms.Choices;
using X.Abp.Forms.EntityFrameworkCore;

namespace X.Abp.Forms;

public class EfCoreChoiceRepository : EfCoreRepository<IFormsDbContext, Choice, Guid>, IChoiceRepository
{
    public EfCoreChoiceRepository(IDbContextProvider<IFormsDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
