// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application.Services;

using X.Abp.Payment.Localization;

namespace X.Abp.Payment.Admin
{
    public abstract class PaymentAdminAppServiceBase : ApplicationService
    {
        protected PaymentAdminAppServiceBase()
        {
            ObjectMapperContext = typeof(AbpPaymentAdminApplicationModule);
            LocalizationResource = typeof(PaymentResource);
        }
    }
}
