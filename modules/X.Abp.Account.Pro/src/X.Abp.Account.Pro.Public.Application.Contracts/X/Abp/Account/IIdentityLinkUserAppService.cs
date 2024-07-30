// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using X.Abp.Account.Dtos;

namespace X.Abp.Account;

public interface IIdentityLinkUserAppService : IApplicationService
{
    Task<ListResultDto<LinkUserDto>> GetAllListAsync();

    Task LinkAsync(LinkUserInput input);

    Task UnlinkAsync(UnLinkUserInput input);

    Task<bool> IsLinkedAsync(IsLinkedInput input);

    Task<string> GenerateLinkTokenAsync();

    Task<bool> VerifyLinkTokenAsync(VerifyLinkTokenInput input);

    Task<string> GenerateLinkLoginTokenAsync();

    Task<bool> VerifyLinkLoginTokenAsync(VerifyLinkLoginTokenInput input);
}
