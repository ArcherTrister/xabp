// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Security.Claims;
using System.Threading.Tasks;

using DeviceDetectorNET;
using DeviceDetectorNET.Results;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Polly;

using Volo.Abp.AspNetCore.WebClientInfo;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace AbpVnext.Pro.Samples;
public class SampleAppService : ProAppService
{
    protected ITenantStore TenantStore { get; }

    protected IHttpContextAccessor HttpContextAccessor { get; }

    protected IdentityUserManager UserManager { get; }

    protected IdentityOptions IdentityOptions { get; }

    protected AbpIdentityOptions AbpOptions { get; }

    public SampleAppService(
        ITenantStore tenantStore,
        IHttpContextAccessor httpContextAccessor,
        IdentityUserManager userManager,
        IOptions<IdentityOptions> identityOptions,
        IOptions<AbpIdentityOptions> options)
    {
        TenantStore = tenantStore;
        HttpContextAccessor = httpContextAccessor;
        UserManager = userManager;
        IdentityOptions = identityOptions.Value;
        AbpOptions = options.Value;
    }

    public virtual Task<TenantConfiguration> GetTenantConfigurationAsync(string name)
    {
        return TenantStore.FindAsync(name);
    }

    public async Task TestAsync()
    {
        var userName = "admin";
        var password = "1q2w3E*";
        foreach (var externalLoginProviderInfo in AbpOptions.ExternalLoginProviders.Values)
        {
            var externalLoginProvider = (IExternalLoginProvider)HttpContextAccessor.HttpContext.RequestServices
                .GetRequiredService(externalLoginProviderInfo.Type);

            if (await externalLoginProvider.TryAuthenticateAsync(userName, password))
            {
                var user = await UserManager.FindByNameAsync(userName);
                if (user == null)
                {
                    if (externalLoginProvider is IExternalLoginProviderWithPassword externalLoginProviderWithPassword)
                    {
                        await externalLoginProviderWithPassword.CreateUserAsync(userName, externalLoginProviderInfo.Name, password);
                    }
                    else
                    {
                        await externalLoginProvider.CreateUserAsync(userName, externalLoginProviderInfo.Name);
                    }
                }
                else
                {
                    if (externalLoginProvider is IExternalLoginProviderWithPassword externalLoginProviderWithPassword)
                    {
                        await externalLoginProviderWithPassword.UpdateUserAsync(user, externalLoginProviderInfo.Name, password);
                    }
                    else
                    {
                        await externalLoginProvider.UpdateUserAsync(user, externalLoginProviderInfo.Name);
                    }
                }

                // var aaa = await SignInOrTwoFactorAsync(user, true);
                Console.WriteLine("SignInOrTwoFactor");
            }
        }

        // var bbb = await base.PasswordSignInAsync(userName, password, true, true);
        Console.WriteLine("PasswordSignIn");
        var identityUser = await UserManager.FindByNameAsync(userName);
        if (!await IsTwoFactorClientRememberedAsync(identityUser))
        {
            // Store the userId for use after two factor check
            await UserManager.GetUserIdAsync(identityUser);

            // await HttpContextAccessor.HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, StoreTwoFactorInfo(userId, loginProvider));
            // return SignInResult.TwoFactorRequired;
        }
        else
        {
            Console.WriteLine("XXXX");
        }
    }

    public virtual async Task<bool> IsTwoFactorClientRememberedAsync(IdentityUser user)
    {
        var userId = await UserManager.GetUserIdAsync(user);
        var result = await HttpContextAccessor.HttpContext.AuthenticateAsync(IdentityConstants.TwoFactorRememberMeScheme);
        return result?.Principal != null && result.Principal.FindFirstValue(ClaimTypes.Name) == userId;
    }

    public async Task<ParseResult<DeviceDetectorResult>> GetDeviceInfoAsync()
    {
        //string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3";
        //var userAgent = HttpContext..Request.Headers["User-agent"];
        //IWebClientInfoProvider webClientInfoProvider = context.RequestServices.GetRequiredService<IWebClientInfoProvider>();
        //IHttpContextAccessor httpContextAccessor = LazyServiceProvider.GetRequiredService<IHttpContextAccessor>();

        var userAgent = HttpContextAccessor.HttpContext?.Request?.Headers?["User-Agent"];

        return await Task.FromResult(DeviceDetector.GetInfoFromUserAgent(userAgent));

        /*
        // 创建DeviceDetector实例
        var deviceDetector = new DeviceDetector(userAgent);

        // 获取设备信息
        deviceDetector.Parse();

        // 输出设备类型
        Console.WriteLine("Device Type: " + deviceDetector.);

        // 输出操作系统信息
        Console.WriteLine("OS: " + deviceDetector.Os);

        // 输出浏览器信息
        Console.WriteLine("Browser: " + deviceDetector.Browser);
        */

        // 输出更多信息...
        /*
                     if (deviceDetector.IsParsed())
            {
                var osInfo = deviceDetector.GetOs();
                if (deviceDetector.IsMobile())
                {
                    // IdentitySessionDevices.Mobile
                    device = osInfo.Success ? osInfo.Match.Name : "Mobile";
                }
                else if (deviceDetector.IsBrowser())
                {
                    // IdentitySessionDevices.Web
                    device = "Web";
                }
                else if (deviceDetector.IsDesktop())
                {
                    // TODO: PC
                    device = "Desktop";
                }

                if (osInfo.Success)
                {
                    deviceInfo = osInfo.Match.Name + " " + osInfo.Match.Version;
                }

                var clientInfo = deviceDetector.GetClient();
                if (clientInfo.Success)
                {
                    deviceInfo = deviceInfo.IsNullOrWhiteSpace() ? clientInfo.Match.Name : deviceInfo + " " + clientInfo.Match.Name;
                }
            }
         */
    }
}
