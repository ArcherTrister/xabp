// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;
using Xunit;

namespace X.Abp.OpenIddict.Samples;

public class SampleManager_Tests : AbpOpenIddictProDomainTestBase
{
  // private readonly SampleManager _sampleManager;
  public SampleManager_Tests()
  {
    // _sampleManager = GetRequiredService<SampleManager>();
  }

  [Fact]
  public virtual async Task Method1Async()
  {
        await Task.CompletedTask;
  }
}
