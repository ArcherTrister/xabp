// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Shouldly;

using X.Abp.Identity;

using Xunit;

namespace AbpVnext.Pro.Samples;

/* This is just an example test class.
 * Normally, you don't test code of the modules you are using
 * (like IIdentityUserAppService here).
 * Only test your own application services.
 */
public class SampleAppServiceTests : ProApplicationTestBase
{
    private readonly IIdentityUserAppService _userAppService;

    public SampleAppServiceTests()
    {
        _userAppService = GetRequiredService<IIdentityUserAppService>();
    }

    [Fact]
    public async Task InitialDataShouldContainAdminUser()
    {
        // Act
        var result = await _userAppService.GetListAsync(new GetIdentityUsersInput());

        // Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(u => u.UserName == "admin");
    }

    [Fact]
    public void MathShouldInt()
    {
        Console.WriteLine(Math.Round(1.01));
        Console.WriteLine(Math.Round(1.04));
        Console.WriteLine(Math.Round(1.05));
        Console.WriteLine(Math.Round(1.06));
        Console.WriteLine();
        Console.WriteLine(Math.Round(1.41));
        Console.WriteLine(Math.Round(1.44));
        Console.WriteLine(Math.Round(1.45));
        Console.WriteLine(Math.Round(1.46));
        Console.WriteLine();
        Console.WriteLine(Math.Round(1.51));
        Console.WriteLine(Math.Round(1.54));
        Console.WriteLine(Math.Round(1.55));
        Console.WriteLine(Math.Round(1.56));

        Math.Ceiling(1.5);
    }
}
