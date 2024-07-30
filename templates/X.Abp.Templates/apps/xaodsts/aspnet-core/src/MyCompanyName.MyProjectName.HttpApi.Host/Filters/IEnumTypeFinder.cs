// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using Volo.Abp.DependencyInjection;

namespace MyCompanyName.MyProjectName.Filters
{
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
}
