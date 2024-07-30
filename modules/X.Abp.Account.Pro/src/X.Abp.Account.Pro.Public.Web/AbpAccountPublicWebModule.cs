// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.CropperJs;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.Uppy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AutoMapper;
using Volo.Abp.Emailing;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Security;
using Volo.Abp.Sms;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account.Dtos;
using X.Abp.Account.Localization;
using X.Abp.Account.Public.Web.Menus;
using X.Abp.Account.Public.Web.Modules.Account.Components.Toolbar;
using X.Abp.Account.Public.Web.Pages.Account;
using X.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using X.Abp.Account.Public.Web.ProfileManagement;
using X.Abp.Account.Public.Web.Security.Captcha;
using X.Abp.Identity;
using X.Abp.Identity.AspNetCore;
using X.Captcha;
using X.Captcha.G;
using X.Captcha.H;
using X.Captcha.L;
using X.Captcha.Re;

namespace X.Abp.Account.Public.Web;

[DependsOn(
    typeof(AbpIdentityProAspNetCoreModule),
    typeof(AbpEmailingModule),
    typeof(AbpSmsModule),
    typeof(AbpAccountPublicApplicationContractsModule),
    typeof(AbpIdentityProDomainModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpExceptionHandlingModule),
    typeof(AbpAccountPublicWebSharedModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpSecurityModule),
    typeof(AbpAutoMapperModule))]
public class AbpAccountPublicWebModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options => options.AddAssemblyResource(typeof(AccountResource), typeof(AbpAccountPublicApplicationContractsModule).Assembly, typeof(AbpAccountPublicWebModule).Assembly));
        PreConfigure<IMvcBuilder>(mvcBuilder => mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpAccountPublicWebModule).Assembly));
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpAccountPublicWebModule>());
        Configure<AbpNavigationOptions>(options => options.MenuContributors.Add(new AbpAccountPublicMenuContributor()));
        Configure<AbpToolbarOptions>(options => options.Contributors.Add(new AccountModuleToolbarContributor()));
        context.Services.AddAutoMapperObjectMapper<AbpAccountPublicWebModule>();
        Configure<AbpAutoMapperOptions>(options => options.AddProfile<AbpAccountPublicWebAutomapperProfile>(true));

        ConfigureProfileManagement();

        ConfigureCaptcha(context);

        context.Services.AddAuthentication().AddCookie(ConfirmUserModel.ConfirmUserScheme, options =>
        {
            options.LoginPath = new PathString("/Account/Login");
            options.ExpireTimeSpan = TimeSpan.FromMinutes(5.0);
            options.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
            };
        });

        context.Services.AddAuthentication().AddCookie(ChangePasswordModel.ChangePasswordScheme, options =>
        {
            options.LoginPath = new PathString("/Account/Login");
            options.ExpireTimeSpan = TimeSpan.FromMinutes(5.0);
            options.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
            };
        });

        Configure<AbpAspNetCoreMvcOptions>(options => options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(ProfilePictureInput)));
    }

    private void ConfigureProfileManagement()
    {
        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/Account/Manage");
            options.Conventions.AuthorizePage("/Account/SecurityLogs");
            options.Conventions.AuthorizePage("/Account/ScanCodeLogin");
            options.Conventions.AuthorizePage("/Account/SpaExternalLoginBind");
        });

        Configure<ProfileManagementPageOptions>(options =>
        {
            options.Contributors.Add(new AccountProfileManagementPageContributor());
        });

        Configure<AbpBundlingOptions>(options =>
        {
            options.ScriptBundles.Configure(
                StandardBundles.Scripts.Global,
                configuration =>
                {
                    configuration.AddFiles("/client-proxies/account-proxy.js");
                    configuration.AddFiles("/Pages/Account/LinkUsers/account-link-user-global.js");
                });

            options.ScriptBundles
                .Configure(typeof(ManageModel).FullName,
                    configuration =>
                    {
                        configuration.AddFiles("/client-proxies/account-proxy.js");
                        configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/Password/Default.js");
                        configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/ProfilePicture/Default.js");
                        configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/PersonalInfo/Default.js");
                        configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/TwoFactor/Default.js");
                        configuration.AddContributors(typeof(UppyScriptContributor));
                        configuration.AddContributors(typeof(CropperJsScriptContributor));
                    });
            options.StyleBundles
                .Configure(typeof(ManageModel).FullName,
                    configuration =>
                    {
                        configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/ProfilePicture/Default.css");
                        configuration.AddContributors(typeof(CropperJsStyleContributor));
                    });
        });

        Configure<DynamicJavaScriptProxyOptions>(options =>
        {
            options.DisableModule(AbpAccountPublicRemoteServiceConsts.ModuleName);
        });
    }

    private void ConfigureCaptcha(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDynamicOptions<CaptchaOptions, AbpCaptchaOptionsManager>();

        var configuration = context.Services.GetConfiguration();

        if (!configuration["XCaptcha:SiteKey"].IsNullOrWhiteSpace())
        {
            Configure<CaptchaOptions>(CaptchaConsts.G2, configuration.GetSection("XCaptcha"));
            Configure<CaptchaOptions>(CaptchaConsts.G3, configuration.GetSection("XCaptcha"));
            Configure<CaptchaOptions>(CaptchaConsts.H2, configuration.GetSection("XCaptcha"));
            Configure<CaptchaOptions>(CaptchaConsts.L2, configuration.GetSection("XCaptcha"));
            Configure<CaptchaOptions>(CaptchaConsts.Re2, configuration.GetSection("XCaptcha"));
            Configure<CaptchaOptions>(CaptchaConsts.Re3, configuration.GetSection("XCaptcha"));
        }

        context.Services.AddTransient<ICaptchaLanguageCodeProvider, CultureInfoCaptchaLanguageCodeProvider>();
        context.Services.AddTransient<IGCaptchaV2SiteVerify, GCaptchaV2SiteVerify>();
        context.Services.AddTransient<IGCaptchaV3SiteVerify, GCaptchaV3SiteVerify>();
        context.Services.AddTransient<IHCaptchaV2SiteVerify, HCaptchaV2SiteVerify>();
        context.Services.AddTransient<ILCaptchaV2SiteVerify, LCaptchaV2SiteVerify>();
        context.Services.AddTransient<IReCaptchaV2SiteVerify, ReCaptchaV2SiteVerify>();
        context.Services.AddTransient<IReCaptchaV3SiteVerify, ReCaptchaV3SiteVerify>();
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() => ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(
            IdentityModuleExtensionConsts.ModuleName,
            IdentityModuleExtensionConsts.EntityNames.User,
            null,
            [typeof(AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel)]));
    }
}
