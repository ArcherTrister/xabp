// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using X.Abp.Payment.Requests;
using X.Abp.Saas.Editions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.Subscription;

public class SubscriptionAppService : SaasAppServiceBase, ISubscriptionAppService
{
    protected IPaymentRequestAppService PaymentRequestAppService { get; }

    protected ITenantRepository TenantRepository { get; }

    protected EditionManager EditionManager { get; }

    public SubscriptionAppService(IPaymentRequestAppService paymentRequestAppService, ITenantRepository tenantRepository, EditionManager editionManager)
    {
        PaymentRequestAppService = paymentRequestAppService;
        TenantRepository = tenantRepository;
        EditionManager = editionManager;
    }

    public virtual async Task<PaymentRequestWithDetailsDto> CreateSubscriptionAsync(Guid editionId, Guid tenantId)
    {
        var edition = await EditionManager.GetEditionForSubscriptionAsync(editionId);

        var input = new PaymentRequestCreateDto();

        var productCreateDto = new PaymentRequestProductCreateDto
        {
            PlanId = edition.PlanId,
            Name = edition.DisplayName,
            Code = $"{tenantId}_{edition.PlanId}",
            Count = 1,
            PaymentType = PaymentType.Subscription
        };
        input.Products.Add(productCreateDto);
        input.ExtraProperties.Add(EditionConsts.EditionIdParameterName, editionId);
        input.ExtraProperties.Add(TenantConsts.TenantIdParameterName, tenantId);
        var paymentRequest = await PaymentRequestAppService.CreateAsync(input);
        var tenant = await TenantRepository.GetAsync(tenantId, true);
        tenant.EditionEndDateUtc = DateTime.UtcNow;
        await TenantRepository.UpdateAsync(tenant);

        return paymentRequest;
    }
}
