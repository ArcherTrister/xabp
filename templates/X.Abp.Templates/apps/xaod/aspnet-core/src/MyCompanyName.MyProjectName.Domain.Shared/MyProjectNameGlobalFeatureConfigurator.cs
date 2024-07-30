// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Volo.Abp.Threading;

namespace MyCompanyName.MyProjectName;

public static class MyProjectNameGlobalFeatureConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            /* You can configure (enable/disable) global features of the used modules here.
             * Please refer to the documentation to learn more about the Global Features System:
             * https://docs.abp.io/en/abp/latest/Global-Features
             */
        });
    }
}
