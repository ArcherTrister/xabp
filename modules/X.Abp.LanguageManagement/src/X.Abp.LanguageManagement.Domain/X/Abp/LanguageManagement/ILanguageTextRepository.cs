// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.LanguageManagement;

public interface ILanguageTextRepository : IBasicRepository<LanguageText, Guid>
{
    List<LanguageText> GetList(string resourceName, string cultureName);

    Task<List<LanguageText>> GetListAsync(string resourceName, string cultureName, CancellationToken cancellationToken = default);
}
