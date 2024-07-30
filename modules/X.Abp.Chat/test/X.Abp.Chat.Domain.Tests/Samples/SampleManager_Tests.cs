using System.Threading.Tasks;
using Xunit;

namespace X.Abp.Chat.Samples;

public class SampleManager_Tests : ChatDomainTestBase
{
    //private readonly SampleManager _sampleManager;

    public SampleManager_Tests()
    {
        //_sampleManager = GetRequiredService<SampleManager>();
    }

    [Fact]
    public Task Method1Async()
    {
        return Task.CompletedTask;
    }
}
