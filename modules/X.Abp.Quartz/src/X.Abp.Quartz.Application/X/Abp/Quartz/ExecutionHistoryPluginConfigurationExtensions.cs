// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using Quartz.Impl.AdoJobStore;
using Quartz.Util;

using X.Abp.Quartz.Plugins.ExecutionHistory;

using static Quartz.SchedulerBuilder;

namespace X.Abp.Quartz;

public static class ExecutionHistoryPluginConfigurationExtensions
{
    public static void UseInMemoryExecutionHistoryPlugin(this PersistentStoreOptions options)
    {
        // options.SetProperty("quartz.plugin.ExecutionHistory.type", typeof(InMemoryExecutionHistoryPlugin).AssemblyQualifiedNameWithoutVersion());
        // options.SetProperty("quartz.plugin.ExecutionHistory.storeType", typeof(InMemoryExecutionHistoryStore).AssemblyQualifiedNameWithoutVersion());
        options.SetProperty("quartz.plugin.ExecutionHistory.type", typeof(ExecutionHistoryPlugin).AssemblyQualifiedNameWithoutVersion());
        options.SetProperty("quartz.plugin.ExecutionHistory.storeType", typeof(JobHistoryStore).AssemblyQualifiedNameWithoutVersion());
        options.SetProperty("quartz.plugin.ExecutionHistory.dataSource", AdoProviderOptions.DefaultDataSourceName);

        // options.SetProperty("quartz.plugin.ExecutionHistory.tablePrefix", !string.IsNullOrWhiteSpace(tablePrefix) ? tablePrefix : AdoConstants.DefaultTablePrefix);
        options.SetProperty("quartz.plugin.ExecutionHistory.provider", "InMemory");

        // options.SetProperty("quartz.plugin.ExecutionHistory.driverDelegateType", "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz");
    }

    public static void UseSqlServerExecutionHistoryPlugin(this PersistentStoreOptions options)
    {
        var provider = options.Properties[$"quartz.dataSource.{AdoProviderOptions.DefaultDataSourceName}.provider"];

        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentNullException(nameof(provider), "Please place this method after the UseMySql method.");
        }

        if (!provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"The data provider should be mysql, but now it is {provider}.");
        }

        var tablePrefix = options.Properties["quartz.jobStore.tablePrefix"];

        options.SetProperty("quartz.plugin.ExecutionHistory.type", typeof(ExecutionHistoryPlugin).AssemblyQualifiedNameWithoutVersion());
        options.SetProperty("quartz.plugin.ExecutionHistory.storeType", typeof(JobHistoryStore).AssemblyQualifiedNameWithoutVersion());
        options.SetProperty("quartz.plugin.ExecutionHistory.dataSource", AdoProviderOptions.DefaultDataSourceName);
        options.SetProperty("quartz.plugin.ExecutionHistory.tablePrefix", !string.IsNullOrWhiteSpace(tablePrefix) ? tablePrefix : AdoConstants.DefaultTablePrefix);
        options.SetProperty("quartz.plugin.ExecutionHistory.provider", "SqlServer");
        options.SetProperty("quartz.plugin.ExecutionHistory.driverDelegateType", "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz");
    }

    public static void UseMySqlExecutionHistoryPlugin(this PersistentStoreOptions options)
    {
        var provider = options.Properties[$"quartz.dataSource.{AdoProviderOptions.DefaultDataSourceName}.provider"];

        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentException("Please place this method after the UseMySql method.");
        }

        if (!provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"The data provider should be mysql, but now it is {provider}.");
        }

        var tablePrefix = options.Properties["quartz.jobStore.tablePrefix"];

        options.SetProperty("quartz.plugin.ExecutionHistory.type", typeof(ExecutionHistoryPlugin).AssemblyQualifiedNameWithoutVersion());
        options.SetProperty("quartz.plugin.ExecutionHistory.storeType", typeof(JobHistoryStore).AssemblyQualifiedNameWithoutVersion());
        options.SetProperty("quartz.plugin.ExecutionHistory.dataSource", AdoProviderOptions.DefaultDataSourceName);
        options.SetProperty("quartz.plugin.ExecutionHistory.tablePrefix", !string.IsNullOrWhiteSpace(tablePrefix) ? tablePrefix : AdoConstants.DefaultTablePrefix);
        options.SetProperty("quartz.plugin.ExecutionHistory.provider", "MySql");
        options.SetProperty("quartz.plugin.ExecutionHistory.driverDelegateType", "Quartz.Impl.AdoJobStore.MySQLDelegate, Quartz");
    }

    /*
    //internal static void SetExecutionHistoryStore(this SchedulerContext context, IExecutionHistoryStore store)
    //{
    //    context.Put(typeof(IExecutionHistoryStore).FullName, store);
    //}

    //internal static IExecutionHistoryStore GetExecutionHistoryStore(this SchedulerContext context)
    //{
    //    return (IExecutionHistoryStore)context.Get(typeof(IExecutionHistoryStore).FullName);
    //}
    */

    /// <summary>
    /// 多属性排序
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="query">query</param>
    /// <param name="condition">eg:Id asc,Age desc</param>
    internal static IOrderedQueryable<T> MultiOrderBy<T>(this IQueryable<T> query, string condition)
    {
        var conditions = condition.Split(',');

        if (conditions.Length == 0)
        {
            return (IOrderedQueryable<T>)query;
        }

        IOrderedQueryable<T> res = null;

        for (var i = 0; i < conditions.Length; i++)
        {
            var strings = conditions[i].Split(" ");
            var fieldName = strings[0];
            var direction = strings[1];

            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, fieldName);
            var exp = Expression.Lambda(prop, param);

            var method = i == 0
                ? direction.ToLower(CultureInfo.CurrentCulture) == "asc" ? "OrderBy" : "OrderByDescending"
                : direction.ToLower(CultureInfo.CurrentCulture) == "asc" ? "ThenBy" : "ThenByDescending";
            Type[] types = { query.ElementType, exp.Body.Type };
            var mce = i == 0 ? Expression.Call(typeof(Queryable), method, types, query.Expression, exp) : Expression.Call(typeof(Queryable), method, types, res.Expression, exp);

            if (conditions.Length == 1)
            {
                return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(mce);
            }

            res = i == 0 ? (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(mce) : (IOrderedQueryable<T>)res.Provider.CreateQuery<T>(mce);
        }

        return res;
    }

    internal static string ReplaceTablePrefix(this string sql, string schedulerName)
    {
        if (string.IsNullOrWhiteSpace(schedulerName))
        {
            sql = sql.Replace("AND SCHED_NAME = @schedulerName", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        return sql;
    }
}
