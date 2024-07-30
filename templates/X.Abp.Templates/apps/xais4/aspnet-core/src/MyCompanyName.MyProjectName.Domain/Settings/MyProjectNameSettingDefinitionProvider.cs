// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Volo.Abp.Settings;

namespace MyCompanyName.MyProjectName.Settings;

public class MyProjectNameSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MyProjectNameSettings.MySetting1));
    }
}
