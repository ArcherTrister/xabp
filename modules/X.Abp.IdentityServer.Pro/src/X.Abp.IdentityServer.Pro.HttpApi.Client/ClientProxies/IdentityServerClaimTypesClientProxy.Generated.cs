// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.ClientProxying;
using Volo.Abp.Http.Modeling;
using X.Abp.IdentityServer.ClaimType;
using X.Abp.IdentityServer.ClaimType.Dtos;

// ReSharper disable once CheckNamespace
namespace X.Abp.IdentityServer;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IIdentityServerClaimTypeAppService), typeof(IdentityServerClaimTypesClientProxy))]
public partial class IdentityServerClaimTypesClientProxy : ClientProxyBase<IIdentityServerClaimTypeAppService>, IIdentityServerClaimTypeAppService
{
    public virtual async Task<List<IdentityClaimTypeDto>> GetListAsync()
    {
        return await RequestAsync<List<IdentityClaimTypeDto>>(nameof(GetListAsync));
    }
}
