// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

using Quartz;
using Quartz.Impl.AdoJobStore;
using Quartz.Simpl;
using Quartz.Spi;
using Quartz.Util;

using Volo.Abp.Application.Dtos;

using X.Abp.Quartz.Histories.Dtos;

namespace X.Abp.Quartz.Plugins.ExecutionHistory;

public class JobHistoryStore
{
  private StdAdoDelegate _driverDelegate;

  private Type _delegateType = typeof(StdAdoDelegate);

  protected ITypeLoadHelper TypeLoadHelper { get; private set; }

  private readonly ConcurrentDictionary<long, ExecutionHistoryEntry> histories = new();

  protected string DataSource { get; }

  protected string DelegateTypeName { get; }

  protected string TablePrefix { get; }

  protected string Provider { get; }

  public JobHistoryStore(string dataSource, string delegateTypeName, string tablePrefix, string provider)
  {
    DataSource = dataSource;
    DelegateTypeName = delegateTypeName;
    TablePrefix = tablePrefix ?? AdoConstants.DefaultTablePrefix;
    Provider = provider;
    TypeLoadHelper = new SimpleTypeLoadHelper();
    TypeLoadHelper.Initialize();
  }

  private const string PropertySqlInsertJobExecuted = "INSERT INTO {0}JOB_HISTORY (SCHED_NAME, INSTANCE_NAME, TRIGGER_NAME, TRIGGER_GROUP, JOB_NAME, JOB_GROUP, SCHED_TIME, FIRED_TIME, RUN_TIME, ERROR, ERROR_MESSAGE)  VALUES (@schedulerName, @instanceName, @triggerName, @triggerGroup, @jobName, @jobGroup, @scheduledTime, @firedTime, @runTime, @error, @errorMessage)";

  public virtual async Task CreateJobHistoryEntryAsync(
      IJobExecutionContext context,
      JobExecutionException jobException,
      CancellationToken cancellationToken = default)
  {
    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
    {
      lock (histories)
      {
        if (histories.Count >= 500)
        {
          histories.Clear();
        }

        var entry = new ExecutionHistoryEntry()
        {
          EntryId = histories.Count + 1,
          FireInstanceId = context.FireInstanceId,
          SchedulerInstanceId = context.Scheduler.SchedulerInstanceId,
          SchedulerName = context.Scheduler.SchedulerName,
          ActualFireTimeUtc = context.FireTimeUtc.UtcDateTime,
          ScheduledFireTimeUtc = context.ScheduledFireTimeUtc?.UtcDateTime,
          Recovering = context.Recovering,
          JobName = context.JobDetail.Key.ToString(),
          TriggerName = context.Trigger.Key.ToString(),
          Error = jobException != null,
          ErrorMessage = jobException?.ToString(),
        };

        histories.TryAdd(entry.EntryId, entry);
      }
    }
    else
    {
      var sql = AdoJobStoreUtil.ReplaceTablePrefix(PropertySqlInsertJobExecuted, TablePrefix);

      using var connection = GetConnection(IsolationLevel.ReadUncommitted);
      using var command = DbAccessor.PrepareCommand(connection, sql);

      DbAccessor.AddCommandParameter(command, "schedulerName", context.Scheduler.SchedulerName);
      DbAccessor.AddCommandParameter(command, "instanceName", context.Scheduler.SchedulerInstanceId);
      DbAccessor.AddCommandParameter(command, "jobName", context.JobDetail.Key.Name);
      DbAccessor.AddCommandParameter(command, "jobGroup", context.JobDetail.Key.Group);
      DbAccessor.AddCommandParameter(command, "triggerName", context.Trigger.Key.Name);
      DbAccessor.AddCommandParameter(command, "triggerGroup", context.Trigger.Key.Group);
      DbAccessor.AddCommandParameter(command, "scheduledTime", DbAccessor.GetDbDateTimeValue(context.ScheduledFireTimeUtc));
      DbAccessor.AddCommandParameter(command, "firedTime", DbAccessor.GetDbDateTimeValue(context.FireTimeUtc));
      DbAccessor.AddCommandParameter(command, "runTime", DbAccessor.GetDbTimeSpanValue(context.JobRunTime));
      DbAccessor.AddCommandParameter(command, "error", DbAccessor.GetDbBooleanValue(jobException != null));
      DbAccessor.AddCommandParameter(command, "errorMessage", jobException?.ToString());

      await command.ExecuteNonQueryAsync(cancellationToken);
      connection.Commit(false);
    }
  }

