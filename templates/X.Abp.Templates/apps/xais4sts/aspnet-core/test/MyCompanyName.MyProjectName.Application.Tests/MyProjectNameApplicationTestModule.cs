// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName;

[DependsOn(
    typeof(MyProjectNameApplicationModule),
    typeof(MyProjectNameDomainTestModule)
    )]
public class MyProjectNameApplicationTestModule : AbpModule
{

}
