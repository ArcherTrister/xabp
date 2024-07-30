// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Quartz.Web.Menus;

public class AbpQuartzMenus
{
    public const string Prefix = "AbpQuartz";

    // Add your menu items here...
    public const string Scheduler = Prefix + ".Scheduler";
    public const string Job = Prefix + ".Job";
    public const string AllJob = Job + ".AllJob";
    public const string Queued = Job + ".Queued";
    public const string Plan = Job + ".Plan";
    public const string Executing = Job + ".Executing";
    public const string Complete = Job + ".Complete";
    public const string Fail = Job + ".Fail";
    public const string Delete = Job + ".Delete";
    public const string Waiting = Job + ".Waiting";
    public const string Trigger = Prefix + ".Trigger";
    public const string Calendar = Prefix + ".Calendar";
    public const string History = Prefix + ".History";

    // public const string Recurring = Prefix + ".Recurring";
}
