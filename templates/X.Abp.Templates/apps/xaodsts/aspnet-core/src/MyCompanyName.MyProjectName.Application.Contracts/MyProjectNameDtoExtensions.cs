// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Volo.Abp.Threading;

namespace MyCompanyName.MyProjectName;

public static class MyProjectNameDtoExtensions
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            /* You can add extension properties to DTOs
             * defined in the depended modules.
             *
             * Example:
             *
             * ObjectExtensionManager.Instance
             *   .AddOrUpdateProperty<IdentityRoleDto, string>("Title");
             *
             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Object-Extensions
             */
        });
    }
}
