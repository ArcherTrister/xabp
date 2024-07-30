// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;

namespace X.Abp.Saas.Tenants;

public interface ITenantManager : ITransientDependency, IDomainService
{
    Task<Tenant> CreateAsync(string name, Guid? editionId = null);

    Task ChangeNameAsync(Tenant tenant, string name);

    Task<bool> IsActiveAsync(Tenant tenant);
}
