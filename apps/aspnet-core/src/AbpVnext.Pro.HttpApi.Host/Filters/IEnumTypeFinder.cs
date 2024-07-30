// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.DependencyInjection;

namespace AbpVnext.Pro.Filters;

/// <summary>
/// 枚举类型查找器
/// </summary>
public interface IEnumTypeFinder : ISingletonDependency
{
    /// <summary>
    /// 获取枚举类型字典集合
    /// </summary>
    Dictionary<string, Type> EnumTypes { get; }
}
