// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;

using Microsoft.EntityFrameworkCore;

using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.EntityFrameworkCore;

public static class SaasEfCoreQueryableExtensions
{
    public static IQueryable<Tenant> IncludeDetails(
      this IQueryable<Tenant> queryable,
      bool include = true)
    {
        return !include ? queryable : queryable.Include(tenant => tenant.ConnectionStrings);
    }
}
