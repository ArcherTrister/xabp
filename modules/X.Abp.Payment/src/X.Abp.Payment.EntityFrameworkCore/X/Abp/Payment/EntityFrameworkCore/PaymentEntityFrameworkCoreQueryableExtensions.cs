// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;

using Microsoft.EntityFrameworkCore;

using X.Abp.Payment.Requests;

namespace X.Abp.Payment.EntityFrameworkCore;

public static class PaymentEntityFrameworkCoreQueryableExtensions
{
    public static IQueryable<PaymentRequest> IncludeDetails(
      this IQueryable<PaymentRequest> queryable,
      bool include = true)
    {
        return !include ? queryable : queryable.Include(p => p.Products);
    }
}
