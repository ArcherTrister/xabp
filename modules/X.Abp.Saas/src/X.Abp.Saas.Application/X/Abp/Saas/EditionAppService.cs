// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

using X.Abp.Payment.Plans;
using X.Abp.Saas.Dtos;
using X.Abp.Saas.Editions;
using X.Abp.Saas.Permissions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas;

[Authorize(AbpSaasPermissions.Editions.Default)]
public class EditionAppService : SaasAppServiceBase, IEditionAppService
{
    protected IEditionRepository EditionRepository { get; }

    protected ITenantRepository TenantRepository { get; }

    protected IPlanAppService PlanAppService { get; }

    protected AbpSaasPaymentOptions PaymentOptions { get; }

    public EditionAppService(IEditionRepository editionRepository, ITenantRepository tenantRepository, IServiceProvider serviceProvider, IOptions<AbpSaasPaymentOptions> paymentOptions)
    {
        EditionRepository = editionRepository;
        TenantRepository = tenantRepository;
        PlanAppService = serviceProvider.GetService<IPlanAppService>();
        PaymentOptions = paymentOptions.Value;
    }

    public virtual async Task<EditionDto> GetAsync(Guid id)
    {
        var edition = await EditionRepository.GetAsync(id);
        var editionDto = ObjectMapper.Map<Edition, EditionDto>(edition);
        if (PaymentOptions.IsPaymentSupported && edition.PlanId.HasValue)
        {
            var plan = await PlanAppService.GetAsync(edition.PlanId.Value);
            editionDto.PlanName = plan?.Name;
        }

        return editionDto;
    }

    public virtual async Task<PagedResultDto<EditionDto>> GetListAsync(GetEditionsInput input)
    {
        var count = await EditionRepository.GetCountAsync(input.Filter);
        var list = await EditionRepository.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.Filter);
        var editionDtos = ObjectMapper.Map<List<Edition>, List<EditionDto>>(list);
        if (PaymentOptions.IsPaymentSupported)
        {
            var array = (from x in list
                         where x.PlanId.HasValue
                         select x.PlanId.Value).Distinct().ToArray();
            var source = await PlanAppService.GetManyAsync(array);
            foreach (var editionDto in editionDtos.Where(x => x.PlanId.HasValue))
            {
                var val = source.FirstOrDefault(x => x.Id == editionDto.PlanId.Value);
                editionDto.PlanName = val?.Name;
            }
        }

        return new PagedResultDto<EditionDto>(count, editionDtos);
    }

    [Authorize(AbpSaasPermissions.Editions.Create)]
    public virtual async Task<EditionDto> CreateAsync(EditionCreateDto input)
    {
        var edition = new Edition(GuidGenerator.Create(), input.DisplayName)
        {
            PlanId = input.PlanId
        };

        input.MapExtraPropertiesTo(edition);
        await EditionRepository.InsertAsync(edition);
        return ObjectMapper.Map<Edition, EditionDto>(edition);
    }

    [Authorize(AbpSaasPermissions.Editions.Update)]
    public virtual async Task<EditionDto> UpdateAsync(Guid id, EditionUpdateDto input)
    {
        var edition = await EditionRepository.GetAsync(id);
        edition.SetDisplayName(input.DisplayName);
        edition.PlanId = input.PlanId;
        edition.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        input.MapExtraPropertiesTo(edition);
        var source = await EditionRepository.UpdateAsync(edition);
        return ObjectMapper.Map<Edition, EditionDto>(source);
    }

    [Authorize(AbpSaasPermissions.Editions.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await EditionRepository.DeleteAsync(id);
    }

    public virtual Task<List<PlanDto>> GetPlanLookupAsync()
    {
        return PaymentOptions.IsPaymentSupported ? PlanAppService.GetPlanListAsync() : Task.FromResult(new List<PlanDto>());
    }

    public virtual async Task<GetEditionUsageStatisticsResultDto> GetUsageStatisticsAsync()
    {
        var editions = await EditionRepository.GetListAsync();
        var list = from info in await TenantRepository.GetListAsync()
                   group info by info.EditionId into @group
                   select new
                   {
                       EditionId = @group.Key,
                       Count = @group.Count()
                   };
        var dictionary = new Dictionary<string, int>();
        foreach (var x in list)
        {
            var edition = editions.FirstOrDefault(e => e.Id == x.EditionId);
            var text = edition?.DisplayName;
            if (text != null)
            {
                dictionary.Add(text, x.Count);
            }
        }

        return new GetEditionUsageStatisticsResultDto
        {
            Data = dictionary
        };
    }
}
