# abp-entityFrameworkCore

entityFrameworkCore module for ABP framework.

## How to install

Install the package from [NuGet](https://www.nuget.org/) or from the `Package Manager Console` :

```powershell
PM> Install-Package X.Abp.EntityFrameworkCore.FieldEncryption
```

## How to use

```csharp
[DependsOn(typeof(AbpEntityFrameworkCoreFieldEncryptionModule))]
public class XXXEntityFrameworkCoreModule : AbpModule
{
}

public class XXXDbContext : AbpDbContext<XXXDbContext>
{
    protected IFieldEncryptionProvider FieldEncryptionProvider => LazyServiceProvider?.LazyGetRequiredService<IFieldEncryptionProvider>();

    public DbSet<XXXEntity> XXXs { get; set; }

    public ConfigurationCenterDbContext(DbContextOptions<ConfigurationCenterDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        modelBuilder.Entity<XXXEntity>().Property(x => x.FieldName).IsRequired().IsEncrypted();

        if (FieldEncryptionProvider is not null)
        {
            builder.UseEncryption(FieldEncryptionProvider);
        }
    }
}
```
