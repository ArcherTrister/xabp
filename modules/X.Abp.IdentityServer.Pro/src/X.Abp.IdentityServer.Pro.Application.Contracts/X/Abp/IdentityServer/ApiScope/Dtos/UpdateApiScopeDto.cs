// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace X.Abp.IdentityServer.ApiScope.Dtos;

public class UpdateApiScopeDto : ExtensibleObject
{
    [DynamicStringLength(typeof(ApiScopeConsts), "DisplayNameMaxLength", null)]
    public string DisplayName { get; set; }

    [DynamicStringLength(typeof(ApiScopeConsts), "DescriptionMaxLength", null)]
    public string Description { get; set; }

    public bool Required { get; set; }

    public bool Enabled { get; set; }

    public bool Emphasize { get; set; }

    public bool ShowInDiscoveryDocument { get; set; }

    public List<ApiScopeClaimDto> UserClaims { get; set; }

    public List<ApiScopePropertyDto> Properties { get; set; }
}
