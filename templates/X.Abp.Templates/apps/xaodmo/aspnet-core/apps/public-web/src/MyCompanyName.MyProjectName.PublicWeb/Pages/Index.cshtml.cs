using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace MyCompanyName.MyProjectName.PublicWeb.Pages;

public class IndexModel : AbpPageModel
{
    public void OnGet()
    {

    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
