// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using MongoDB.Driver;

using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.MongoDB;

namespace X.Abp.Identity.MongoDB;

[ConnectionStringName(AbpIdentityDbProperties.ConnectionStringName)]
public class IdentityProMongoDbContext : AbpMongoDbContext, IIdentityProMongoDbContext
{
    /* Add mongo collections here. Example:
    * public IMongoCollection<Question> Questions => Collection<Question>();
    */
    public IMongoCollection<IdentityUser> Users => Collection<IdentityUser>();

    public IMongoCollection<IdentityRole> Roles => Collection<IdentityRole>();

    public IMongoCollection<IdentityClaimType> ClaimTypes => Collection<IdentityClaimType>();

    public IMongoCollection<OrganizationUnit> OrganizationUnits => Collection<OrganizationUnit>();

    public IMongoCollection<IdentitySecurityLog> SecurityLogs => Collection<IdentitySecurityLog>();

    public IMongoCollection<IdentityLinkUser> LinkUsers => Collection<IdentityLinkUser>();

    public IMongoCollection<IdentityUserDelegation> UserDelegations => Collection<IdentityUserDelegation>();

    public IMongoCollection<IdentitySession> Sessions => Collection<IdentitySession>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureIdentityPro();
    }
}
