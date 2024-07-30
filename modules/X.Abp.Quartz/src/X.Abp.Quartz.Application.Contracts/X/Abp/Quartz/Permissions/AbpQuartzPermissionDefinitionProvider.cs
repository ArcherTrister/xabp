// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

using X.Abp.Quartz.Localization;

namespace X.Abp.Quartz.Permissions;

public class AbpQuartzPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var quartzGroup = context.AddGroup(AbpQuartzPermissions.GroupName, L("Permission:Quartz"));

        // Scheduler
        quartzGroup.AddPermission(AbpQuartzPermissions.Schedulers.Default, L("Permission:SchedulerManagement"), MultiTenancySides.Host);

        // Calendar
        var calendarsPermission = quartzGroup.AddPermission(AbpQuartzPermissions.Calendars.Default, L("Permission:CalendarManagement"), MultiTenancySides.Host);
        calendarsPermission.AddChild(AbpQuartzPermissions.Calendars.Create, L("Permission:Create"), MultiTenancySides.Host);
        calendarsPermission.AddChild(AbpQuartzPermissions.Calendars.Delete, L("Permission:Delete"), MultiTenancySides.Host);

        // ExecutionHistory
        quartzGroup.AddPermission(AbpQuartzPermissions.ExecutionHistory.Default, L("Permission:ExecutionHistoryManagement"), MultiTenancySides.Host);

        // Job
        var jobsPermission = quartzGroup.AddPermission(AbpQuartzPermissions.Jobs.Default, L("Permission:JobManagement"), MultiTenancySides.Host);
        jobsPermission.AddChild(AbpQuartzPermissions.Jobs.Create, L("Permission:Create"), MultiTenancySides.Host);
        jobsPermission.AddChild(AbpQuartzPermissions.Jobs.Update, L("Permission:Edit"), MultiTenancySides.Host);
        jobsPermission.AddChild(AbpQuartzPermissions.Jobs.Delete, L("Permission:Delete"), MultiTenancySides.Host);

        // Trigger
        var triggersPermission = quartzGroup.AddPermission(AbpQuartzPermissions.Triggers.Default, L("Permission:TriggerManagement"), MultiTenancySides.Host);
        triggersPermission.AddChild(AbpQuartzPermissions.Triggers.Update, L("Permission:Edit"), MultiTenancySides.Host);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<QuartzResource>(name);
    }
}
