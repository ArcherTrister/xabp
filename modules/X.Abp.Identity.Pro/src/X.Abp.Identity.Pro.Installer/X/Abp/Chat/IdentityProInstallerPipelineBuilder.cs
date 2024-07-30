using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Studio.ModuleInstalling;

namespace X.Chat;

[Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
[ExposeServices(typeof(IModuleInstallingPipelineBuilder))]
public class IdentityProInstallerPipelineBuilder : ModuleInstallingPipelineBuilderBase, IModuleInstallingPipelineBuilder, ITransientDependency
{
    public async Task<ModuleInstallingPipeline> BuildAsync(ModuleInstallingContext context)
    {
        context.AddEfCoreConfigurationMethodDeclaration(
            new EfCoreConfigurationMethodDeclaration(
                "X.Abp.Identity.EntityFrameworkCore",
                "ConfigureIdentityPro"
            )
        );

        return await Task.FromResult(GetBasePipeline(context));
    }
}
