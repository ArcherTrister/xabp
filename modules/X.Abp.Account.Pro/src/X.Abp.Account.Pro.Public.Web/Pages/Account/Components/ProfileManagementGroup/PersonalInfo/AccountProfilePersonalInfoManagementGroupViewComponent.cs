// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

using X.Abp.Account.Dtos;

namespace X.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;

public class AccountProfilePersonalInfoManagementGroupViewComponent : AbpViewComponent
{
    protected IProfileAppService ProfileAppService { get; }

    public AccountProfilePersonalInfoManagementGroupViewComponent(
        IProfileAppService profileAppService)
    {
        ProfileAppService = profileAppService;

        ObjectMapperContext = typeof(AbpAccountPublicWebModule);
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await ProfileAppService.GetAsync();

        var model = ObjectMapper.Map<ProfileDto, PersonalInfoModel>(user);

        return View("~/Pages/Account/Components/ProfileManagementGroup/PersonalInfo/Default.cshtml", model);
    }

    public class PersonalInfoModel
    {
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
        [Display(Name = "DisplayName:UserName")]
        public string UserName { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        [Display(Name = "DisplayName:Email")]
        public string Email { get; set; }

        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxNameLength))]
        [Display(Name = "DisplayName:Name")]
        public string Name { get; set; }

        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxSurnameLength))]
        [Display(Name = "DisplayName:Surname")]
        public string Surname { get; set; }

        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPhoneNumberLength))]
        [Display(Name = "DisplayName:PhoneNumber")]
        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}
