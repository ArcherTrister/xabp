using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.Identity;
using X.Abp.Identity.EntityFrameworkCore;
using X.Abp.IdentityServer.ApiResources;
using X.Abp.IdentityServer.ApiScopes;
using X.Abp.IdentityServer.Clients;
using X.Abp.IdentityServer.Devices;
using X.Abp.IdentityServer.EntityFrameworkCore;
using X.Abp.IdentityServer.Grants;
using X.Abp.IdentityServer.IdentityResources;

namespace MyCompanyName.MyProjectName.IdentityService.EntityFramework;

[ConnectionStringName(IdentityServiceDbProperties.ConnectionStringName)]
public class IdentityServiceDbContext : AbpDbContext<IdentityServiceDbContext>, IIdentityProDbContext, IIdentityServerProDbContext
{
    public DbSet<IdentityUser> Users { get; set; }

    public DbSet<IdentityRole> Roles { get; set; }

    public DbSet<IdentityClaimType> ClaimTypes { get; set; }

    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }

    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }

    public DbSet<IdentityLinkUser> LinkUsers { get; set; }

    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    public DbSet<ApiResource> ApiResources { get; set; }

    public DbSet<ApiResourceSecret> ApiResourceSecrets { get; set; }

    public DbSet<ApiResourceClaim> ApiResourceClaims { get; set; }

    public DbSet<ApiResourceScope> ApiResourceScopes { get; set; }

    public DbSet<ApiResourceProperty> ApiResourceProperties { get; set; }

    public DbSet<ApiScope> ApiScopes { get; set; }

    public DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

    public DbSet<ApiScopeProperty> ApiScopeProperties { get; set; }

    public DbSet<IdentityResource> IdentityResources { get; set; }

    public DbSet<IdentityResourceClaim> IdentityClaims { get; set; }

    public DbSet<IdentityResourceProperty> IdentityResourceProperties { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<ClientGrantType> ClientGrantTypes { get; set; }

    public DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }

    public DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }

    public DbSet<ClientScope> ClientScopes { get; set; }

    public DbSet<ClientSecret> ClientSecrets { get; set; }

    public DbSet<ClientClaim> ClientClaims { get; set; }

    public DbSet<ClientIdPRestriction> ClientIdPRestrictions { get; set; }

    public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }

    public DbSet<ClientProperty> ClientProperties { get; set; }

    public DbSet<PersistedGrant> PersistedGrants { get; set; }

    public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

    public IdentityServiceDbContext(DbContextOptions<IdentityServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureIdentityPro();
        modelBuilder.ConfigureIdentityServerPro();

        /* Define mappings for your custom entities here...
        modelBuilder.Entity<MyEntity>(b =>
        {
            b.ToTable(IdentityServiceDbProperties.DbTablePrefix + "MyEntities", IdentityServiceDbProperties.DbSchema);
            b.ConfigureByConvention();
            //TODO: Configure other properties, indexes... etc.
        });
        */
    }
}
