// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Account.Dtos;

namespace X.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.AuthenticatorApp
{
    public class AccountProfileAuthenticatorAppGroupViewComponent : AbpViewComponent
    {
        protected IAccountAppService AccountAppService { get; }

        public AccountProfileAuthenticatorAppGroupViewComponent(IAccountAppService accountAppService)
        {
            AccountAppService = accountAppService;
        }

        public virtual async Task<IViewComponentResult> InvokeAsync()
        {
            AuthenticatorInfoDto authenticatorInfo = await AccountAppService.GetAuthenticatorInfoAsync();
            AuthenticatorAppModel model = new AuthenticatorAppModel
            {
                HasAuthenticator = await AccountAppService.HasAuthenticatorAsync(),
                SharedKey = authenticatorInfo.Key,
                AuthenticatorUri = authenticatorInfo.Uri
            };

            return View("~/Pages/Account/Components/ProfileManagementGroup/AuthenticatorApp/Default.cshtml", model);
        }

        public class AuthenticatorAppModel
        {
            public bool HasAuthenticator { get; set; }

            public string SharedKey { get; set; }

            public string AuthenticatorUri { get; set; }

            public string Code { get; set; }
        }
    }
}
