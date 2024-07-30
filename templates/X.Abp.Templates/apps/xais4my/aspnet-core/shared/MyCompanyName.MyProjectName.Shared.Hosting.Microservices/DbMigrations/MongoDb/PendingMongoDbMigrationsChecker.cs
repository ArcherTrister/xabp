/*
 * Un-comment this MongoDb Migrations Checker class if you are using MongoDB in any of your microservice.
 * You need to inherit from this DbMigrationChecker in order to use service-specific MongoDb provider
 */

// using System;
// using System.Threading.Tasks;
// using Microsoft.Extensions.DependencyInjection;
// using MongoDB.Driver;
// using Serilog;
// using Volo.Abp.Data;
// using Volo.Abp.DistributedLocking;
// using Volo.Abp.EventBus.Distributed;
// using Volo.Abp.MongoDB;
// using Volo.Abp.MultiTenancy;
// using Volo.Abp.Uow;
//
// namespace MyCompanyName.MyProjectName.Shared.Hosting.Microservices.DbMigrations.MongoDb;

// public abstract class PendingMongoDbMigrationsChecker<TDbContext> : PendingMigrationsCheckerBase where TDbContext : AbpMongoDbContext
// {
//     protected IUnitOfWorkManager UnitOfWorkManager { get; }
//     protected IServiceProvider ServiceProvider { get; }
//     protected ICurrentTenant CurrentTenant { get; }
//     protected IDistributedEventBus DistributedEventBus { get; }
//     protected IAbpDistributedLock DistributedLockProvider { get; }
//     protected string DatabaseName { get; }
//     protected IDataSeeder DataSeeder { get; }
//
//     protected PendingMongoDbMigrationsChecker(
//         IUnitOfWorkManager unitOfWorkManager,
//         IServiceProvider serviceProvider,
//         ICurrentTenant currentTenant,
//         IDistributedEventBus distributedEventBus,
//         IAbpDistributedLock abpDistributedLock,
//         string databaseName,
//         IDataSeeder dataSeeder)
//     {
//         UnitOfWorkManager = unitOfWorkManager;
//         ServiceProvider = serviceProvider;
//         CurrentTenant = currentTenant;
//         DistributedEventBus = distributedEventBus;
//         DistributedLockProvider = abpDistributedLock;
//         DatabaseName = databaseName;
//         DataSeeder = dataSeeder;
//     }
//
//     public virtual async Task CheckAndApplyDatabaseMigrationsAsync()
//     {
//         await TryAsync(async () =>
//         {
//             using (CurrentTenant.Change(null))
//             {
//                 // Create database tables if needed
//                 using (var uow = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
//                 {
//                     await MigrateDatabaseSchemaAsync();
//
//                     await DataSeeder.SeedAsync();
//
//                     await uow.CompleteAsync();
//                 }
//             }
//         });
//     }
//     /// <summary>
//     /// Apply scheme update for MongoDB Database.
//     /// </summary>
//     protected virtual async Task<bool> MigrateDatabaseSchemaAsync()
//     {
//         var result = false;
//         await using (var handle = await DistributedLockProvider.TryAcquireAsync("Migration_" + DatabaseName))
//         {
//             using (var uow = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
//             {
//                 Log.Information($"Lock is acquired for db migration and seeding on database named: {DatabaseName}...");
//
//                 if (handle is null)
//                 {
//                     Log.Information($"Handle is null because of the locking for : {DatabaseName}");
//                     return false;
//                 }
//
//                 async Task<bool> MigrateDatabaseSchemaWithDbContextAsync()
//                 {
//                     var dbContexts = ServiceProvider.GetServices<IAbpMongoDbContext>();
//                     var connectionStringResolver = ServiceProvider.GetRequiredService<IConnectionStringResolver>();
//
//                     foreach (var dbContext in dbContexts)
//                     {
//                         var connectionString =
//                             await connectionStringResolver.ResolveAsync(
//                                 ConnectionStringNameAttribute.GetConnStringName(dbContext.GetType()));
//                         if (connectionString.IsNullOrWhiteSpace())
//                         {
//                             continue;
//                         }
//
//                         var mongoUrl = new MongoUrl(connectionString);
//                         var databaseName = mongoUrl.DatabaseName;
//                         var client = new MongoClient(mongoUrl);
//
//                         if (databaseName.IsNullOrWhiteSpace())
//                         {
//                             databaseName = ConnectionStringNameAttribute.GetConnStringName(dbContext.GetType());
//                         }
//
//                         (dbContext as AbpMongoDbContext)?.InitializeCollections(client.GetDatabase(databaseName));
//                     }
//
//                     return true;
//                 }
//
//                 //Migrating the host database
//                 result = await MigrateDatabaseSchemaWithDbContextAsync();
//
//                 await uow.CompleteAsync();
//             }
//
//             return result;
//         }
//     }
// }
