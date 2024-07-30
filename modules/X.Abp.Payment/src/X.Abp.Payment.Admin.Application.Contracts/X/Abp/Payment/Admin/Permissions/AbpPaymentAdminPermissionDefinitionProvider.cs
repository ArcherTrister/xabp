// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

using X.Abp.Payment.Localization;

namespace X.Abp.Payment.Admin.Permissions
{
    public class AbpPaymentAdminPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var permissionGroupDefinition = context.GetGroupOrNull(AbpPaymentAdminPermissions.GroupName) ?? context.AddGroup(AbpPaymentAdminPermissions.GroupName, L("Permission:Payment"));

            var planPermissionDefinition = permissionGroupDefinition.AddPermission(AbpPaymentAdminPermissions.Plans.Default, L("Permission:PaymentPlanManagement"), MultiTenancySides.Host, true);
            planPermissionDefinition.AddChild(AbpPaymentAdminPermissions.Plans.Create, L("Permission:PaymentPlanManagement.Create"), MultiTenancySides.Host, true);
            planPermissionDefinition.AddChild(AbpPaymentAdminPermissions.Plans.Update, L("Permission:PaymentPlanManagement.Update"), MultiTenancySides.Host, true);
            planPermissionDefinition.AddChild(AbpPaymentAdminPermissions.Plans.Delete, L("Permission:PaymentPlanManagement.Delete"), MultiTenancySides.Host, true);

            var gatewayPlansPermissionDefinition = permissionGroupDefinition.AddPermission(AbpPaymentAdminPermissions.Plans.GatewayPlans.Default, L("Permission:PaymentGatewayPlanManagement"), MultiTenancySides.Host, true);
            gatewayPlansPermissionDefinition.AddChild(AbpPaymentAdminPermissions.Plans.GatewayPlans.Create, L("Permission:PaymentGatewayPlanManagement.Create"), MultiTenancySides.Host, true);
            gatewayPlansPermissionDefinition.AddChild(AbpPaymentAdminPermissions.Plans.GatewayPlans.Update, L("Permission:PaymentGatewayPlanManagement.Update"), MultiTenancySides.Host, true);
            gatewayPlansPermissionDefinition.AddChild(AbpPaymentAdminPermissions.Plans.GatewayPlans.Delete, L("Permission:PaymentGatewayPlanManagement.Delete"), MultiTenancySides.Host, true);

            permissionGroupDefinition.AddPermission(AbpPaymentAdminPermissions.PaymentRequests.Default, L("Permission:PaymentRequests"), MultiTenancySides.Host, true);
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<PaymentResource>(name);
        }
    }
}
