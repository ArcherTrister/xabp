// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;

namespace AbpVnext.Pro.Filters;

/// <summary>
/// 枚举查找器
/// </summary>
public class EnumTypeFinder : IEnumTypeFinder
{
    private static Dictionary<string, Type> _dic = new();

    public EnumTypeFinder()
    {
        // TODO: ITypeFinder IAssemblyFinder
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        _dic = assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsEnum &&
            (type.Name.Equals("DayOfWeek", StringComparison.OrdinalIgnoreCase) ||
            type.FullName.StartsWith("Volo", StringComparison.InvariantCultureIgnoreCase) ||
            type.FullName.StartsWith('X') ||
            type.FullName.StartsWith("AbpVnext.Pro", StringComparison.InvariantCultureIgnoreCase)))
            .Distinct().ToDictionary(p => p.FullName, p => p);
    }

    /// <summary>
    /// 获取枚举类型字典集合
    /// </summary>
    public Dictionary<string, Type> EnumTypes => _dic;
}
