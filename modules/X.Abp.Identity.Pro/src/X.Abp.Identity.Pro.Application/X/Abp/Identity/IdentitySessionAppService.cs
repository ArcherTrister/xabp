// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Users;

using X.Abp.Identity.Permissions;

namespace X.Abp.Identity
{
    [Authorize(AbpIdentityProPermissions.Sessions.Default)]
    public class IdentitySessionAppService :
      IdentityAppServiceBase,
      IIdentitySessionAppService
    {
        protected IdentitySessionManager IdentitySessionManager { get; }

        protected IIdentitySessionRepository IdentitySessionRepository { get; }

        protected IIdentityUserRepository IdentityUserRepository { get; }

        public IdentitySessionAppService(
          IdentitySessionManager identitySessionManager,
          IIdentitySessionRepository identitySessionRepository,
          IIdentityUserRepository identityUserRepository)
        {
            IdentitySessionManager = identitySessionManager;
            IdentitySessionRepository = identitySessionRepository;
            IdentityUserRepository = identityUserRepository;
        }

        public virtual async Task<PagedResultDto<IdentitySessionDto>> GetListAsync(
          GetIdentitySessionListInput input)
        {
            long count = await IdentitySessionRepository.GetCountAsync(input.UserId, input.Device, input.ClientId);
            List<IdentitySession> sessions = await IdentitySessionManager.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.UserId, input.Device, input.ClientId);
            List<IdentitySessionDto> dtos = new List<IdentitySessionDto>(ObjectMapper.Map<List<IdentitySession>, List<IdentitySessionDto>>(sessions));
            List<IdentityUser> users = await IdentityUserRepository.GetListByIdsAsync(dtos.Select(x => x.UserId).ToArray());
            foreach (IdentitySessionDto dto in dtos)
            {
                dto.IsCurrent = dto.SessionId == CurrentUser.GetSessionId().ToString();
                if (dto.TenantId.HasValue)
                {
                    dto.TenantName = CurrentTenant.Name;
                }

                dto.UserName = users.FirstOrDefault(x => x.Id == dto.UserId)?.UserName;
            }

            return new PagedResultDto<IdentitySessionDto>(count, dtos);
        }

        public virtual async Task<IdentitySessionDto> GetAsync(Guid id)
        {
            IdentitySession session = await IdentitySessionManager.GetAsync(id);
            IdentitySessionDto dto = ObjectMapper.Map<IdentitySession, IdentitySessionDto>(session);
            if (dto.TenantId.HasValue)
            {
                dto.TenantName = CurrentTenant.Name;
            }

            dto.UserName = (await IdentityUserRepository.GetAsync(dto.UserId)).UserName;
            return dto;
        }

        public virtual async Task RevokeAsync(Guid id)
        {
            await IdentitySessionManager.RevokeAsync(id);
        }
    }
}
