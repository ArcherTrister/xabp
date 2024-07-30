// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Views.Error;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Views.Error.DefaultErrorComponent
{
    public class LeptonErrorPageModel : AbpErrorViewModel
    {
        public string DefaultErrorMessageKey { get; set; }
    }
}
