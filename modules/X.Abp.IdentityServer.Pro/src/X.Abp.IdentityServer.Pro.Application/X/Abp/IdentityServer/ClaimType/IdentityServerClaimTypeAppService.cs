// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Identity;

using X.Abp.Identity;
using X.Abp.IdentityServer.ClaimType.Dtos;

namespace X.Abp.IdentityServer.ClaimType;

public class IdentityServerClaimTypeAppService : IdentityServerAppServiceBase, IIdentityServerClaimTypeAppService
{
    protected IIdentityClaimTypeRepository ClaimTypeRepository { get; }

    public IdentityServerClaimTypeAppService(IIdentityClaimTypeRepository claimTypeRepository)
    {
        ClaimTypeRepository = claimTypeRepository;
    }

    public virtual async Task<List<IdentityClaimTypeDto>> GetListAsync()
    {
        var list = await ClaimTypeRepository.GetListAsync();
        return ObjectMapper.Map<List<IdentityClaimType>, List<IdentityClaimTypeDto>>(list);
    }
}
