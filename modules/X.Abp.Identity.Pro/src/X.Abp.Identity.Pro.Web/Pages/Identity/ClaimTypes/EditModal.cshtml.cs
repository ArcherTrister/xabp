// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;

namespace X.Abp.Identity.Web.Pages.Identity.ClaimTypes;

public class EditModalModel : IdentityPageModel
{
    [BindProperty]
    public ClaimTypeInfoModel ClaimType { get; set; }

    protected IIdentityClaimTypeAppService IdentityClaimTypeAppService { get; }

    public EditModalModel(IIdentityClaimTypeAppService identityClaimTypeAppService)
    {
        IdentityClaimTypeAppService = identityClaimTypeAppService;
    }

    public virtual async Task OnGetAsync(Guid id)
    {
        ClaimType = ObjectMapper.Map<ClaimTypeDto, ClaimTypeInfoModel>(
            await IdentityClaimTypeAppService.GetAsync(id));
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        var input = ObjectMapper.Map<ClaimTypeInfoModel, UpdateClaimTypeDto>(ClaimType);
        await IdentityClaimTypeAppService.UpdateAsync(ClaimType.Id, input);

        return NoContent();
    }

    public class ClaimTypeInfoModel : ExtensibleObject, IHasConcurrencyStamp
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool Required { get; set; }

        public string Regex { get; set; }

        public string RegexDescription { get; set; }

        public string Description { get; set; }

        public IdentityClaimValueType ValueType { get; set; }

        [HiddenInput]
        public string ConcurrencyStamp { get; set; }
    }
}
