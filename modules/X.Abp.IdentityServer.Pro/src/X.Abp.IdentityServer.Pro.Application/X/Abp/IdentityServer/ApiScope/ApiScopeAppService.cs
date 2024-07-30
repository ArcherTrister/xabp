// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.ObjectExtending;

using X.Abp.IdentityServer.ApiScope.Dtos;
using X.Abp.IdentityServer.Permissions;

namespace X.Abp.IdentityServer.ApiScope;

[Authorize(AbpIdentityServerProPermissions.ApiScope.Default)]
public class ApiScopeAppService : IdentityServerAppServiceBase, IApiScopeAppService
{
    protected IApiScopeRepository ApiScopeRepository { get; }

    public ApiScopeAppService(IApiScopeRepository apiScopeRepository)
    {
        ApiScopeRepository = apiScopeRepository;
    }

    public virtual async Task<PagedResultDto<ApiScopeWithDetailsDto>> GetListAsync(GetApiScopeListInput input)
    {
        var apiScopeList = await ApiScopeRepository.GetListAsync(
                        input.Sorting, input.SkipCount, input.MaxResultCount, input.Filter);

        var count = await ApiScopeRepository.GetCountAsync(input.Filter);
        var modelList = ObjectMapper.Map<List<Volo.Abp.IdentityServer.ApiScopes.ApiScope>, List<ApiScopeWithDetailsDto>>(apiScopeList);

        return new PagedResultDto<ApiScopeWithDetailsDto>(count, modelList);
    }

    public virtual async Task<List<ApiScopeWithDetailsDto>> GetAllListAsync()
    {
        var apiScopeList = await ApiScopeRepository.GetListAsync();
        return ObjectMapper.Map<List<Volo.Abp.IdentityServer.ApiScopes.ApiScope>, List<ApiScopeWithDetailsDto>>(apiScopeList);
    }

    public virtual async Task<ApiScopeWithDetailsDto> GetAsync(Guid id)
    {
        var apiScope = await ApiScopeRepository.GetAsync(id);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.ApiScopes.ApiScope, ApiScopeWithDetailsDto>(apiScope);
    }

    [Authorize(AbpIdentityServerProPermissions.ApiScope.Create)]
    public virtual async Task<ApiScopeWithDetailsDto> CreateAsync(CreateApiScopeDto input)
    {
        var nameExist = await ApiScopeRepository.CheckNameExistAsync(input.Name);
        if (nameExist)
        {
            throw new BusinessException("Volo.IdentityServer:DuplicateApiScopeName").WithData("Name", input.Name);
        }

        var id = GuidGenerator.Create();
        var apiScope = new Volo.Abp.IdentityServer.ApiScopes.ApiScope(
            id,
            input.Name,
            input.DisplayName,
            input.Description,
            input.Required,
            input.Emphasize,
            input.ShowInDiscoveryDocument,
            input.Enabled);

        input.UserClaims?.ForEach(
            claim => apiScope.AddUserClaim(claim.Type));

        input.MapExtraPropertiesTo(apiScope);
        var result = await ApiScopeRepository.InsertAsync(apiScope);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.ApiScopes.ApiScope, ApiScopeWithDetailsDto>(result);
    }

    [Authorize(AbpIdentityServerProPermissions.ApiScope.Update)]
    public virtual async Task<ApiScopeWithDetailsDto> UpdateAsync(Guid id, UpdateApiScopeDto input)
    {
        var apiScope = await ApiScopeRepository.GetAsync(id);
        apiScope.DisplayName = input.DisplayName;
        apiScope.Description = input.Description;
        apiScope.Required = input.Required;
        apiScope.Emphasize = input.Emphasize;
        apiScope.ShowInDiscoveryDocument = input.Enabled;
        apiScope.Enabled = input.Enabled;

        UpdateApiScopeClaims(input, apiScope);

        apiScope.RemoveAllProperties();
        foreach (var item in input.Properties)
        {
            apiScope.AddProperty(item.Key, item.Value);
        }

        input.MapExtraPropertiesTo(apiScope);

        var result = await ApiScopeRepository.UpdateAsync(apiScope);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.ApiScopes.ApiScope, ApiScopeWithDetailsDto>(result);
    }

    [Authorize(AbpIdentityServerProPermissions.ApiScope.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await ApiScopeRepository.DeleteAsync(id);
    }

    protected virtual void UpdateApiScopeClaims(UpdateApiScopeDto input, Volo.Abp.IdentityServer.ApiScopes.ApiScope apiScope)
    {
        foreach (var apiScopeClaimDto in input.UserClaims)
        {
            if (apiScope.FindClaim(apiScopeClaimDto.Type) == null)
            {
                apiScope.AddUserClaim(apiScopeClaimDto.Type);
            }
        }

        using var enumerator2 = apiScope.UserClaims.ToList().GetEnumerator();
        while (enumerator2.MoveNext())
        {
            var claim = enumerator2.Current;
            if (input.UserClaims.FirstOrDefault((ApiScopeClaimDto c) => claim.Equals(apiScope.Id, c.Type)) == null)
            {
                apiScope.RemoveClaim(claim.Type);
            }
        }
    }

    protected virtual void UpdateApiScopeProperties(UpdateApiScopeDto input, Volo.Abp.IdentityServer.ApiScopes.ApiScope apiScope)
    {
        foreach (var apiScopePropertyDto in input.Properties)
        {
            if (apiScope.FindProperty(apiScopePropertyDto.Key) == null)
            {
                apiScope.AddProperty(apiScopePropertyDto.Key, apiScopePropertyDto.Value);
            }
        }

        using var enumerator2 = apiScope.Properties.ToList().GetEnumerator();
        while (enumerator2.MoveNext())
        {
            var property = enumerator2.Current;
            if (input.Properties.FirstOrDefault((ApiScopePropertyDto c) => property.Equals(apiScope.Id, c.Key, c.Value)) == null)
            {
                apiScope.RemoveProperty(property.Key);
            }
        }
    }
}
