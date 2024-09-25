//// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
//// See https://github.com/ArcherTrister/xabp
//// for more information concerning the license and the contributors participating to this project.

//using System.Threading.Tasks;

//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//using Volo.Abp.Identity;
//using Volo.Abp.Identity.AspNetCore;
//using Volo.Abp.Settings;

//using IdentityUser = Volo.Abp.Identity.IdentityUser;

//namespace AbpVnext.Pro.Customs;

//public class CustomSignInManager : AbpSignInManager
//{
//    public CustomSignInManager(IdentityUserManager userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<IdentityUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<IdentityUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<IdentityUser> confirmation, IOptions<AbpIdentityOptions> options, ISettingProvider settingProvider) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation, options, settingProvider)
//    {
//    }

//    //protected AbpIdentityOptions AbpOptions { get; }

//    //public CustomSignInManager(
//    //    IdentityUserManager userManager,
//    //    IHttpContextAccessor contextAccessor,
//    //    IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
//    //    IOptions<IdentityOptions> optionsAccessor,
//    //    ILogger<SignInManager<IdentityUser>> logger,
//    //    IAuthenticationSchemeProvider schemes,
//    //    IUserConfirmation<IdentityUser> confirmation,
//    //    IOptions<AbpIdentityOptions> options)
//    //    : base(
//    //    userManager,
//    //    contextAccessor,
//    //    claimsFactory,
//    //    optionsAccessor,
//    //    logger,
//    //    schemes,
//    //    confirmation)
//    //{
//    //    AbpOptions = options.Value;
//    //}

//    public override async Task<SignInResult> PasswordSignInAsync(
//        string userName,
//        string password,
//        bool isPersistent,
//        bool lockoutOnFailure)
//    {
//        foreach (var externalLoginProviderInfo in AbpOptions.ExternalLoginProviders.Values)
//        {
//            var externalLoginProvider = (IExternalLoginProvider)Context.RequestServices
//                .GetRequiredService(externalLoginProviderInfo.Type);

//            if (await externalLoginProvider.TryAuthenticateAsync(userName, password))
//            {
//                var user = await UserManager.FindByNameAsync(userName);
//                if (user == null)
//                {
//                    user = externalLoginProvider is IExternalLoginProviderWithPassword externalLoginProviderWithPassword
//                        ? await externalLoginProviderWithPassword.CreateUserAsync(userName, externalLoginProviderInfo.Name, password)
//                        : await externalLoginProvider.CreateUserAsync(userName, externalLoginProviderInfo.Name);
//                }
//                else
//                {
//                    if (externalLoginProvider is IExternalLoginProviderWithPassword externalLoginProviderWithPassword)
//                    {
//                        await externalLoginProviderWithPassword.UpdateUserAsync(user, externalLoginProviderInfo.Name, password);
//                    }
//                    else
//                    {
//                        await externalLoginProvider.UpdateUserAsync(user, externalLoginProviderInfo.Name);
//                    }
//                }

//                return await SignInOrTwoFactorAsync(user, isPersistent);
//            }
//        }

//        return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
//    }

//    protected override async Task<SignInResult> PreSignInCheck(IdentityUser user)
//    {
//        if (!user.IsActive)
//        {
//            Logger.LogWarning("The user is not active therefore cannot login! (username: \"{UserName}\", id:\"{Id}\")", user.UserName, user.Id);
//            return SignInResult.NotAllowed;
//        }

//        // if (user.ShouldChangePasswordOnNextLogin)
//        // {
//        //    Logger.LogWarning("The user should change password! (username: \"{UserName}\", id:\"{Id}\")", user.UserName, user.Id);
//        //    return SignInResult.NotAllowed;
//        // }
//        return await base.PreSignInCheck(user);
//    }
//}
