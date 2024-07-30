// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MyCompanyName.MyProjectName.Localization;

using Volo.Abp.Application.Services;

namespace MyCompanyName.MyProjectName;

/* Inherit your application services from this class.
 */
public abstract class MyProjectNameAppService : ApplicationService
{
    protected MyProjectNameAppService()
    {
        LocalizationResource = typeof(MyProjectNameResource);
    }
}
