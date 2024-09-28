// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

//using System.Linq;

//using System.Security.Claims;
//using System.Threading.Tasks;

//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;

//using Volo.Abp.Identity;
//using Volo.Abp.Security.Claims;
//using Volo.Abp.Uow;

//using IdentityRole = Volo.Abp.Identity.IdentityRole;
//using IdentityUser = Volo.Abp.Identity.IdentityUser;

//namespace AbpVnext.Pro.Customs;

//public class CustomUserClaimsPrincipalFactory : AbpUserClaimsPrincipalFactory
//{
//    public CustomUserClaimsPrincipalFactory(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options, ICurrentPrincipalAccessor currentPrincipalAccessor, IAbpClaimsPrincipalFactory abpClaimsPrincipalFactory) : base(userManager, roleManager, options, currentPrincipalAccessor, abpClaimsPrincipalFactory)
//    {
//    }

//    [UnitOfWork]
//    public override async Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
//    {
//        var principal = await base.CreateAsync(user);

//        if (user.TenantId.HasValue)
//        {
//            principal.Identities
//                .First()
//                .AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
//        }

//        return principal;
//    }
//}
