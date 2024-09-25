using System.Threading.Tasks;
using Xunit;

namespace X.Abp.FileManagement.Samples;

public class SampleManager_Tests : FileManagementDomainTestBase
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
