// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Forms.Localization;
using X.Abp.Forms.Permissions;
using X.Abp.Forms.Web.Menus;

namespace X.Abp.Forms.Web;

[DependsOn(
    typeof(AbpAutoMapperModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpFormsApplicationContractsModule))]
public class AbpFormsWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(FormsResource), typeof(AbpFormsWebModule).Assembly, typeof(AbpFormsApplicationContractsModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpFormsWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/FormQuestions/Vue-question-choice.js");
            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/FormQuestions/Vue-question-types.js");
            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/FormQuestions/Vue-question-item.js");
            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/FormQuestions/Default.js");

            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/FormResponses/Vue-block-response-component.js");
            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/FormResponses/Vue-response-chart.js");
            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/FormResponses/Vue-response-answers.js");
            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/FormResponses/Default.js");

            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/ViewForm/Vue-email-property.js");
            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/ViewForm/Vue-answer.js");
            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/ViewForm/Default.js");

            options.MinificationIgnoredFiles.Add("/Pages/Forms/Shared/Components/ViewResponse/Default.js");
        });

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new AbpFormsMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpFormsWebModule>("X.Abp.Forms.Web");
        });

        context.Services.AddAutoMapperObjectMapper<AbpFormsWebModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AbpFormsWebModule>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/Pages/Forms/Index/", AbpFormsPermissions.Forms.Default);
            options.Conventions.AuthorizePage("/Pages/Forms/CreateModal/", AbpFormsPermissions.Forms.Default);
            options.Conventions.AuthorizePage("/Pages/Forms/SendModal/", AbpFormsPermissions.Forms.Default);
            options.Conventions.AuthorizeFolder("/Pages/Forms/Questions/", AbpFormsPermissions.Forms.Default);
        });

        Configure<DynamicJavaScriptProxyOptions>(options =>
        {
            options.DisableModule(AbpFormsRemoteServiceConsts.ModuleName);
        });
    }
}
