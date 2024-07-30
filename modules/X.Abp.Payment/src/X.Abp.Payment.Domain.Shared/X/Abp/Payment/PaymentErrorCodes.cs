// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Payment;

public static class AbpPaymentErrorCodes
{
    public const string CurrencyMustBeSet = "X.Abp.Payment:010001";

    public class Requests
    {
        public const string CantUpdateExternalSubscriptionId = "Payment:Request:0001";
    }
}
