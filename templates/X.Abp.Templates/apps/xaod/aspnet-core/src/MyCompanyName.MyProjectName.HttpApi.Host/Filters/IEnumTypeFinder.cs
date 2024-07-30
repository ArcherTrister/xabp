// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using Volo.Abp.DependencyInjection;

namespace MyCompanyName.MyProjectName.Filters
{
    /// <summary>
    /// ö�����Ͳ�����
    /// </summary>
    public interface IEnumTypeFinder : ISingletonDependency
    {
        /// <summary>
        /// ��ȡö�������ֵ伯��
        /// </summary>
        Dictionary<string, Type> EnumTypes { get; }
    }
}