  /*
          //private const string PropertySqlUpdateHistoryError = "UPDATE {0}JOB_HISTORY SET Vetoed = @vetoed WHERE Fire_Instance_Id = @fireInstanceId";

          //public virtual async Task UpdateJobHistoryEntryErrorAsync(
          //    IJobExecutionContext context,
          //    JobExecutionException jobException,
          //    CancellationToken cancellationToken = default)
          //{
          //    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
          //    {
          //    }
          //    else
          //    {
          //        string sql = Extensions.ReplaceTablePrefix(PropertySqlUpdateHistoryError, TablePrefix);

          //        using (ConnectionAndTransactionHolder connection = GetConnection(IsolationLevel.ReadUncommitted))
          //        {
          //            using (DbCommand command = Delegate.PrepareCommand(connection, sql))
          //            {
          //                Delegate.AddCommandParameter(command, "error", Delegate.GetDbBooleanValue(jobException != null));
          //                Delegate.AddCommandParameter(command, "errorMessage", jobException?.GetBaseException()?.Message);
          //                Delegate.AddCommandParameter(command, "fireInstanceId", context.FireInstanceId);

          //                await command.ExecuteNonQueryAsync(cancellationToken);
          //                connection.Commit(false);
          //            }
          //        }
          //    }
          //}

          //private const string PropertySqlUpdateHistoryVetoed = "UPDATE {0}JOB_HISTORY SET Vetoed = @vetoed WHERE Fire_Instance_Id = @fireInstanceId";

          //public virtual async Task UpdateJobHistoryEntryVetoedAsync(
          //    IJobExecutionContext context,
          //    CancellationToken cancellationToken = default)
          //{
          //    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
          //    {

          //    }
          //    else
          //    {
          //        string sql = Extensions.ReplaceTablePrefix(PropertySqlUpdateHistoryVetoed, TablePrefix);

          //        using (ConnectionAndTransactionHolder connection = GetConnection(IsolationLevel.ReadUncommitted))
          //        {
          //            using (DbCommand command = Delegate.PrepareCommand(connection, sql))
          //            {
          //                Delegate.AddCommandParameter(command, "vetoed", true);
          //                Delegate.AddCommandParameter(command, "fireInstanceId", context.FireInstanceId);

          //                await command.ExecuteNonQueryAsync(cancellationToken);
          //                connection.Commit(false);
          //            }
          //        }
          //    }
          //}
          */
  private const string PropertySqlSelectHistoryCount = "SELECT COUNT(1) FROM {0}JOB_HISTORY WHERE 1=1 AND SCHED_NAME = @schedulerName";
  /*
          //public virtual async Task<long> GetAllCountAsync(string schedulerName, CancellationToken cancellationToken = default)
          //{
          //    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
          //    {
          //        var result = histories.Values.Where(p => p.SchedulerName.Equals(schedulerName)).LongCount();
          //        return await Task.FromResult(result);
          //    }
          //    else
          //    {
          //        using (ConnectionAndTransactionHolder dbConnection = GetConnection(IsolationLevel.ReadUncommitted))
          //        {
          //            string sqlCount = Extensions.ReplaceTablePrefix(PropertySqlSelectHistoryCount, TablePrefix);

          //            using (DbCommand dbCommand = Delegate.PrepareCommand(dbConnection, sqlCount))
          //            {
          //                Delegate.AddCommandParameter(dbCommand, "schedulerName", schedulerName);
          //                return (long)await dbCommand.ExecuteScalarAsync(cancellationToken);
          //            }
          //        }
          //    }
          //}
  */
  private const string PropertySqlSelectOneHistory = "SELECT [ENTRY_ID],[SCHED_NAME]" +
      ",[INSTANCE_NAME],[TRIGGER_NAME],[TRIGGER_GROUP],[JOB_NAME],[JOB_GROUP]" +
      ",[FIRED_TIME],[SCHED_TIME],[RUN_TIME],[ERROR],[ERROR_MESSAGE]" +
      "FROM {0}JOB_HISTORY WHERE ENTRY_ID = @entryId";

