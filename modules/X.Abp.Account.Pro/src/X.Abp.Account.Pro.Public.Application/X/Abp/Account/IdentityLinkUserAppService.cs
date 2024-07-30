// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

using X.Abp.Account.Dtos;
using X.Abp.Identity;

namespace X.Abp.Account;

[Authorize]
public class IdentityLinkUserAppService : ApplicationService, IIdentityLinkUserAppService
{
    protected IdentityLinkUserManager IdentityLinkUserManager { get; }

    protected IdentityUserManager UserManager { get; }

    protected ITenantStore TenantStore { get; }

    public IdentityLinkUserAppService(
        IdentityLinkUserManager identityLinkUserManager,
        IdentityUserManager userManager,
        ITenantStore tenantStore)
    {
        IdentityLinkUserManager = identityLinkUserManager;
        UserManager = userManager;
        TenantStore = tenantStore;
    }

    public virtual async Task<ListResultDto<LinkUserDto>> GetAllListAsync()
    {
        var currentUserId = CurrentUser.GetId();
        var currentTenantId = CurrentTenant.Id;
        using (CurrentTenant.Change(null))
        {
            var linkUsers = await IdentityLinkUserManager.GetListAsync(new IdentityLinkUserInfo(currentUserId, currentTenantId), includeIndirect: true);

            var allLinkUsers = linkUsers.Select(x => new LinkUserDto
            {
                TargetTenantId = x.TargetTenantId,
                TargetUserId = x.TargetUserId,
                DirectlyLinked = (x.SourceTenantId == currentTenantId && x.SourceUserId == currentUserId) || (x.TargetTenantId == currentTenantId && x.TargetUserId == currentUserId)
            }).Concat(linkUsers.Select(x => new LinkUserDto
            {
                TargetTenantId = x.SourceTenantId,
                TargetUserId = x.SourceUserId,
                DirectlyLinked = (x.SourceTenantId == currentTenantId && x.SourceUserId == currentUserId) || (x.TargetTenantId == currentTenantId && x.TargetUserId == currentUserId)
            })).GroupBy(x => new { x.TargetTenantId, x.TargetUserId })
                .Select(x => x.OrderByDescending(y => y.DirectlyLinked).First())
                .Where(x => x.TargetTenantId != currentTenantId || x.TargetUserId != currentUserId)
                .ToList();

            var userDto = new List<LinkUserDto>();
            foreach (var linkUser in allLinkUsers)
            {
                TenantConfiguration tenant = null;
                if (linkUser.TargetTenantId.HasValue)
                {
                    tenant = await TenantStore.FindAsync(linkUser.TargetTenantId.Value);
                }

                using (CurrentTenant.Change(linkUser.TargetTenantId))
                {
                    var user = await UserManager.FindByIdAsync(linkUser.TargetUserId.ToString());
                    if (user != null)
                    {
                        userDto.Add(new LinkUserDto
                        {
                            TargetUserId = user.Id,
                            TargetUserName = user.UserName,
                            TargetTenantId = tenant?.Id,
                            TargetTenantName = tenant?.Name,
                            DirectlyLinked = linkUser.DirectlyLinked
                        });
                    }
                }
            }

            return new ListResultDto<LinkUserDto>(userDto);
        }
    }

    public virtual async Task LinkAsync(LinkUserInput input)
    {
        if (await IdentityLinkUserManager.VerifyLinkTokenAsync(new IdentityLinkUserInfo(input.UserId, input.TenantId), input.Token, LinkUserTokenProviderConsts.LinkUserTokenPurpose))
        {
            await IdentityLinkUserManager.LinkAsync(new IdentityLinkUserInfo(CurrentUser.GetId(), CurrentTenant.Id),
                new IdentityLinkUserInfo(input.UserId, input.TenantId));
        }
        else
        {
            throw new UserFriendlyException("InvalidLinkToken");
        }
    }

    public virtual async Task UnlinkAsync(UnLinkUserInput input)
    {
        await IdentityLinkUserManager.UnlinkAsync(new IdentityLinkUserInfo(CurrentUser.GetId(), CurrentTenant.Id),
            new IdentityLinkUserInfo(input.UserId, input.TenantId));
    }

    public virtual async Task<bool> IsLinkedAsync(IsLinkedInput input)
    {
        return await IdentityLinkUserManager.IsLinkedAsync(
            new IdentityLinkUserInfo(CurrentUser.GetId(), CurrentTenant.Id),
            new IdentityLinkUserInfo(input.UserId, input.TenantId),
            true);
    }

    public virtual async Task<string> GenerateLinkTokenAsync()
    {
        return await IdentityLinkUserManager.GenerateLinkTokenAsync(
            new IdentityLinkUserInfo(CurrentUser.GetId(), CurrentTenant.Id),
            LinkUserTokenProviderConsts.LinkUserTokenPurpose);
    }

    [AllowAnonymous]
    public virtual async Task<bool> VerifyLinkTokenAsync(VerifyLinkTokenInput input)
    {
        return await IdentityLinkUserManager.VerifyLinkTokenAsync(
            new IdentityLinkUserInfo(input.UserId, input.TenantId),
            input.Token,
            LinkUserTokenProviderConsts.LinkUserTokenPurpose);
    }

    public async Task<string> GenerateLinkLoginTokenAsync()
    {
        return await IdentityLinkUserManager.GenerateLinkTokenAsync(
            new IdentityLinkUserInfo(CurrentUser.GetId(), CurrentTenant.Id),
            LinkUserTokenProviderConsts.LinkUserLoginTokenPurpose);
    }

    [AllowAnonymous]
    public async Task<bool> VerifyLinkLoginTokenAsync(VerifyLinkLoginTokenInput input)
    {
        return await IdentityLinkUserManager.VerifyLinkTokenAsync(
            new IdentityLinkUserInfo(input.UserId, input.TenantId),
            input.Token,
            LinkUserTokenProviderConsts.LinkUserLoginTokenPurpose);
    }
}
