// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Shouldly;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

using Xunit;

namespace AbpVnext.Pro.EntityFrameworkCore.Samples;

/* This is just an example test class.
 * Normally, you don't test ABP framework code
 * Only test your custom repository methods.
 */
public class SampleRepositoryTests : ProEntityFrameworkCoreTestBase
{
    private readonly IRepository<IdentityUser, Guid> _appUserRepository;

    public SampleRepositoryTests()
    {
        _appUserRepository = GetRequiredService<IRepository<IdentityUser, Guid>>();
    }

    [Fact]
    public async Task ShouldQueryAppUser()
    {
        /* Need to manually start Unit Of Work because
         * FirstOrDefaultAsync should be executed while db connection / context is available.
         */
        await WithUnitOfWorkAsync(async () =>
        {
            // Act
            var adminUser = await _appUserRepository
            .FirstOrDefaultAsync(u => u.UserName == "admin");

            // Assert
            adminUser.ShouldNotBeNull();
        });
    }
}
