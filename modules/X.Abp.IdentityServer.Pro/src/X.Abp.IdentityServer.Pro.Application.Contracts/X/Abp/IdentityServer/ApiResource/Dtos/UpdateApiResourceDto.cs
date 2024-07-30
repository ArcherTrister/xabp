// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace X.Abp.IdentityServer.ApiResource.Dtos;

public class UpdateApiResourceDto : ExtensibleObject
{
    [DynamicStringLength(typeof(ApiResourceConsts), "DisplayNameMaxLength", null)]
    public string DisplayName { get; set; }

    [DynamicStringLength(typeof(ApiResourceConsts), "DescriptionMaxLength", null)]
    public string Description { get; set; }

    [DynamicStringLength(typeof(ApiResourceConsts), "AllowedAccessTokenSigningAlgorithmsMaxLength", null)]
    public string AllowedAccessTokenSigningAlgorithms { get; set; }

    public bool Enabled { get; set; }

    /// <summary>
    /// ShowInDiscoveryDocument
    /// </summary>
    public bool ShowInDiscoveryDocument { get; set; }

    public List<ApiResourceSecretDto> Secrets { get; set; }

    public List<ApiResourceScopeDto> Scopes { get; set; }

    public List<ApiResourceClaimDto> UserClaims { get; set; }

    public List<ApiResourcePropertyDto> Properties { get; set; }
}
