// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.ObjectExtending;

public static class PaymentModuleExtensionConsts
{
    public const string ModuleName = "Payment";

    public static class EntityNames
    {
        public const string Plan = "Plan";
        public const string GatewayPlan = "GatewayPlan";
        public const string PaymentRequest = "PaymentRequest";
        public const string PaymentRequestProduct = "PaymentRequestProduct";
    }
}
