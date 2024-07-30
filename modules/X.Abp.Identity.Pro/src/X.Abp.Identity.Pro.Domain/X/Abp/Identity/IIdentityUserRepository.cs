// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Identity;

namespace X.Abp.Identity;

public interface IIdentityUserRepository : Volo.Abp.Identity.IIdentityUserRepository
{
    Task<bool> IsPhoneNumberUedAsync(string phoneNumber, CancellationToken cancellationToken = default);

    Task<IdentityUser> FindByPhoneNumberAsync(string phoneNumber, bool includeDetails = true, CancellationToken cancellationToken = default);

    // Task<IdentityUser> FindByTenantIdAndUserNameAsync(string userName, Guid tenantId, CancellationToken cancellationToken = default);
}
