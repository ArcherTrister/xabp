// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.IdentityServer;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.IdentityServer.Devices;
using Volo.Abp.IdentityServer.Grants;
using Volo.Abp.IdentityServer.IdentityResources;
using Volo.Abp.MultiTenancy;

namespace X.Abp.IdentityServer.EntityFrameworkCore;

[IgnoreMultiTenancy]
[ConnectionStringName(AbpIdentityServerDbProperties.ConnectionStringName)]
public class IdentityServerProDbContext : AbpDbContext<IdentityServerProDbContext>, IIdentityServerProDbContext
{
    #region ApiResource

    public DbSet<ApiResource> ApiResources { get; set; }

    public DbSet<ApiResourceSecret> ApiResourceSecrets { get; set; }

    public DbSet<ApiResourceClaim> ApiResourceClaims { get; set; }

    public DbSet<ApiResourceScope> ApiResourceScopes { get; set; }

    public DbSet<ApiResourceProperty> ApiResourceProperties { get; set; }

    #endregion

    #region ApiScope

    public DbSet<ApiScope> ApiScopes { get; set; }

    public DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

    public DbSet<ApiScopeProperty> ApiScopeProperties { get; set; }

    #endregion

    #region IdentityResource

    public DbSet<IdentityResource> IdentityResources { get; set; }

    public DbSet<IdentityResourceClaim> IdentityClaims { get; set; }

    public DbSet<IdentityResourceProperty> IdentityResourceProperties { get; set; }

    #endregion

    #region Client

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

    #endregion

    public DbSet<PersistedGrant> PersistedGrants { get; set; }

    public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

    public IdentityServerProDbContext(DbContextOptions<IdentityServerProDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureIdentityServerPro();
    }
}
