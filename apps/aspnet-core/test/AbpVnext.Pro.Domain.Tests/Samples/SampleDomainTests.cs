// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Shouldly;

using Volo.Abp.Identity;

using Xunit;

using IIdentityUserRepository = X.Abp.Identity.IIdentityUserRepository;

namespace AbpVnext.Pro.Samples;

/* This is just an example test class.
 * Normally, you don't test code of the modules you are using
 * (like IdentityUserManager here).
 * Only test your own domain services.
 */
public class SampleDomainTests : ProDomainTestBase
{
    private readonly IIdentityUserRepository _identityUserRepository;
    private readonly IdentityUserManager _identityUserManager;

    public SampleDomainTests()
    {
        _identityUserRepository = GetRequiredService<IIdentityUserRepository>();
        _identityUserManager = GetRequiredService<IdentityUserManager>();
    }

    [Fact]
    public async Task ShouldSetEmailOfAUserAsync()
    {
        IdentityUser adminUser;

        /* Need to manually start Unit Of Work because
         * FirstOrDefaultAsync should be executed while db connection / context is available.
         */
        await WithUnitOfWorkAsync(async () =>
        {
            adminUser = await _identityUserRepository
                .FindByNormalizedUserNameAsync("ADMIN");

            await _identityUserManager.SetEmailAsync(adminUser, "newemail@abp.io");
            await _identityUserRepository.UpdateAsync(adminUser);
        });

        adminUser = await _identityUserRepository.FindByNormalizedUserNameAsync("ADMIN");
        adminUser.Email.ShouldBe("newemail@abp.io");
    }

    public override void Dispose()
    {
        base.Dispose();
        _identityUserManager.Dispose();
        GC.SuppressFinalize(this);
    }
}
