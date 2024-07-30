using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyCompanyName.MyProjectName.IdentityService.EntityFramework;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 * */
public class IdentityServiceDbContextFactory : IDesignTimeDbContextFactory<IdentityServiceDbContext>
{
    public IdentityServiceDbContext CreateDbContext(string[] args)
    {
        IdentityServiceEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<IdentityServiceDbContext>()
            .UseSqlServer(GetConnectionStringFromConfiguration(), b =>
            {
                b.MigrationsHistoryTable("__IdentityService_Migrations");
            });

        return new IdentityServiceDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration()
            .GetConnectionString(IdentityServiceDbProperties.ConnectionStringName);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    $"..{Path.DirectorySeparatorChar}MyCompanyName.MyProjectName.IdentityService.HttpApi.Host"
                )
            )
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
