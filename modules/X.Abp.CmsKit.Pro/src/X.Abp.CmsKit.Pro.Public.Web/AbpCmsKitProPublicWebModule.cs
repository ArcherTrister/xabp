// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Volo.CmsKit.Localization;
using Volo.CmsKit.Public.Web;

using X.Abp.CmsKit.Pro.Public.Web.Menus;
using X.Abp.CmsKit.Public;
using X.Captcha;

namespace X.Abp.CmsKit.Pro.Public.Web;

[DependsOn(
    typeof(AbpCmsKitProPublicApplicationContractsModule),
    typeof(CmsKitPublicWebModule),
    typeof(AbpAutoMapperModule))]
public class AbpCmsKitProPublicWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(CmsKitResource), typeof(AbpCmsKitProPublicWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpCmsKitProPublicWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpNavigationOptions>(options => options.MenuContributors.Add(new AbpCmsKitProPublicMenuContributor()));
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpCmsKitProPublicWebModule>("X.Abp.CmsKit.Pro.Public.Web"));
        context.Services.AddAutoMapperObjectMapper<AbpCmsKitProPublicWebModule>();

        Configure<AbpAutoMapperOptions>(options => options.AddMaps<AbpCmsKitProPublicWebModule>(true));
        Configure<RazorPagesOptions>(options => options.Conventions.AddPageRoute("/Public/Newsletters/EmailPreferences", "cms/newsletter/email-preferences"));
        Configure<DynamicJavaScriptProxyOptions>(options => options.DisableModule(AbpCmsKitProPublicRemoteServiceConsts.ModuleName));

        context.Services.AddReCaptchaV3(x =>
        {
            x.SiteKey = configuration["CmsKit:ReCaptcha:SiteKey"];
            x.SiteSecret = configuration["CmsKit:ReCaptcha:SiteSecret"];
        });
    }
}
