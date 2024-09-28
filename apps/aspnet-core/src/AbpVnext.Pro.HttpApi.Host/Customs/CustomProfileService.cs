// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

//using System;

//using System.Threading.Tasks;

//using IdentityServer4.Models;

//using Microsoft.AspNetCore.Identity;

//using Volo.Abp.MultiTenancy;
//using Volo.Abp.Uow;

//using X.Abp.Identity;
//using X.Abp.IdentityServer.AspNetIdentity;

//using IdentityUser = Volo.Abp.Identity.IdentityUser;

//namespace AbpVnext.Pro.Customs;

//public class CustomProfileService : AbpProfileService
//{
//    public CustomProfileService(
//        IdentityUserManager userManager,
//        IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
//        ICurrentTenant currentTenant)
//        : base(userManager, claimsFactory, currentTenant)
//    {
//    }

//    [UnitOfWork]
//    public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
//    {
//        await base.GetProfileDataAsync(context);

//        // var extraClaim = context.Subject.FindFirst(PrivateDomainConsts.WeChatMiniProgramOpenIdClaim);
//        // if (extraClaim != null)
//        // {
//        //    context.IssuedClaims.Add(extraClaim);
//        // }

//        // context.IssuedClaims.AddRange(context.Subject.Identities.First().Claims);
//        // context.IssuedClaims = context.IssuedClaims.DistinctBy(p => p.Type).ToList();
//        Console.WriteLine();

//        // if (context.Subject.Identity.AuthenticationType == CustomWeChatMiniProgramGrantValidator.ExtensionGrantType)
//        // {
//        //    var extraClaim = context.Subject.FindFirst(CustomWeChatMiniProgramGrantValidator.ExtensionGrantType);
//        //    if (extraClaim != null)
//        //    {
//        //        context.IssuedClaims.Add(extraClaim);
//        //    }
//        // }
//    }
//}
