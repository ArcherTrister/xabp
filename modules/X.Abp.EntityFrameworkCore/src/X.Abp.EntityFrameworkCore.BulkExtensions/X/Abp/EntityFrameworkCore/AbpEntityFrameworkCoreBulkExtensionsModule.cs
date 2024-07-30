// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace X.Abp.EntityFrameworkCore
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    public class AbpEntityFrameworkCoreBulkExtensionsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            /*
            {
              "Z.EntityFramework.Extensions": {
                "LicenseName": "[licenseName]",
                "LicenseKey": "[licenseKey]"
              }
            }

            // Check if the license is valid for the default provider (SQL Server)
            string licenseErrorMessage;
            if (!Z.EntityFramework.Extensions.LicenseManager.ValidateLicense(out licenseErrorMessage))
            {
                throw new Exception(licenseErrorMessage);
            }

            // Check if the license is valid for a specific provider
            string licenseErrorMessage;
            if (!Z.EntityFramework.Extensions.LicenseManager.ValidateLicense(out licenseErrorMessage, ProviderType.SqlServer))
            {
                throw new Exception(licenseErrorMessage);
            }
             */

            var configuration = context.Services.GetConfiguration();
            string licenseName = configuration["Z.EntityFramework.Extensions:LicenseName"];
            string licenseKey = configuration["Z.EntityFramework.Extensions:LicenseKey"];

            Z.EntityFramework.Extensions.LicenseManager.AddLicense(licenseName, licenseKey);

            string licenseErrorMessage;
            if (!Z.EntityFramework.Extensions.LicenseManager.ValidateLicense(out licenseErrorMessage))
            {
                throw new Exception(licenseErrorMessage);
            }
        }
    }
}
