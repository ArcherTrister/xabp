// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AbpVnext.Pro.Customs;

internal static class StringExtensions
{
    [DebuggerStepThrough]
    public static string CleanUrlPath(this string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            url = "/";
        }

        if (url != "/" && url.EndsWith("/", System.StringComparison.OrdinalIgnoreCase))
        {
            url = url[..^1];
        }

        return url;
    }

    [DebuggerStepThrough]
    public static string EnsureLeadingSlash(this string url)
    {
        return url != null && !url.StartsWith("/", System.StringComparison.OrdinalIgnoreCase) ? "/" + url : url;
    }

    [DebuggerStepThrough]
    public static string ToSpaceSeparatedString(this IEnumerable<string> list)
    {
        if (list == null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder(100);

        foreach (var element in list)
        {
            sb.Append(element + " ");
        }

        return sb.ToString().Trim();
    }

    [DebuggerStepThrough]
    public static bool IsMissing(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    [DebuggerStepThrough]
    public static bool IsPresent(this string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    [DebuggerStepThrough]
    public static string EnsureTrailingSlash(this string url)
    {
        return url != null && !url.EndsWith("/", System.StringComparison.OrdinalIgnoreCase) ? url + "/" : url;
    }
}
