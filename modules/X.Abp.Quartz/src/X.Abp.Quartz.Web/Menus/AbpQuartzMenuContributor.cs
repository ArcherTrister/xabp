// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.UI.Navigation;

using X.Abp.Quartz.Localization;

namespace X.Abp.Quartz.Web.Menus;

public class AbpQuartzMenuContributor : IMenuContributor
{
  public virtual async Task ConfigureMenuAsync(MenuConfigurationContext context)
  {
    if (context.Menu.Name == StandardMenus.Main)
    {
      await ConfigureMainMenuAsync(context);
    }
  }

  private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
  {
    // Add main menu items.
    var localizer = context.GetLocalizer<QuartzResource>();

    context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Prefix, localizer["Menu:Quartz"], "~/Quartz", icon: "fa fa-globe"));

    if (context.Menu.Name == AbpQuartzMenus.Prefix)
    {
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Scheduler, localizer["Menu:Quartz"], "~/Quartz", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Job, localizer["Menu:Jobs"], "~/Quartz/Jobs", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Trigger, localizer["Menu:Triggers"], "~/Quartz/Triggers", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Calendar, localizer["Menu:Calendars"], "~/Quartz/Calendars", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.History, localizer["Menu:Histories"], "~/Quartz/Histories", icon: "fa fa-globe"));
    }

    if (context.Menu.Name == AbpQuartzMenus.Job)
    {
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.AllJob, localizer["AllJob"], "~/Quartz/Jobs", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Queued, localizer["Queued"], "~/Quartz/Jobs/Queued", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Plan, localizer["Plan"], "~/Quartz/Jobs/Plan", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Executing, localizer["Executing"], "~/Quartz/Jobs/Executing", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Complete, localizer["Complete"], "~/Quartz/Jobs/Complete", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Fail, localizer["Fail"], "~/Quartz/Jobs/Fail", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Delete, localizer["Delete"], "~/Quartz/Jobs/Delete", icon: "fa fa-globe"));
      context.Menu.AddItem(new ApplicationMenuItem(AbpQuartzMenus.Waiting, localizer["Waiting"], "~/Quartz/Jobs/Waiting", icon: "fa fa-globe"));
    }

    return Task.CompletedTask;
  }
}
