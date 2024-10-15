// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace X.Abp.Identity;

public class IdentityDataSeeder : IIdentityDataSeeder, ITransientDependency
{
    protected IGuidGenerator GuidGenerator { get; }

    protected IIdentityRoleRepository RoleRepository { get; }

    protected IIdentityUserRepository UserRepository { get; }

    protected ILookupNormalizer LookupNormalizer { get; }

    protected IdentityUserManager UserManager { get; }

    protected IdentityRoleManager RoleManager { get; }

    protected ICurrentTenant CurrentTenant { get; }

    protected IOptions<IdentityOptions> IdentityOptions { get; }

    public IdentityDataSeeder(
        IGuidGenerator guidGenerator,
        IIdentityRoleRepository roleRepository,
        IIdentityUserRepository userRepository,
        ILookupNormalizer lookupNormalizer,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        ICurrentTenant currentTenant,
        IOptions<IdentityOptions> identityOptions)
    {
        GuidGenerator = guidGenerator;
        RoleRepository = roleRepository;
        UserRepository = userRepository;
        LookupNormalizer = lookupNormalizer;
        UserManager = userManager;
        RoleManager = roleManager;
        CurrentTenant = currentTenant;
        IdentityOptions = identityOptions;
    }

    [UnitOfWork]
    public virtual async Task<IdentityDataSeedResult> SeedAsync(
        string adminEmail,
        string adminPassword,
        Guid? tenantId = null,
        string adminUserName = null)
    {
        Check.NotNullOrWhiteSpace(adminEmail, nameof(adminEmail));
        Check.NotNullOrWhiteSpace(adminPassword, nameof(adminPassword));

        using (CurrentTenant.Change(tenantId))
        {
            await IdentityOptions.SetAsync();

            var result = new IdentityDataSeedResult();

            // "admin" user
            if (adminUserName.IsNullOrWhiteSpace())
            {
                adminUserName = IdentityDataSeedContributor.AdminUserNameDefaultValue;
            }

            var adminUser = await UserRepository.FindByNormalizedUserNameAsync(
                LookupNormalizer.NormalizeName(adminUserName));

            if (adminUser != null)
            {
                return result;
            }

            adminUser = new IdentityUser(
                GuidGenerator.Create(),
                adminUserName,
                adminEmail,
                tenantId)
            {
                Name = adminUserName
            };

            (await UserManager.CreateAsync(adminUser, adminPassword, validatePassword: false)).CheckIdentityErrors();
            result.CreatedAdminUser = true;

            // "admin" role
            const string adminRoleName = "admin";
            var adminRole =
                await RoleRepository.FindByNormalizedNameAsync(LookupNormalizer.NormalizeName(adminRoleName));
            if (adminRole == null)
            {
                adminRole = new IdentityRole(
                    GuidGenerator.Create(),
                    adminRoleName,
                    tenantId)
                {
                    IsStatic = true,
                    IsPublic = true
                };

                (await RoleManager.CreateAsync(adminRole)).CheckIdentityErrors();
                result.CreatedAdminRole = true;
            }

            (await UserManager.AddToRoleAsync(adminUser, adminRoleName)).CheckIdentityErrors();

            return result;
        }
    }
}
