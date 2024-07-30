// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace X.Abp.Forms.Responses;

public static class EfCoreResponsesQueryableExtensions
{
    public static IQueryable<FormResponse> IncludeDetails(this IQueryable<FormResponse> queryable,
        bool include = true)
    {
        return !include ? queryable : queryable.Include(a => a.Answers);
    }
}
