// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Volo.Abp;
using Volo.Abp.Domain.Services;

using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.Editions;

public class EditionManager : DomainService
{
    protected IEditionRepository EditionRepository { get; }

    protected ITenantRepository TenantRepository { get; }

    public EditionManager(IEditionRepository editionRepository, ITenantRepository tenantRepository)
    {
        EditionRepository = editionRepository;
        TenantRepository = tenantRepository;
    }

    public virtual async Task<Edition> CreateAsync(string displayName)
    {
        Check.NotNullOrWhiteSpace(displayName, nameof(displayName), EditionConsts.MaxDisplayNameLength);

        await ValidateDisplayNameAsync(displayName);
        return new Edition(GuidGenerator.Create(), displayName);
    }

    public virtual async Task ChangeDisplayNameAsync(Edition edition, string displayName)
    {
        Check.NotNull(edition, nameof(edition));
        Check.NotNullOrWhiteSpace(displayName, nameof(displayName), EditionConsts.MaxDisplayNameLength);
        await ValidateDisplayNameAsync(displayName, edition.Id);
        edition.SetDisplayName(displayName);
    }

    protected virtual async Task ValidateDisplayNameAsync(string displayName, Guid? expectedId = null)
    {
        Edition edition = await EditionRepository.FindByDisplayNameAsync(displayName);
        if (edition == null)
        {
            return;
        }

        if (expectedId.HasValue && expectedId.Value != edition.Id)
        {
            throw new BusinessException("X.Abp.Saas:DuplicateEditionDisplayName").WithData("Name", displayName);
        }
    }

    public virtual async Task<Edition> GetEditionForSubscriptionAsync(Guid id)
    {
        var edition = await EditionRepository.GetAsync(id);
        await CheckEditionForSubscriptionAsync(edition);
        return edition;
    }

    public virtual Task CheckEditionForSubscriptionAsync(Edition edition)
    {
        return edition.PlanId == null ? throw new EditionDoesntHavePlanException(edition.Id) : Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Edition edition, Guid? assignToEditionId = null)
    {
        await TenantRepository.UpdateEditionsAsync(edition.Id, assignToEditionId);

        await EditionRepository.DeleteAsync(edition, false);
    }

    public virtual async Task MoveAllTenantsAsync(Guid id, Guid? targetEditionId = null)
    {
        await TenantRepository.UpdateEditionsAsync(id, targetEditionId);
    }
}
