// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.ApiResources
{
    public class ApiResourceSecretModalView
    {
        [SelectItems("SecretTypes")]
        public string Type { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public DateTime? Expiration { get; set; }
    }
}
