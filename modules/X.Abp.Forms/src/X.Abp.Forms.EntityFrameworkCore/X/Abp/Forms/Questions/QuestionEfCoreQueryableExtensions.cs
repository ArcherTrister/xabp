// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace X.Abp.Forms.Questions;

public static class QuestionEfCoreQueryableExtensions
{
    public static IQueryable<QuestionBase> IncludeDetails(this IQueryable<QuestionBase> queryable,
        bool include = true)
    {
        // Bug: Magic string and EfCore bug:https://github.com/dotnet/efcore/issues/22016
        // Bug: https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-5.0/breaking-changes#some-queries-with-correlated-collection-that-also-use-distinct-or-groupby-are-no-longer-supported
        return !include ? queryable : queryable.Include("Choices");
    }
}
