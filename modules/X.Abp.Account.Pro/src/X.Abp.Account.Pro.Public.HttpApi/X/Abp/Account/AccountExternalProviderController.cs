// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Account.ExternalProviders;

namespace X.Abp.Account;

[RemoteService(Name = AbpAccountPublicRemoteServiceConsts.RemoteServiceName)]
[Area(AbpAccountPublicRemoteServiceConsts.ModuleName)]
[Route("api/account/external-provider")]
public class AccountExternalProviderController : AbpControllerBase, IAccountExternalProviderAppService
{
    protected IAccountExternalProviderAppService AccountExternalProviderAppService { get; }

    public AccountExternalProviderController(IAccountExternalProviderAppService accountExternalProviderAppService)
    {
        AccountExternalProviderAppService = accountExternalProviderAppService;
    }

    [HttpGet]
    public virtual async Task<ExternalProviderDto> GetAllAsync()
    {
        return await AccountExternalProviderAppService.GetAllAsync();
    }

    // [Authorize]
    [HttpGet]
    [Route("by-name")]
    public virtual async Task<ExternalProviderItemWithSecretDto> GetByNameAsync(GetByNameInput input)
    {
        return await AccountExternalProviderAppService.GetByNameAsync(input);
    }
}
