using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Studio.ModuleInstalling;

namespace X.Abp.LeptonTheme
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IModuleInstallingPipelineBuilder))]
    public class LeptonThemeInstallerPipelineBuilder : ModuleInstallingPipelineBuilderBase, IModuleInstallingPipelineBuilder, ITransientDependency
    {
        public async Task<ModuleInstallingPipeline> BuildAsync(ModuleInstallingContext context)
        {
            return GetBasePipeline(context);
        }
    }
}
