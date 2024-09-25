// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Payment.Admin.Permissions
{
    public static class AbpPaymentAdminPermissions
    {
        public const string GroupName = "Payment";

        public static class Plans
        {
            public const string Default = GroupName + ".Plans";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";

            public static class GatewayPlans
            {
                public const string Default = GroupName + ".Plans.GatewayPlans";
                public const string Create = Default + ".Create";
                public const string Update = Default + ".Update";
                public const string Delete = Default + ".Delete";
            }
        }

        public static class PaymentRequests
        {
            public const string Default = GroupName + ".PaymentRequests";
        }
    }
}