  public virtual async Task<ExecutionHistoryDto> GetJobHistoryEntryAsync(long entryId, CancellationToken cancellationToken = default)
  {
    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
    {
      histories.TryGetValue(entryId, out var entry);
      return await Task.FromResult(entry == null ? null : new ExecutionHistoryDto
      {
        EntryId = entry.EntryId,
        Error = entry.Error,
        ErrorMessage = entry.ErrorMessage,
        FiredTime = entry.FiredTime,
        JobGroup = entry.JobGroup,
        JobName = entry.JobName,
        RunTime = entry.RunTime,
        ScheduledTime = entry.ScheduledTime,
        TriggerGroup = entry.TriggerGroup,
        TriggerName = entry.TriggerName,
      });
    }
    else
    {
      using var dbConnection = GetConnection(IsolationLevel.ReadUncommitted);
      var sql = AdoJobStoreUtil.ReplaceTablePrefix(PropertySqlSelectOneHistory, TablePrefix);

      using var dbCommand = DbAccessor.PrepareCommand(dbConnection, sql);
      DbAccessor.AddCommandParameter(dbCommand, nameof(entryId), entryId);

      using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
      var list = new List<ExecutionHistoryDto>();
      while (await reader.ReadAsync(cancellationToken))
      {
        var entry = new ExecutionHistoryDto
        {
          EntryId = Convert.ToInt64(reader["ENTRY_ID"]),

          // Recovering = Delegate.GetBooleanFromDbValue(reader["Recovering"]),
          // Vetoed = Delegate.GetBooleanFromDbValue(reader["Vetoed"]),
          // ActualFireTimeUtc = Convert.ToDateTime(reader["Actual_Fire_Time_Utc"]),
          // FinishedTimeUtc = Convert.ToDateTime(reader["Finished_Time_Utc"]),
          // FireInstanceId = reader.GetString("Fire_Instance_Id"),
          // ScheduledFireTimeUtc = Convert.ToDateTime(reader["Scheduled_Fire_Time_Utc"]),
          // SchedulerInstanceId = reader.GetString("Scheduler_Instance_Id"),
          // SchedulerName = reader.GetString("SCHED_NAME"),                                    JobName = reader.GetString("JOB_NAME"),
          JobGroup = reader.GetString("JOB_GROUP"),
          TriggerName = reader.GetString("TRIGGER_NAME"),
          TriggerGroup = reader.GetString("TRIGGER_GROUP"),
          FiredTime = DbAccessor.GetDateTimeFromDbValue(reader["FIRED_TIME"]).GetValueOrDefault(),
          ScheduledTime = DbAccessor.GetDateTimeFromDbValue(reader["SCHED_TIME"]).GetValueOrDefault(),
          RunTime = DbAccessor.GetTimeSpanFromDbValue(reader["RUN_TIME"]).GetValueOrDefault(),
          Error = DbAccessor.GetBooleanFromDbValue(reader["ERROR"]),
          ErrorMessage = reader.GetString("ERROR_MESSAGE")
        };
        list.Add(entry);
      }

      return list.FirstOrDefault();
    }
  }

  // private const string PropertySqlServerSelectHistoryEntryPage =
  //    "SELECT ENTRY_ID, SCHED_NAME, Scheduler_Instance_Id, Fire_Instance_Id, Scheduled_Fire_Time_Utc, Actual_Fire_Time_Utc, Finished_Time_Utc, Recovering,Vetoed, " +
  //    "TRIGGER_NAME, TRIGGER_GROUP, JOB_NAME, JOB_GROUP, FIRED_TIME, SCHED_TIME, RUN_TIME, ERROR, ERROR_MESSAGE FROM (SELECT *, ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber FROM {0}JOB_HISTORY ) as B " +
  //    "WHERE 1=1 AND SCHED_NAME = @schedulerName AND RowNumber BETWEEN @page AND @endPage";

  // private const string PropertyMySqlSelectHistoryEntryPage =
  //    "SELECT ENTRY_ID, SCHED_NAME, Scheduler_Instance_Id, Fire_Instance_Id, Scheduled_Fire_Time_Utc, Actual_Fire_Time_Utc, Finished_Time_Utc, Recovering, Vetoed, " +
  //    "TRIGGER_NAME, TRIGGER_GROUP, JOB_NAME, JOB_GROUP, FIRED_TIME, SCHED_TIME, RUN_TIME, ERROR, ERROR_MESSAGE FROM {0}JOB_HISTORY " +
  //    "WHERE 1=1 AND SCHED_NAME = @schedulerName ORDER BY {1} LIMIT @page, @pageSize";
  private const string PropertySqlServerSelectHistoryEntryPage =
"SELECT ENTRY_ID, SCHED_NAME, INSTANCE_NAME, TRIGGER_NAME, TRIGGER_GROUP, JOB_NAME, JOB_GROUP, FIRED_TIME, SCHED_TIME, RUN_TIME, ERROR, ERROR_MESSAGE FROM (SELECT *, ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber FROM {0}JOB_HISTORY ) as B " +
"WHERE 1=1 AND SCHED_NAME = @schedulerName AND RowNumber BETWEEN @page AND @endPage";

