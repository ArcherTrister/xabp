// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Masuit.Tools.Systems;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System;
using System.Collections.Generic;

namespace MyCompanyName.MyProjectName.Filters
{
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
                Enum value = (Enum)Enum.Parse(type, item);
                //enumDescriptions.Add($"{item}={Convert.ToInt32(value)},{value.GetDescription()};");
                enumDescriptions.Add($"{item}({value.GetDescription()})={Convert.ToInt32(value)}");
            }
            return $"<br><div>{Environment.NewLine}{string.Join("<br/>" + Environment.NewLine, enumDescriptions)}</div>";
        }
    }
}
