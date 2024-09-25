using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace MyCompanyName.MyProjectName.Shared.Hosting.Microservices.DbMigrations;

public abstract class PendingMigrationsCheckerBase : ITransientDependency
{
    private readonly ILogger<PendingMigrationsCheckerBase> _logger;
    protected PendingMigrationsCheckerBase(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PendingMigrationsCheckerBase>();
    }

    public async Task TryAsync(Func<Task> task, int retryCount = 3)
    {
        try
        {
            await task();
        }
        catch (Exception ex)
        {
            retryCount--;

            if (retryCount <= 0)
            {
                throw;
            }

            _logger.LogWarning("{TypeName} has been thrown. The operation will be tried {RetryCount} times more. Exception:\n{Message}", ex.GetType().Name, retryCount, ex.Message);

            await Task.Delay(RandomHelper.GetRandom(5000, 15000));

            await TryAsync(task, retryCount);
        }
    }
}
