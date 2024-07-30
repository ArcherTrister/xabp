// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using IdentityModel.Client;

using Volo.Abp.MultiTenancy;

namespace X.Abp.Account.Web.Extensions;

public class SpaExternalLoginTokenRequest : ProtocolRequest, IMultiTenant
{
    public string LoginProvider { get; set; }

    public string ProviderKey { get; set; }

    public Guid? TenantId { get; set; }

    public string Scope { get; set; }
}
