// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace X.Abp.Identity
{
    [RemoteService(Name = AbpIdentityProRemoteServiceConsts.RemoteServiceName)]
    [Area(AbpIdentityProRemoteServiceConsts.ModuleName)]
    [Route("api/identity/external-login")]
    [ControllerName("ExternalLogin")]
    public class IdentityExternalLoginController :
      AbpControllerBase,
      IIdentityExternalLoginAppService
    {
        public IIdentityExternalLoginAppService ExternalLoginAppService { get; }

        public IdentityExternalLoginController(
          IIdentityExternalLoginAppService externalLoginAppService)
        {
            ExternalLoginAppService = externalLoginAppService;
        }

        [HttpPost]
        public virtual Task CreateOrUpdateAsync()
        {
            return ExternalLoginAppService.CreateOrUpdateAsync();
        }
    }
}
