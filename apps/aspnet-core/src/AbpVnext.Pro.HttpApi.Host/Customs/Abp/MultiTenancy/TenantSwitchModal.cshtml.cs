using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Pages.Abp.MultiTenancy;

using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;

namespace AbpVnext.Pro.Pages.Abp.MultiTenancy
{
    public class CustomTenantSwitchModalModel : TenantSwitchModalModel
    {
        public CustomTenantSwitchModalModel(
            ITenantStore tenantStore,
            IOptions<AbpAspNetCoreMultiTenancyOptions> options)
            : base(tenantStore, options)
        {
        }

        public override async Task OnGetAsync()
        {
            Input = new TenantInfoModel();

            if (CurrentTenant.IsAvailable)
            {
                var tenant = await TenantStore.FindAsync(CurrentTenant.GetId());
                Input.Name = tenant?.Name;
            }
        }

        public override async Task OnPostAsync()
        {
            Guid? tenantId = null;
            if (!Input.Name.IsNullOrEmpty())
            {
                var tenant = await TenantStore.FindAsync(Input.Name);
                if (tenant == null)
                {
                    throw new UserFriendlyException(L["GivenTenantIsNotExist", Input.Name]);
                }

                if (!tenant.IsActive)
                {
                    throw new UserFriendlyException(L["GivenTenantIsNotAvailable", Input.Name]);
                }

                tenantId = tenant.Id;
            }

            AbpMultiTenancyCookieHelper.SetTenantCookie(HttpContext, tenantId, Options.TenantKey);
        }
    }
}
