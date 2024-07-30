// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

using X.Abp.Forms.Forms;

namespace X.Abp.Forms.Web.Pages.Forms;

public class SendModalModel : FormsPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public SendFormInfoModel Form { get; set; }

    protected IFormAppService FormAppService { get; }

    protected IHttpContextAccessor HttpContextAccessor { get; }

    public SendModalModel(IFormAppService formAppService, IHttpContextAccessor httpContextAccessor)
    {
        FormAppService = formAppService;
        HttpContextAccessor = httpContextAccessor;
    }

    public virtual async Task OnGetAsync()
    {
        var form = await FormAppService.GetAsync(Id);

        var link = GenerateLink(form.Id);
        var message = L["Form:SendFormInvitation"].Value + $"\n<br />\n<a href=\"{link}\">{form.Title}</a>";

        Form = new SendFormInfoModel
        {
            Id = form.Id,
            Link = link,
            Body = message,
            Subject = form.Title
        };
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(NoContent());
    }

    public class SendFormInfoModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        public string Link { get; set; }

        [TextArea(Rows = 3)]
        public string Body { get; set; }

        public string Subject { get; set; }

        [Required]
        [EmailAddress]
        public string To { get; set; }
    }

    private string GenerateLink(Guid formId)
    {
        var request = HttpContextAccessor.HttpContext?.Request;
        var scheme = request?.Scheme;
        var host = request?.Host.Value;
        return $"{scheme}://{host}/Forms/{formId}/ViewForm";
    }
}
