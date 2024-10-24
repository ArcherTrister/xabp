// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Modularity;

using Xunit;

namespace X.Abp.OpenIddict.Samples;

/* Write your custom repository tests like that, in this project, as abstract classes.
 * Then inherit these abstract classes from EF Core & MongoDB test projects.
 * In this way, both database providers are tests with the same set tests.
 */

public abstract class SampleRepository_Tests<TStartupModule> : AbpOpenIddictProTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    // private readonly ISampleRepository _sampleRepository;
    protected SampleRepository_Tests()
    {
        // _sampleRepository = GetRequiredService<ISampleRepository>();
    }

    [Fact]
    public virtual async Task Method1Async()
    {
    }
}
