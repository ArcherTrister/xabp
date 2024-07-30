## How To Add New Migration?

This solution configured so it uses two database schema;

* Default schema is used to store host & tenant data (when the tenant uses shared database).
* Tenant schema is used to store only the tenant data (when the tenant uses a dedicated database).

In this way, dedicated tenant databases do not have host-related empty tables.

To make this possible, there are two migrations `DbContext` in the `EntityFrameworkCore` project. So, you need to specify the `DbContext` when you want to add new migration.

> When you add/change a multi-tenant entity (that implements IMultiTenant) in your project, you typically need to add two migrations: one for the default DbContext and the other one is for the tenant  DbContext. If you are making change for a host-only entity, then you don't need to add-migration for the tenant DbContext (if you add, you will get an empty migration file).

### Example: Adding Migration to the Default DbContext

Using Visual Studio Package Manager Console;

````bash
Add-Migration Your_Migration_Name -Context ProDbContext
````

Using EF Core command line tool;

````bash
dotnet ef migrations add Your_Migration_Name --context ProDbContext
````

### Example: Adding Migration to the Tenant DbContext

Using Visual Studio Package Manager Console:

````bash
Add-Migration Your_Migration_Name -Context ProTenantDbContext -OutputDir TenantMigrations
````

Using EF Core command line tool:

````bash
dotnet ef migrations add Your_Migration_Name --context ProTenantDbContext --output-dir TenantMigrations
````

## Updating the Databases

It is suggested to run the `DbMigrator` application to update the database after adding a new migration. It is simpler and also automatically handles tenant database upgrades.
