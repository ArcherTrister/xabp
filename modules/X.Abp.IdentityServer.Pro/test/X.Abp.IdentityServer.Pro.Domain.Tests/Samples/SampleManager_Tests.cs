using System.Threading.Tasks;
using Xunit;

namespace X.Abp.IdentityServer.Samples;

public class SampleManager_Tests : AbpIdentityServerProDomainTestBase
{
  //private readonly SampleManager _sampleManager;

  public SampleManager_Tests()
  {
    //_sampleManager = GetRequiredService<SampleManager>();
  }

  [Fact]
  public virtual Task Method1Async()
  {
    return Task.CompletedTask;
  }
}
