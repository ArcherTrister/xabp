using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Cli;

public class CliDataSeedContributor : IDataSeedContributor, ITransientDependency
{
  private readonly IGuidGenerator _guidGenerator;
  private readonly ICurrentTenant _currentTenant;

  public CliDataSeedContributor(
      IGuidGenerator guidGenerator, ICurrentTenant currentTenant)
  {
    _guidGenerator = guidGenerator;
    _currentTenant = currentTenant;
  }

  public virtual Task SeedAsync(DataSeedContext context)
  {
    /* Instead of returning the Task.CompletedTask, you can insert your test data
     * at this point!
     */

    using (_currentTenant.Change(context?.TenantId))
    {
      return Task.CompletedTask;
    }
  }
}