  private const string PropertyMySqlSelectHistoryEntryPage =
      "SELECT ENTRY_ID, SCHED_NAME, INSTANCE_NAME, TRIGGER_NAME, TRIGGER_GROUP, JOB_NAME, JOB_GROUP, FIRED_TIME, SCHED_TIME, RUN_TIME, ERROR, ERROR_MESSAGE FROM {0}JOB_HISTORY " +
      "WHERE 1=1 AND SCHED_NAME = @schedulerName ORDER BY {1} LIMIT @page, @pageSize";

  public virtual async Task<PagedResultDto<ExecutionHistoryDto>> GetPageJobHistoryEntriesAsync(
      string schedulerName,
      int skipCount,
      int maxResultCount,
      string sorting,
      CancellationToken cancellationToken = default)
  {
    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
    {
      var query = histories.Values.AsQueryable();
      var count = query.WhereIf(!string.IsNullOrEmpty(schedulerName), q => q.SchedulerName.Equals(schedulerName, StringComparison.OrdinalIgnoreCase)).LongCount();
      var list = query.WhereIf(!string.IsNullOrEmpty(schedulerName), q => q.SchedulerName.Equals(schedulerName, StringComparison.OrdinalIgnoreCase)).MultiOrderBy(string.IsNullOrWhiteSpace(sorting) ? "firedTime desc" : sorting).Take(maxResultCount).Skip(skipCount).ToList();
      var result = new PagedResultDto<ExecutionHistoryDto>
      {
        TotalCount = count,
        Items = list.Select(entry => new ExecutionHistoryDto
        {
          EntryId = entry.EntryId,
          Error = entry.Error,
          ErrorMessage = entry.ErrorMessage,
          FiredTime = entry.FiredTime,
          JobGroup = entry.JobGroup,
          JobName = entry.JobName,
          RunTime = entry.RunTime,
          ScheduledTime = entry.ScheduledTime,
          TriggerGroup = entry.TriggerGroup,
          TriggerName = entry.TriggerName,
        }).ToList()
      };
      return await Task.FromResult(result);
    }
    else
    {
      sorting = string.IsNullOrWhiteSpace(sorting)
          ? "FIRED_TIME desc"
          : sorting.Replace("errorMessage", "ERROR_MESSAGE", StringComparison.OrdinalIgnoreCase)
              .Replace("error", "ERROR", StringComparison.OrdinalIgnoreCase)
              .Replace("runTime", "RUN_TIME", StringComparison.OrdinalIgnoreCase)
              .Replace("scheduledTime", "SCHED_TIME", StringComparison.OrdinalIgnoreCase)
              .Replace("firedTime", "FIRED_TIME", StringComparison.OrdinalIgnoreCase)
              .Replace("triggerGroup", "TRIGGER_GROUP", StringComparison.OrdinalIgnoreCase)
              .Replace("triggerName", "TRIGGER_NAME", StringComparison.OrdinalIgnoreCase)
              .Replace("jobGroup", "JOB_GROUP", StringComparison.OrdinalIgnoreCase)
              .Replace("jobName", "JOB_NAME", StringComparison.OrdinalIgnoreCase);

      var pageResult = new PagedResultDto<ExecutionHistoryDto>();

      using (var dbConnection = GetConnection(IsolationLevel.ReadUncommitted))
      {
        var sqlCount = AdoJobStoreUtil.ReplaceTablePrefix(PropertySqlSelectHistoryCount, TablePrefix);
        sqlCount = sqlCount.ReplaceTablePrefix(schedulerName);

        using (var dbCommand = DbAccessor.PrepareCommand(dbConnection, sqlCount))
        {
          if (!string.IsNullOrWhiteSpace(schedulerName))
          {
            DbAccessor.AddCommandParameter(dbCommand, nameof(schedulerName), schedulerName);
          }

          var count = await dbCommand.ExecuteScalarAsync(cancellationToken);
          pageResult.TotalCount = Convert.ToInt64(count);
        }

        var sql = string.Format(PropertySqlServerSelectHistoryEntryPage, TablePrefix, sorting);

        if (Provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
        {
          sql = string.Format(PropertyMySqlSelectHistoryEntryPage, TablePrefix, sorting);
        }

        sql = sql.ReplaceTablePrefix(schedulerName);

        using (var dbCommand = DbAccessor.PrepareCommand(dbConnection, sql))
        {
          if (!string.IsNullOrWhiteSpace(schedulerName))
          {
            DbAccessor.AddCommandParameter(dbCommand, nameof(schedulerName), schedulerName);
          }

          DbAccessor.AddCommandParameter(dbCommand, "page", skipCount);

          if (Provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
          {
            // pageSize
            DbAccessor.AddCommandParameter(dbCommand, "pageSize", maxResultCount);
          }
          else
          {
            // endPage
            DbAccessor.AddCommandParameter(dbCommand, "endPage", skipCount + maxResultCount);
          }

          using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
          var list = new List<ExecutionHistoryDto>();
          while (await reader.ReadAsync(cancellationToken))
          {
            var entry = new ExecutionHistoryDto
            {
              EntryId = Convert.ToInt64(reader["ENTRY_ID"]),

              // Recovering = Delegate.GetBooleanFromDbValue(reader["Recovering"]),
              // Vetoed = Delegate.GetBooleanFromDbValue(reader["Vetoed"]),
              // ActualFireTimeUtc = Convert.ToDateTime(reader["Actual_Fire_Time_Utc"]),
              // FinishedTimeUtc = Convert.ToDateTime(reader["Finished_Time_Utc"]),
              // FireInstanceId = reader.GetString("Fire_Instance_Id"),
              // ScheduledFireTimeUtc = Convert.ToDateTime(reader["Scheduled_Fire_Time_Utc"]),
              // SchedulerInstanceId = reader.GetString("Scheduler_Instance_Id"),
              // SchedulerName = reader.GetString("SCHED_NAME"),
              JobName = reader.GetString("JOB_NAME"),
              JobGroup = reader.GetString("JOB_GROUP"),
              TriggerName = reader.GetString("TRIGGER_NAME"),
              TriggerGroup = reader.GetString("TRIGGER_GROUP"),
              FiredTime = DbAccessor.GetDateTimeFromDbValue(reader["FIRED_TIME"]).GetValueOrDefault(),
              ScheduledTime = DbAccessor.GetDateTimeFromDbValue(reader["SCHED_TIME"]).GetValueOrDefault(),
              RunTime = DbAccessor.GetTimeSpanFromDbValue(reader["RUN_TIME"]).GetValueOrDefault(),
              Error = DbAccessor.GetBooleanFromDbValue(reader["ERROR"]),
              ErrorMessage = reader.GetString("ERROR_MESSAGE")
            };
            list.Add(entry);
          }

          pageResult.Items = list.AsReadOnly();
        }
      }

      return pageResult;
    }
  }

  /*
  //private const string PropertySqlServerSelectHistoryGroupByJob = "SELECT TOP @limit * FROM {0}JOB_HISTORY WHERE SCHED_NAME = @schedulerName GROUP BY JOB_NAME ORDER BY Actual_Fire_Time_Utc desc";
  //private const string PropertyMySqlSelectHistoryGroupByJob = "SELECT * FROM {0}JOB_HISTORY WHERE SCHED_NAME = @schedulerName GROUP BY JOB_NAME ORDER BY Actual_Fire_Time_Utc desc limit @limit";

  //public virtual async Task<IEnumerable<ExecutionHistoryEntry>> FilterLastOfEveryJobAsync(string schedulerName, int limitPerJob, CancellationToken cancellationToken = default)
  //{
  //    List<ExecutionHistoryEntry> list = new List<ExecutionHistoryEntry>();
  //    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
  //    {

  //    }
  //    else
  //    {
  //        using (ConnectionAndTransactionHolder dbConnection = GetConnection(IsolationLevel.ReadUncommitted))
  //        {
  //            string sql = string.Format(PropertySqlServerSelectHistoryGroupByJob, TablePrefix);

  //            if (Provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
  //            {
  //                sql = string.Format(PropertyMySqlSelectHistoryGroupByJob, TablePrefix);
  //            }

  //            using (DbCommand dbCommand = Delegate.PrepareCommand(dbConnection, sql))
  //            {
  //                Delegate.AddCommandParameter(dbCommand, "schedulerName", schedulerName);

  //                Delegate.AddCommandParameter(dbCommand, "limit", limitPerJob);

  //                using (DbDataReader reader = await dbCommand.ExecuteReaderAsync(cancellationToken))
  //                {
  //                    while (await reader.ReadAsync(cancellationToken))
  //                    {
  //                        var entry = new ExecutionHistoryEntry
  //                        {
  //                            Recovering = Delegate.GetBooleanFromDbValue(reader["Recovering"]),
  //                            Vetoed = Delegate.GetBooleanFromDbValue(reader["Vetoed"]),
  //                            ActualFireTimeUtc = Convert.ToDateTime(reader["Actual_Fire_Time_Utc"]),
  //                            FinishedTimeUtc = Convert.ToDateTime(reader["Finished_Time_Utc"]),
  //                            FireInstanceId = reader.GetString("Fire_Instance_Id"),
  //                            ScheduledFireTimeUtc = Convert.ToDateTime(reader["Scheduled_Fire_Time_Utc"]),
  //                            SchedulerInstanceId = reader.GetString("Scheduler_Instance_Id"),
  //                            SchedulerName = reader.GetString("SCHED_NAME"),
  //                            JobName = reader.GetString("JOB_NAME"),
  //                            JobGroup = reader.GetString("JOB_GROUP"),
  //                            TriggerName = reader.GetString("TRIGGER_NAME"),
  //                            TriggerGroup = reader.GetString("TRIGGER_GROUP"),
  //                            FiredTime = Delegate.GetDateTimeFromDbValue(reader["FIRED_TIME"]).GetValueOrDefault(),
  //                            ScheduledTime = Delegate.GetDateTimeFromDbValue(reader["SCHED_TIME"]).GetValueOrDefault(),
  //                            RunTime = Delegate.GetTimeSpanFromDbValue(reader["RUN_TIME"]).GetValueOrDefault(),
  //                            Error = Delegate.GetBooleanFromDbValue(reader["ERROR"]),
  //                            ErrorMessage = reader.GetString("ERROR_MESSAGE")
  //                        };
  //                        list.Add(entry);
  //                    }
  //                }
  //            }
  //        }

  //        list.Reverse();
  //    }

  //    return list;
  //}

  //private const string PropertySqlServerSelectHistoryGroupByTrigger = "SELECT TOP @limit * FROM {0}JOB_HISTORY WHERE SCHED_NAME = @schedulerName GROUP BY TRIGGER_NAME ORDER BY Actual_Fire_Time_Utc desc";
  //private const string PropertyMySqlSelectHistoryGroupByTrigger = "SELECT * FROM {0}JOB_HISTORY WHERE SCHED_NAME = @schedulerName GROUP BY TRIGGER_NAME ORDER BY Actual_Fire_Time_Utc desc limit @limit";

  //public virtual async Task<IEnumerable<ExecutionHistoryEntry>> FilterLastOfEveryTriggerAsync(string schedulerName, int limitPerTrigger, CancellationToken cancellationToken = default)
  //{
  //    List<ExecutionHistoryEntry> list = new List<ExecutionHistoryEntry>();
  //    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
  //    {

  //    }
  //    else
  //    {
  //        using (ConnectionAndTransactionHolder dbConnection = GetConnection(IsolationLevel.ReadUncommitted))
  //        {
  //            string sql = string.Format(PropertySqlServerSelectHistoryGroupByTrigger, TablePrefix);

  //            if (Provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
  //            {
  //                sql = string.Format(PropertyMySqlSelectHistoryGroupByTrigger, TablePrefix);
  //            }

  //            using (DbCommand dbCommand = Delegate.PrepareCommand(dbConnection, sql))
  //            {
  //                Delegate.AddCommandParameter(dbCommand, "schedulerName", schedulerName);

  //                Delegate.AddCommandParameter(dbCommand, "limit", limitPerTrigger);

  //                using (DbDataReader reader = await dbCommand.ExecuteReaderAsync(cancellationToken))
  //                {
  //                    while (await reader.ReadAsync(cancellationToken))
  //                    {
  //                        var entry = new ExecutionHistoryEntry
  //                        {
  //                            Recovering = Delegate.GetBooleanFromDbValue(reader["Recovering"]),
  //                            Vetoed = Delegate.GetBooleanFromDbValue(reader["Vetoed"]),
  //                            ActualFireTimeUtc = Convert.ToDateTime(reader["Actual_Fire_Time_Utc"]),
  //                            FinishedTimeUtc = Convert.ToDateTime(reader["Finished_Time_Utc"]),
  //                            FireInstanceId = reader.GetString("Fire_Instance_Id"),
  //                            ScheduledFireTimeUtc = Convert.ToDateTime(reader["Scheduled_Fire_Time_Utc"]),
  //                            SchedulerInstanceId = reader.GetString("Scheduler_Instance_Id"),
  //                            SchedulerName = reader.GetString("SCHED_NAME"),
  //                            JobName = reader.GetString("JOB_NAME"),
  //                            JobGroup = reader.GetString("JOB_GROUP"),
  //                            TriggerName = reader.GetString("TRIGGER_NAME"),
  //                            TriggerGroup = reader.GetString("TRIGGER_GROUP"),
  //                            FiredTime = Delegate.GetDateTimeFromDbValue(reader["FIRED_TIME"]).GetValueOrDefault(),
  //                            ScheduledTime = Delegate.GetDateTimeFromDbValue(reader["SCHED_TIME"]).GetValueOrDefault(),
  //                            RunTime = Delegate.GetTimeSpanFromDbValue(reader["RUN_TIME"]).GetValueOrDefault(),
  //                            Error = Delegate.GetBooleanFromDbValue(reader["ERROR"]),
  //                            ErrorMessage = reader.GetString("ERROR_MESSAGE")
  //                        };
  //                        list.Add(entry);
  //                    }
  //                }
  //            }
  //        }

  //        list.Reverse();
  //    }

  //    return list;
  //}

  //private const string PropertySqlServerSelectHistory = "SELECT TOP @limit * FROM {0}JOB_HISTORY WHERE SCHED_NAME = @schedulerName ORDER BY Actual_Fire_Time_Utc desc";
  //private const string PropertyMySqlSelectHistory = "SELECT * FROM {0}JOB_HISTORY WHERE SCHED_NAME = @schedulerName ORDER BY Actual_Fire_Time_Utc desc limit @limit";

  //public virtual async Task<IEnumerable<ExecutionHistoryEntry>> FilterLastAsync(string schedulerName, int limit, CancellationToken cancellationToken = default)
  //{
  //    List<ExecutionHistoryEntry> list = new List<ExecutionHistoryEntry>();
  //    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
  //    {

  //    }
  //    else
  //    {
  //        using (ConnectionAndTransactionHolder dbConnection = GetConnection(IsolationLevel.ReadUncommitted))
  //        {
  //            string sql = string.Format(PropertySqlServerSelectHistory, TablePrefix);

  //            if (Provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
  //            {
  //                sql = string.Format(PropertyMySqlSelectHistory, TablePrefix);
  //            }

  //            using (DbCommand dbCommand = Delegate.PrepareCommand(dbConnection, sql))
  //            {
  //                Delegate.AddCommandParameter(dbCommand, "schedulerName", schedulerName);

  //                Delegate.AddCommandParameter(dbCommand, "limit", limit);

  //                using (DbDataReader reader = await dbCommand.ExecuteReaderAsync(cancellationToken))
  //                {
  //                    while (await reader.ReadAsync(cancellationToken))
  //                    {
  //                        var entry = new ExecutionHistoryEntry
  //                        {
  //                            Recovering = Delegate.GetBooleanFromDbValue(reader["Recovering"]),
  //                            Vetoed = Delegate.GetBooleanFromDbValue(reader["Vetoed"]),
  //                            ActualFireTimeUtc = Convert.ToDateTime(reader["Actual_Fire_Time_Utc"]),
  //                            FinishedTimeUtc = Convert.ToDateTime(reader["Finished_Time_Utc"]),
  //                            FireInstanceId = reader.GetString("Fire_Instance_Id"),
  //                            ScheduledFireTimeUtc = Convert.ToDateTime(reader["Scheduled_Fire_Time_Utc"]),
  //                            SchedulerInstanceId = reader.GetString("Scheduler_Instance_Id"),
  //                            SchedulerName = reader.GetString("SCHED_NAME"),
  //                            JobName = reader.GetString("JOB_NAME"),
  //                            JobGroup = reader.GetString("JOB_GROUP"),
  //                            TriggerName = reader.GetString("TRIGGER_NAME"),
  //                            TriggerGroup = reader.GetString("TRIGGER_GROUP"),
  //                            FiredTime = Delegate.GetDateTimeFromDbValue(reader["FIRED_TIME"]).GetValueOrDefault(),
  //                            ScheduledTime = Delegate.GetDateTimeFromDbValue(reader["SCHED_TIME"]).GetValueOrDefault(),
  //                            RunTime = Delegate.GetTimeSpanFromDbValue(reader["RUN_TIME"]).GetValueOrDefault(),
  //                            Error = Delegate.GetBooleanFromDbValue(reader["ERROR"]),
  //                            ErrorMessage = reader.GetString("ERROR_MESSAGE")
  //                        };
  //                        list.Add(entry);
  //                    }
  //                }
  //            }
  //        }

  //        list.Reverse();
  //    }

  //    return list;
  //}

  //private const string PropertySqlSelectHistorySuccessCount = "SELECT COUNT(1) FROM {0}JOB_HISTORY WHERE SCHED_NAME = @schedulerName AND Error = false";

  //public virtual async Task<long> GetTotalJobsExecutedAsync(string schedulerName, CancellationToken cancellationToken = default)
  //{
  //    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
  //    {
  //        long totalJobsExecuted = histories.Values.Where(x => x.SchedulerName.Equals(schedulerName) && x.Error.Equals(false)).LongCount();
  //        return await Task.FromResult(totalJobsExecuted);
  //    }
  //    else
  //    {
  //        using (ConnectionAndTransactionHolder dbConnection = GetConnection(IsolationLevel.ReadUncommitted))
  //        {
  //            string sqlCount = Extensions.ReplaceTablePrefix(PropertySqlSelectHistorySuccessCount, TablePrefix);
  //            using (DbCommand dbCommand = Delegate.PrepareCommand(dbConnection, sqlCount))
  //            {
  //                Delegate.AddCommandParameter(dbCommand, "schedulerName", schedulerName);
  //                return (long)await dbCommand.ExecuteScalarAsync(cancellationToken);
  //            }
  //        }
  //    }
  //}

  //private const string PropertySqlSelectHistoryFailedCount = "SELECT COUNT(1) FROM {0}JOB_HISTORY WHERE SCHED_NAME = @schedulerName AND Error = true";

  //public virtual async Task<long> GetTotalJobsFailedAsync(string schedulerName, CancellationToken cancellationToken = default)
  //{
  //    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
  //    {
  //        long totalJobsFailed = histories.Values.Where(x => x.SchedulerName.Equals(schedulerName) && x.Error.Equals(true)).LongCount();
  //            return await Task.FromResult(totalJobsFailed);
  //    }
  //    else
  //    {
  //        using (ConnectionAndTransactionHolder dbConnection = GetConnection(IsolationLevel.ReadUncommitted))
  //        {
  //            string sqlCount = Extensions.ReplaceTablePrefix(PropertySqlSelectHistoryFailedCount, TablePrefix);
  //            using (DbCommand dbCommand = Delegate.PrepareCommand(dbConnection, sqlCount))
  //            {
  //                Delegate.AddCommandParameter(dbCommand, "schedulerName", schedulerName);
  //                return (long)await dbCommand.ExecuteScalarAsync(cancellationToken);
  //            }
  //        }
  //    }
  //}
  */
  public virtual async Task PurgeAsync(string schedulerName, CancellationToken cancellationToken = default)
  {
    if (Provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
    {
      histories.RemoveAll(p => p.Value.SchedulerName.Equals(schedulerName, StringComparison.OrdinalIgnoreCase));
    }

    await Task.FromResult(cancellationToken);
  }

  /// <summary>
  /// Get the driver delegate for DB operations.
  /// </summary>
  protected virtual IDbAccessor DbAccessor
  {
    get
    {
      var jobHistoryStore = this;
      lock (jobHistoryStore)
      {
        if (_driverDelegate != null)
        {
          return _driverDelegate;
        }

        try
        {
          if (DelegateTypeName != null)
          {
            _delegateType = TypeLoadHelper.LoadType(DelegateTypeName);
          }

          var dbProvider = DBConnectionManager.Instance.GetDbProvider(DataSource);
          var args = new DelegateInitializationArgs { DbProvider = dbProvider };

          var ctor = _delegateType.GetConstructor(Array.Empty<Type>());
          if (ctor == null)
          {
#pragma warning disable CA1065 // 不要在意外的位置引发异常
            throw new ArgumentNullException(nameof(ctor), "Configured delegate does not have public constructor that takes no arguments");
#pragma warning restore CA1065 // 不要在意外的位置引发异常
          }

          _driverDelegate = (StdAdoDelegate)ctor.Invoke(null);
          _driverDelegate.Initialize(args);
        }
        catch (Exception e)
        {
#pragma warning disable CA1065 // 不要在意外的位置引发异常
          throw new NoSuchDelegateException("Couldn't instantiate delegate: " + e.Message, e);
#pragma warning restore CA1065 // 不要在意外的位置引发异常
        }
      }

      return _driverDelegate;
    }
  }

  /// <summary>
  /// Gets the connection and starts a new transaction.
  /// </summary>
  /// <param name="isolationLevel">IsolationLevel</param>
  protected ConnectionAndTransactionHolder GetConnection(IsolationLevel isolationLevel)
  {
    DbConnection conn;
    DbTransaction tx;
    try
    {
      conn = DBConnectionManager.Instance.GetConnection(DataSource);
      conn.Open();
    }
    catch (Exception e)
    {
      throw new JobPersistenceException(
          $"Failed to obtain DB connection from data source '{DataSource}': {e}", e);
    }

    if (conn == null)
    {
      throw new JobPersistenceException($"Could not get connection from DataSource '{DataSource}'");
    }

    try
    {
      tx = conn.BeginTransaction(isolationLevel);
    }
    catch (Exception e)
    {
      conn.Close();
      throw new JobPersistenceException("Failure setting up connection.", e);
    }

    return new ConnectionAndTransactionHolder(conn, tx);
  }
}
