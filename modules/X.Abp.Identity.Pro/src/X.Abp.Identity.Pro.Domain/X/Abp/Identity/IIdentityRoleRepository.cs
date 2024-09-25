// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Identity;

namespace X.Abp.Identity;

public interface IIdentityRoleRepository : Volo.Abp.Identity.IIdentityRoleRepository
{
    Task<List<IdentityRoleWithUserCount>> GetListWithUserCountAsync(string filter, bool includeDetails, CancellationToken cancellationToken = default);
}
