// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Users;

using X.Abp.Account.Public.Web.ExternalProviders;
using X.Abp.Identity;

namespace X.Abp.Account.Public.Web.Pages.Account
{
    public class ScanCodeLoginModel : AccountPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string QrCodeKey { get; set; }

        protected IScanCodeLoginProvider ScanCodeLoginProvider => LazyServiceProvider.LazyGetRequiredService<IScanCodeLoginProvider>();

        public virtual async Task<IActionResult> OnGetAsync()
        {
            // TODO: SSO
            await ScanCodeLoginProvider.ScanCodeAsync(QrCodeKey, CurrentUser.GetId(), CurrentTenant.Id);

            return Page();
        }

        /// <summary>
        /// 扫码登录
        /// </summary>
        /// <returns>IActionResult</returns>
        public virtual async Task OnPostAsync()
        {
            using (CurrentTenant.Change(CurrentTenant.Id))
            {
                var user = await UserManager.GetByIdAsync(CurrentUser.GetId());
                var token = await UserManager.GenerateUserTokenAsync(
                    user,
                    ScanCodeUserTokenProviderConsts.ScanCodeUserTokenProviderName,
                    ScanCodeUserTokenProviderConsts.ScanCodeUserLoginTokenPurpose);

                var qrCodeInfo = await ScanCodeLoginProvider.ConfirmLoginAsync(QrCodeKey, token);

                if (qrCodeInfo.QrCodeStatus != QrCodeStatus.Confirmed)
                {
                    throw new UserFriendlyException("二维码无效");
                }
            }
        }
    }
}
