using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Studio.ModuleInstalling;

namespace X.Abp.Account;

[Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
[ExposeServices(typeof(IModuleInstallingPipelineBuilder))]
public class AccountProInstallerPipelineBuilder : ModuleInstallingPipelineBuilderBase, IModuleInstallingPipelineBuilder, ITransientDependency
{
    public async Task<ModuleInstallingPipeline> BuildAsync(ModuleInstallingContext context)
    {
        //context.AddEfCoreConfigurationMethodDeclaration(
        //    new EfCoreConfigurationMethodDeclaration(
        //        "Volo.Abp.Account.EntityFrameworkCore",
        //        "ConfigureAccountPro"
        //    )
        //);

        return await Task.FromResult(GetBasePipeline(context));
    }
}
