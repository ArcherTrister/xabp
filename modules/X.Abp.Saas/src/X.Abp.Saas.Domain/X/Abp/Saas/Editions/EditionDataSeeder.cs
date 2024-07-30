// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Saas.Editions;

public class EditionDataSeeder : IEditionDataSeeder, ITransientDependency
{
    protected IEditionRepository EditionRepository { get; }

    protected IGuidGenerator GuidGenerator { get; }

    protected ICurrentTenant CurrentTenant { get; }

    public EditionDataSeeder(IEditionRepository editionRepository, IGuidGenerator guidGenerator, ICurrentTenant currentTenant)
    {
        EditionRepository = editionRepository;
        GuidGenerator = guidGenerator;
        CurrentTenant = currentTenant;
    }

    public virtual async Task CreateStandardEditionsAsync()
    {
        if (CurrentTenant.IsAvailable)
        {
            return;
        }

        await AddEditionIfNotExistsAsync("Standard");
    }

    protected virtual async Task AddEditionIfNotExistsAsync(string displayName)
    {
        var flag = await EditionRepository.CheckNameExistAsync(displayName);

        if (!flag)
        {
            await EditionRepository.InsertAsync(new Edition(GuidGenerator.Create(), displayName));
        }
    }
}
