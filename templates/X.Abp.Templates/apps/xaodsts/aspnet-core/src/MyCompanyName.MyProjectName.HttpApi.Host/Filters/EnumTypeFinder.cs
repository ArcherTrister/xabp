// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyCompanyName.MyProjectName.Filters
{
    /// <summary>
    /// 枚举类型查找器
    /// </summary>
    public class EnumTypeFinder : IEnumTypeFinder
    {
        private static Dictionary<string, Type> _dic = new();

        public EnumTypeFinder()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _dic = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsEnum &&
                (type.Name.Equals("DayOfWeek", StringComparison.OrdinalIgnoreCase) ||
                type.FullName.StartsWith("Volo", StringComparison.InvariantCultureIgnoreCase) ||
                type.FullName.StartsWith("X", StringComparison.InvariantCultureIgnoreCase) ||
                type.FullName.StartsWith("MyCompanyName.MyProjectName", StringComparison.InvariantCultureIgnoreCase)))
                .Distinct().ToDictionary(p => p.FullName, p => p);
        }

        /// <summary>
        /// 获取枚举类型字典集合
        /// </summary>
        public Dictionary<string, Type> EnumTypes => _dic;
    }
}
