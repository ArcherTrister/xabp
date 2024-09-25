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

    protected EditionManager EditionManager { get; }

    protected ITenantRepository TenantRepository { get; }

    protected IPlanAppService PlanAppService { get; }

    protected AbpSaasPaymentOptions PaymentOptions { get; }

    public EditionAppService(IEditionRepository editionRepository, EditionManager editionManager, ITenantRepository tenantRepository, IServiceProvider serviceProvider, IOptions<AbpSaasPaymentOptions> paymentOptions)
    {
        EditionRepository = editionRepository;
        EditionManager = editionManager;
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

        editionDto.TenantCount = await TenantRepository.GetCountAsync(null, edition.Id);

        return editionDto;
    }

    public virtual async Task<PagedResultDto<EditionDto>> GetListAsync(GetEditionsInput input)
    {
        int count = await EditionRepository.GetCountAsync(input.Filter);
        List<EditionWithTenantCount> editions = await EditionRepository.GetListWithTenantCountAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.Filter, false);
        List<EditionDto> editionDtos = ObjectMapper.Map<List<Edition>, List<EditionDto>>(editions.Select(x => x.Edition).ToList());
        foreach (EditionDto editionDto in editionDtos)
        {
            EditionWithTenantCount editionWithTenantCount = editions.FirstOrDefault(x => x.Edition.Id == editionDto.Id);
            editionDto.TenantCount = editionWithTenantCount != null ? editionWithTenantCount.TenantCount : 0L;
        }

        if (PaymentOptions.IsPaymentSupported)
        {
            Guid[] planIds = editions.Select(x => x.Edition).Where(x => x.PlanId.HasValue).Select(x => x.PlanId.Value).Distinct().ToArray();
            List<PlanDto> plans = await PlanAppService.GetManyAsync(planIds);
            foreach (EditionDto editionDto in editionDtos.Where(x => x.PlanId.HasValue))
            {
                PlanDto planDto = plans.FirstOrDefault(x => x.Id == editionDto.PlanId.Value);
                editionDto.PlanName = planDto?.Name;
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

    [Authorize(AbpSaasPermissions.Editions.Update)]
    public virtual async Task MoveAllTenantsAsync(Guid id, Guid? targetEditionId)
    {
        await EditionManager.MoveAllTenantsAsync((await EditionRepository.GetAsync(id)).Id, targetEditionId);
    }

    public virtual async Task<List<EditionDto>> GetAllListAsync()
    {
        List<EditionWithTenantCount> source = await EditionRepository.GetListWithTenantCountAsync();
        List<EditionDto> editionDtos = ObjectMapper.Map<List<Edition>, List<EditionDto>>(source.Select(x => x.Edition).ToList());
        foreach (EditionDto editionDto in editionDtos)
        {
            EditionWithTenantCount editionWithTenantCount = source.FirstOrDefault(x => x.Edition.Id == editionDto.Id);
            editionDto.TenantCount = editionWithTenantCount != null ? editionWithTenantCount.TenantCount : 0L;
        }

        return editionDtos;
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

    [Obsolete("Use `IPlanAppService` to perform this operation. This will be removed in next major version.")]
    public virtual Task<List<PlanDto>> GetPlanLookupAsync()
    {
        return PaymentOptions.IsPaymentSupported ? PlanAppService.GetPlanListAsync() : Task.FromResult(new List<PlanDto>());
    }
}
