// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.BlobStoring;
using Volo.Abp.Imaging;
using Volo.Abp.Modularity;
using Volo.Abp.Sms;
using Volo.Abp.Timing;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Account;

[DependsOn(
    typeof(AbpAutoMapperModule),
    typeof(AbpBlobStoringModule),
    typeof(AbpImagingImageSharpModule),
    typeof(AbpSmsModule),
    typeof(AbpTimingModule),
    typeof(AbpAccountPublicApplicationContractsModule),
    typeof(AbpAccountSharedApplicationModule))]
public class AbpAccountPublicApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].Urls[AccountUrlNames.PasswordReset] = "Account/ResetPassword";
            options.Applications["MVC"].Urls[AccountUrlNames.EmailConfirmation] = "Account/EmailConfirmation";
        });

        context.Services.AddAutoMapperObjectMapper<AbpAccountPublicApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<AbpAccountPubicApplicationModuleAutoMapperProfile>();
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpAccountPublicApplicationModule>();
        });

        context.Services.AddHttpClient("GravatarHttpClient");
    }
}
