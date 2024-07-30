// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using Masuit.Tools.Systems;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyCompanyName.MyProjectName.Filters
{
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
                    if (dict.ContainsKey(item.Key))
                    {
                        property.Description = DescribeEnum(dict[item.Key]);
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
                Enum value = (Enum)Enum.Parse(type, item);

                // enumDescriptions.Add($"{item}={Convert.ToInt32(value)},{value.GetDescription()};");
                enumDescriptions.Add($"{item}({value.GetDescription()})={Convert.ToInt32(value)}");
            }

            return $"<br><div>{Environment.NewLine}{string.Join("<br/>" + Environment.NewLine, enumDescriptions)}</div>";
        }
    }
}
