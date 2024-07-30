// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Masuit.Tools.Systems;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace AbpVnext.Pro.Filters;

/// <summary>
/// 向Swagger添加枚举值说明
/// </summary>
public class EnumDocumentFilter : IDocumentFilter
{
    protected IEnumTypeFinder EnumTypeFinder { get; }

    public EnumDocumentFilter(IEnumTypeFinder enumTypeFinder)
    {
        EnumTypeFinder = enumTypeFinder;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var item in swaggerDoc.Components.Schemas)
        {
            var property = item.Value;
            if (property.Enum != null && property.Enum.Count > 0)
            {
                var dict = EnumTypeFinder.EnumTypes;
                if (dict.TryGetValue(item.Key, out Type type))
                {
                    property.Description = DescribeEnum(type);
                }
            }
        }
    }

    private static string DescribeEnum(Type type)
    {
        var enums = Enum.GetNames(type);
        var enumDescriptions = new List<string>();
        foreach (var item in enums)
        {
            var value = (Enum)Enum.Parse(type, item);

            // enumDescriptions.Add($"{item}={Convert.ToInt32(value)},{value.GetDescription()};");
            enumDescriptions.Add($"{item}({value.GetDescription()})={Convert.ToInt32(value)}");
        }

        return $"<br><div>{Environment.NewLine}{string.Join("<br/>" + Environment.NewLine, enumDescriptions)}</div>";
    }
}
