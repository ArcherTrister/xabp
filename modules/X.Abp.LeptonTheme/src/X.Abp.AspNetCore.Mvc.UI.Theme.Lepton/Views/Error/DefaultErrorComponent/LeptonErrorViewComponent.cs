// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Views.Error;

using X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Views.Error.DefaultErrorComponent
{
    public class LeptonErrorViewComponent : LeptonViewComponentBase
    {
        public IViewComponentResult Invoke(AbpErrorViewModel model, string defaultErrorMessageKey)
        {
            var leptonModel = new LeptonErrorPageModel
            {
                ErrorInfo = model.ErrorInfo,
                HttpStatusCode = model.HttpStatusCode,
                DefaultErrorMessageKey = defaultErrorMessageKey
            };

            return View("~/Views/Error/DefaultErrorComponent/Default.cshtml", leptonModel);
        }
    }
}
