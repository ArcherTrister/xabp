using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace X.Abp.Forms.Samples;

public class SampleAppService_Tests : FormsApplicationTestBase
{
  private readonly ISampleAppService _sampleAppService;

  public SampleAppService_Tests()
  {
    _sampleAppService = GetRequiredService<ISampleAppService>();
  }

  [Fact]
  public virtual async Task GetAsync()
  {
    var result = await _sampleAppService.GetAsync();
    result.Value.ShouldBe(42);
  }

  [Fact]
  public virtual async Task GetAuthorizedAsync()
  {
    var result = await _sampleAppService.GetAuthorizedAsync();
    result.Value.ShouldBe(42);
  }
}
