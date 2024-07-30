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
            public const string Default = "Payment.Plans";
            public const string Create = "Payment.Plans.Create";
            public const string Update = "Payment.Plans.Update";
            public const string Delete = "Payment.Plans.Delete";

            public static class GatewayPlans
            {
                public const string Default = "Payment.Plans.GatewayPlans";
                public const string Create = "Payment.Plans.GatewayPlans.Create";
                public const string Update = "Payment.Plans.GatewayPlans.Update";
                public const string Delete = "Payment.Plans.GatewayPlans.Delete";
            }
        }

        public static class PaymentRequests
        {
            public const string Default = "Payment.PaymentRequests";
        }
    }
}
