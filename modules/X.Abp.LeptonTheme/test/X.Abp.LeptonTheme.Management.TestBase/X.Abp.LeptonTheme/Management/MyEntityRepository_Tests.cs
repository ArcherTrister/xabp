using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace X.Abp.LeptonTheme.Management
{
    public abstract class MyEntityRepository_Tests<TStartupModule> : LeptonThemeManagementTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        
    }
}
