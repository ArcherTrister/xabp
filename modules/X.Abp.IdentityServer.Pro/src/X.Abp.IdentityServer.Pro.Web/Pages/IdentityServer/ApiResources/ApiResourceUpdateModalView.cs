// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

using X.Abp.IdentityServer.ApiResource.Dtos;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.ApiResources
{
    public class ApiResourceUpdateModalView : ExtensibleObject
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DynamicStringLength(typeof(ApiResourceConsts), "DescriptionMaxLength", null)]
        [ReadOnlyInput]
        public string Name { get; set; }

        [DynamicStringLength(typeof(ApiResourceConsts), "DisplayNameMaxLength", null)]
        public string DisplayName { get; set; }

        [DynamicStringLength(typeof(ApiResourceConsts), "DescriptionMaxLength", null)]
        public string Description { get; set; }

        [DynamicStringLength(typeof(ApiResourceConsts), "AllowedAccessTokenSigningAlgorithmsMaxLength", null)]
        public string AllowedAccessTokenSigningAlgorithms { get; set; }

        public bool Enabled { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }

        public ApiResourceSecretDto[] Secrets { get; set; }

        public string[] UserClaims { get; set; }

        public List<string> Scopes { get; set; }

        public ApiResourcePropertyDto[] Properties { get; set; } = Array.Empty<ApiResourcePropertyDto>();
    }
}
