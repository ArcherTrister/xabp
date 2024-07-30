// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.IdentityServer.IdentityResources;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

using X.Abp.IdentityServer.IdentityResource.Dtos;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.IdentityResources
{
    public class IdentityResourceUpdateModalView : ExtensibleObject
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DynamicStringLength(typeof(IdentityResourceConsts), "DescriptionMaxLength", null)]
        [Required]
        public string Name { get; set; }

        [DynamicStringLength(typeof(IdentityResourceConsts), "DisplayNameMaxLength", null)]
        public string DisplayName { get; set; }

        [DynamicStringLength(typeof(IdentityResourceConsts), "DescriptionMaxLength", null)]
        public string Description { get; set; }

        public bool Enabled { get; set; }

        public bool Required { get; set; }

        public bool Emphasize { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }

        public string[] UserClaims { get; set; }

        public IdentityResourcePropertyDto[] Properties { get; set; }

        public IdentityResourceUpdateModalView()
        {
            Properties = Array.Empty<IdentityResourcePropertyDto>();
        }
    }
}
