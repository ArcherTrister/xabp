// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X.Abp.OpenIddict;

public static class AbpAsyncEnumerableExtensions
{
    public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        return source is null ? throw new ArgumentNullException(nameof(source)) : ExecuteAsync();

        async Task<List<T>> ExecuteAsync()
        {
            var list = new List<T>();

            await foreach (var element in source)
            {
                list.Add(element);
            }

            return list;
        }
    }
}
