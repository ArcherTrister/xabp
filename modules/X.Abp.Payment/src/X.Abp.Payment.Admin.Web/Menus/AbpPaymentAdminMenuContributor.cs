// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

using X.Abp.Payment.Admin.Permissions;
using X.Abp.Payment.Localization;

namespace X.Abp.Payment.Admin.Web.Menus;

public class AbpPaymentAdminMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        IStringLocalizer localizer = context.GetLocalizer<PaymentResource>();
        ApplicationMenuItem menuItem = new ApplicationMenuItem(AbpPaymentAdminMenus.GroupName, localizer["Menu:PaymentManagement"], null, "fa fa-money-check");
        context.Menu.AddItem(menuItem);
        menuItem.AddItem(new ApplicationMenuItem(AbpPaymentAdminMenus.Plans.PlansMenu, localizer["Menu:Plans"].Value, "/Payment/Plans", "fa fa-file-alt"))
            .RequirePermissions(AbpPaymentAdminPermissions.Plans.Default);
        menuItem.AddItem(new ApplicationMenuItem(AbpPaymentAdminMenus.PaymentRequests.PaymentRequestsMenu, localizer["Menu:PaymentRequests"].Value, "/Payment/Requests", "fa fa-exchange"))
            .RequirePermissions(AbpPaymentAdminPermissions.PaymentRequests.Default);
        return Task.CompletedTask;
    }
}
