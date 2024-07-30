// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using X.Abp.Payment.Requests;

namespace X.Abp.Payment.Admin.Requests
{
    public interface IPaymentRequestAdminAppService : IApplicationService
    {
        Task<PaymentRequestWithDetailsDto> GetAsync(Guid id);

        Task<PagedResultDto<PaymentRequestWithDetailsDto>> GetListAsync(
          PaymentRequestGetListInput input);
    }
}
