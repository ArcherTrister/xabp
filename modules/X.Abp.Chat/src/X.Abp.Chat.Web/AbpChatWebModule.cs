// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Chat.Localization;
using X.Abp.Chat.Permission;
using X.Abp.Chat.Web.Bundling;

namespace X.Abp.Chat.Web;

[DependsOn(
    typeof(AbpChatApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule))]
public class AbpChatWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(AbpChatResource), typeof(AbpChatWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpChatWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpChatWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<AbpChatWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AbpChatWebModule>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/Chat/Index", AbpChatPermissions.Messaging);
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new ChatToolbarContributor());
        });

        Configure<AbpBundlingOptions>(options =>
        {
            options.ScriptBundles
                .Get(StandardBundles.Scripts.Global)
                .AddContributors(typeof(ChatGlobalScriptContributor));
        });

        Configure<DynamicJavaScriptProxyOptions>(options =>
        {
            options.DisableModule(AbpChatRemoteServiceConsts.ModuleName);
        });
    }
}
