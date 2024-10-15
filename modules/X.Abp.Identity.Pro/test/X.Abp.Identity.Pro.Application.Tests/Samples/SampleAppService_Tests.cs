using System.Threading.Tasks;

using Shouldly;

using Xunit;

namespace X.Abp.Identity.Samples;

public class SampleAppService_Tests : AbpIdentityProApplicationTestBase
{
    // private readonly ISampleAppService _sampleAppService;
    private readonly IIdentityUserRepository _identityUserRepository;

    public SampleAppService_Tests()
    {
        // _sampleAppService = GetRequiredService<ISampleAppService>();
        _identityUserRepository = GetRequiredService<IIdentityUserRepository>();
    }

    [Fact]
    public async Task FindUserByPhoneNumber_Test()
    {
        var user = await _identityUserRepository.FindByPhoneNumberAsync("13888888888");
        user.ShouldNotBeNull();
    }
}
