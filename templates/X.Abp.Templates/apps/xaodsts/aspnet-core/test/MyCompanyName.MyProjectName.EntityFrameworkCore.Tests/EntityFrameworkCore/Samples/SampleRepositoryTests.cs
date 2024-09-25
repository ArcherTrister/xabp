// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

using Shouldly;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

using Xunit;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore.Samples;

/* This is just an example test class.
 * Normally, you don't test ABP framework code
 * Only test your custom repository methods.
 */
public class SampleRepositoryTests : MyProjectNameEntityFrameworkCoreTestBase
{
    private readonly IRepository<IdentityUser, Guid> _appUserRepository;

    public SampleRepositoryTests()
    {
        _appUserRepository = GetRequiredService<IRepository<IdentityUser, Guid>>();
    }

    [Fact]
    public async Task Should_Query_AppUser()
    {
        /* Need to manually start Unit Of Work because
         * FirstOrDefaultAsync should be executed while db connection / context is available.
         */
        await WithUnitOfWorkAsync(async () =>
        {
            //Act
            var adminUser = await _appUserRepository
            .FirstOrDefaultAsync(u => u.UserName == "admin");

            //Assert
            adminUser.ShouldNotBeNull();
        });
    }
}
