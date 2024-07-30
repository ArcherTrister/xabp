// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.FileManagement.Files;
using X.Abp.FileManagement.Localization;
using X.Abp.FileManagement.Permissions;
using X.Abp.FileManagement.Web.Menus;

namespace X.Abp.FileManagement.Web;

[DependsOn(
    typeof(AbpFileManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule))]
public class AbpFileManagementWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(FileManagementResource), typeof(AbpFileManagementWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpFileManagementWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new AbpFileManagementMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpFileManagementWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<AbpFileManagementWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AbpFileManagementWebModule>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/FileManagement/", AbpFileManagementPermissions.DirectoryDescriptor.Default);
            options.Conventions.AuthorizePage("/FileManagement/Index", AbpFileManagementPermissions.DirectoryDescriptor.Default);

            options.Conventions.AuthorizePage("/FileManagement/Directory/ChangeNameModal", AbpFileManagementPermissions.DirectoryDescriptor.Update);
            options.Conventions.AuthorizePage("/FileManagement/Directory/CreateModal", AbpFileManagementPermissions.DirectoryDescriptor.Create);

            options.Conventions.AuthorizePage("/FileManagement/File/ChangeNameModal", AbpFileManagementPermissions.FileDescriptor.Update);
            options.Conventions.AuthorizePage("/FileManagement/File/MoveModal", AbpFileManagementPermissions.FileDescriptor.Update);
        });

        Configure<DynamicJavaScriptProxyOptions>(options =>
        {
            options.DisableModule(AbpFileManagementRemoteServiceConsts.ModuleName);
        });

        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(CreateFileInputWithStream));
        });
    }
}
