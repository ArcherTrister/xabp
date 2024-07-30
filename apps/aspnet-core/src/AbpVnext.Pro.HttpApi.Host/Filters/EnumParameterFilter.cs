// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Masuit.Tools.Systems;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace AbpVnext.Pro.Filters;

public class EnumParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        var type = context.ApiParameterDescription.Type;

        if (type != null)
        {
            var typeIsEnum = type.IsEnum || (IsNullableType(type) && type.GenericTypeArguments[0].IsEnum);

            if (typeIsEnum)
            {
                var enumType = type.IsEnum ? type : type.GenericTypeArguments[0];
                parameter.Description = DescribeEnum(enumType);
            }
        }
    }

    private static bool IsNullableType(Type theType)
    {
        return theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
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
