// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace X.Abp.CmsKit.Polls;

public static class EfCorePollQueryableExtensions
{
    public static IQueryable<Poll> IncludeDetails(
      this IQueryable<Poll> queryable,
      bool include = true)
    {
        return !include ? queryable : queryable.Include(poll => poll.PollOptions);
    }
}
