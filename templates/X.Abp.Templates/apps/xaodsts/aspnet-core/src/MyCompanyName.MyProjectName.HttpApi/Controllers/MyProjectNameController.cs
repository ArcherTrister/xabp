// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MyCompanyName.MyProjectName.Localization;

using Volo.Abp.AspNetCore.Mvc;

namespace MyCompanyName.MyProjectName.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MyProjectNameController : AbpControllerBase
{
    protected MyProjectNameController()
    {
        LocalizationResource = typeof(MyProjectNameResource);
    }
}
