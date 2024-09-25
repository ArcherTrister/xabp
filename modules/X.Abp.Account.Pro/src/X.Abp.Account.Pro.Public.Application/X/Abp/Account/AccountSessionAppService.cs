// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.Users;

using X.Abp.Account.Dtos;
using X.Abp.Identity;

using IIdentityUserRepository = X.Abp.Identity.IIdentityUserRepository;

namespace X.Abp.Account
{
    [Authorize]
    public class AccountSessionAppService :
      ApplicationService,
      IAccountSessionAppService
    {
        protected IdentitySessionManager IdentitySessionManager { get; }

        protected IIdentitySessionRepository IdentitySessionRepository { get; }

        protected IIdentityUserRepository IdentityUserRepository { get; }

        public AccountSessionAppService(
          IdentitySessionManager identitySessionManager,
          IIdentitySessionRepository identitySessionRepository,
          IIdentityUserRepository identityUserRepository)
        {
            IdentitySessionManager = identitySessionManager;
            IdentitySessionRepository = identitySessionRepository;
            IdentityUserRepository = identityUserRepository;
        }

        public virtual async Task<PagedResultDto<IdentitySessionDto>> GetListAsync(GetAccountIdentitySessionListInput input)
        {
            long count = await IdentitySessionRepository.GetCountAsync(CurrentUser.GetId(), input.Device, input.ClientId);
            List<IdentitySession> source = await IdentitySessionManager.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, CurrentUser.GetId(), input.Device, input.ClientId);
            List<IdentitySessionDto> items = new List<IdentitySessionDto>(ObjectMapper.Map<List<IdentitySession>, List<IdentitySessionDto>>(source));
            foreach (IdentitySessionDto identitySessionDto in items)
            {
                identitySessionDto.IsCurrent = identitySessionDto.SessionId == CurrentUserExtensions.GetSessionId(CurrentUser);
                identitySessionDto.TenantName = CurrentTenant.Name;
                identitySessionDto.UserName = CurrentUser.UserName;
            }

            return new PagedResultDto<IdentitySessionDto>(count, items);
        }

        public virtual async Task<IdentitySessionDto> GetAsync(Guid id)
        {
            IdentitySession source = await GetCurrentUserSessionAsync(id);
            IdentitySessionDto identitySessionDto = ObjectMapper.Map<IdentitySession, IdentitySessionDto>(source);
            identitySessionDto.IsCurrent = identitySessionDto.SessionId == CurrentUserExtensions.GetSessionId(CurrentUser);
            if (identitySessionDto.TenantId.HasValue)
            {
                identitySessionDto.TenantName = CurrentTenant.Name;
            }

            identitySessionDto.UserName = CurrentUser.Name;
            return identitySessionDto;
        }

        public virtual async Task RevokeAsync(Guid id)
        {
            await IdentitySessionManager.RevokeAsync((await GetCurrentUserSessionAsync(id)).Id);
        }

        protected virtual async Task<IdentitySession> GetCurrentUserSessionAsync(Guid id)
        {
            IdentitySession session = await IdentitySessionManager.GetAsync(id);
            if (session.UserId != CurrentUser.GetId())
            {
                throw new EntityNotFoundException(typeof(IdentitySession), id);
            }

            return session;
        }
    }
}
