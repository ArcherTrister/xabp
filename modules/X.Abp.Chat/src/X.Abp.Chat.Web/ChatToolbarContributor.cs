// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;

using X.Abp.Chat.Features;
using X.Abp.Chat.Permission;
using X.Abp.Chat.Web.Pages.Chat.Components.MessagesToolbarItem;

namespace X.Abp.Chat.Web;

public class ChatToolbarContributor : IToolbarContributor
{
  public virtual async Task ConfigureToolbarAsync(IToolbarConfigurationContext context)
  {
    if (context.Toolbar.Name == StandardToolbars.Main)
    {
      var featureChecker = context.ServiceProvider.GetService<IFeatureChecker>();
      if (await featureChecker.IsEnabledAsync(AbpChatFeatures.Enable))
      {
        context.Toolbar.Items
            .Insert(0, new ToolbarItem(typeof(MessagesToolbarItemViewComponent)).RequirePermissions(AbpChatPermissions.Messaging));
      }
    }
  }
}
