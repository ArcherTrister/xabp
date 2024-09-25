// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace X.Abp.Identity
{
    [RemoteService(Name = AbpIdentityProRemoteServiceConsts.RemoteServiceName)]
    [Area(AbpIdentityProRemoteServiceConsts.ModuleName)]
    [Route("/api/identity/sessions")]
    [ControllerName("Sessions")]
    public class IdentitySessionController :
      AbpControllerBase,
      IIdentitySessionAppService
    {
        protected IIdentitySessionAppService IdentitySessionAppService { get; }

        public IdentitySessionController(
          IIdentitySessionAppService identitySessionAppService)
        {
            IdentitySessionAppService = identitySessionAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<IdentitySessionDto>> GetListAsync(
          GetIdentitySessionListInput input)
        {
            return IdentitySessionAppService.GetListAsync(input);
        }

        [Route("{id}")]
        [HttpGet]
        public virtual Task<IdentitySessionDto> GetAsync(Guid id) => IdentitySessionAppService.GetAsync(id);

        [HttpDelete]
        [Route("{id}")]
        public virtual Task RevokeAsync(Guid id) => IdentitySessionAppService.RevokeAsync(id);
    }
}
