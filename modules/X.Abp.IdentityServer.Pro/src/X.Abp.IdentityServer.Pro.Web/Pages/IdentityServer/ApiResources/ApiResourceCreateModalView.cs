// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.ApiResources
{
    public class ApiResourceCreateModalView : ExtensibleObject
    {
        [DynamicStringLength(typeof(ApiResourceConsts), "NameMaxLength", null)]
        [Required]
        public string Name { get; set; }

        [DynamicStringLength(typeof(ApiResourceConsts), "DisplayNameMaxLength", null)]
        public string DisplayName { get; set; }

        [DynamicStringLength(typeof(ApiResourceConsts), "DescriptionMaxLength", null)]
        public string Description { get; set; }

        [DynamicStringLength(typeof(ApiResourceConsts), "AllowedAccessTokenSigningAlgorithmsMaxLength", null)]
        public string AllowedAccessTokenSigningAlgorithms { get; set; }

        public string[] UserClaims { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }
    }
}
