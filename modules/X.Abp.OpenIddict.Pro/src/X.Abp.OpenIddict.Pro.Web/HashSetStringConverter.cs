// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

namespace X.Abp.OpenIddict.Pro.Web;

public class HashSetStringConverter : IValueConverter<HashSet<string>, string>, IValueConverter<string, HashSet<string>>
{
    public static HashSetStringConverter Converter = new();

    public string Convert(HashSet<string> sourceMember, ResolutionContext context)
    {
        return sourceMember != null && sourceMember.Any() ? sourceMember.Aggregate((x, y) => x + Environment.NewLine + y) : null;
    }

    public HashSet<string> Convert(string sourceMember, ResolutionContext context)
    {
        var stringSet = new HashSet<string>();
        if (!string.IsNullOrWhiteSpace(sourceMember))
        {
            sourceMember = sourceMember.Trim();
            foreach (var str in sourceMember.Trim().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Distinct())
            {
                stringSet.Add(str);
            }
        }

        return stringSet;
    }
}
