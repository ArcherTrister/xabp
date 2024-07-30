// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4.Models;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.ObjectExtending;

using X.Abp.IdentityServer.ApiResource.Dtos;
using X.Abp.IdentityServer.Permissions;

namespace X.Abp.IdentityServer.ApiResource;

[Authorize(AbpIdentityServerProPermissions.ApiResource.Default)]
public class ApiResourceAppService : IdentityServerAppServiceBase, IApiResourceAppService
{
    protected IApiResourceRepository ApiResourceRepository { get; }

    public ApiResourceAppService(IApiResourceRepository apiResourceRepository)
    {
        ApiResourceRepository = apiResourceRepository;
    }

    public virtual async Task<PagedResultDto<ApiResourceWithDetailsDto>> GetListAsync(GetApiResourceListInput input)
    {
        var resources = await ApiResourceRepository.GetListAsync(
                        input.Sorting, input.SkipCount, input.MaxResultCount, input.Filter);

        var count = await ApiResourceRepository.GetCountAsync(input.Filter);
        var list = ObjectMapper.Map<List<Volo.Abp.IdentityServer.ApiResources.ApiResource>, List<ApiResourceWithDetailsDto>>(resources);

        return new PagedResultDto<ApiResourceWithDetailsDto>(count, list);
    }

    public virtual async Task<List<ApiResourceWithDetailsDto>> GetAllListAsync()
    {
        var list = await ApiResourceRepository.GetListAsync();
        return ObjectMapper.Map<List<Volo.Abp.IdentityServer.ApiResources.ApiResource>, List<ApiResourceWithDetailsDto>>(list);
    }

    public virtual async Task<ApiResourceWithDetailsDto> GetAsync(Guid id)
    {
        var apiResource = await ApiResourceRepository.GetAsync(id);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.ApiResources.ApiResource, ApiResourceWithDetailsDto>(apiResource);
    }

    [Authorize(AbpIdentityServerProPermissions.ApiResource.Create)]
    public virtual async Task<ApiResourceWithDetailsDto> CreateAsync(CreateApiResourceDto input)
    {
        var nameExist = await ApiResourceRepository.CheckNameExistAsync(input.Name);
        if (nameExist)
        {
            throw new BusinessException("Volo.IdentityServer:DuplicateApiResourceName").WithData("Name", input.Name);
        }

        var id = GuidGenerator.Create();
        var apiResource = new Volo.Abp.IdentityServer.ApiResources.ApiResource(id, input.Name, input.DisplayName, input.Description)
        {
            ShowInDiscoveryDocument = input.ShowInDiscoveryDocument
        };

        foreach (var claim in input.UserClaims)
        {
            apiResource.AddUserClaim(claim.Type);
        }

        input.MapExtraPropertiesTo(apiResource);
        var result = await ApiResourceRepository.InsertAsync(apiResource);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.ApiResources.ApiResource, ApiResourceWithDetailsDto>(result);
    }

    [Authorize(AbpIdentityServerProPermissions.ApiResource.Update)]
    public virtual async Task<ApiResourceWithDetailsDto> UpdateAsync(Guid id, UpdateApiResourceDto input)
    {
        var apiResource = await ApiResourceRepository.GetAsync(id);
        apiResource.DisplayName = input.DisplayName;
        apiResource.Description = input.Description;
        apiResource.Enabled = input.Enabled;
        apiResource.ShowInDiscoveryDocument = input.ShowInDiscoveryDocument;

        UpdateApiResourceScope(input, apiResource);
        UpdateApiResourceSecrets(input, apiResource);
        UpdateApiResourceClaims(input, apiResource);

        apiResource.RemoveAllProperties();
        foreach (var item in input.Properties)
        {
            apiResource.AddProperty(item.Key, item.Value);
        }

        input.MapExtraPropertiesTo(apiResource);

        var result = await ApiResourceRepository.UpdateAsync(apiResource);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.ApiResources.ApiResource, ApiResourceWithDetailsDto>(result);
    }

    [Authorize(AbpIdentityServerProPermissions.ApiResource.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await ApiResourceRepository.DeleteAsync(id);
    }

    protected virtual void UpdateApiResourceScope(UpdateApiResourceDto input, Volo.Abp.IdentityServer.ApiResources.ApiResource apiResource)
    {
        foreach (var apiResourceScopeDto in input.Scopes)
        {
            if (apiResource.FindScope(apiResourceScopeDto.Scope) == null)
            {
                apiResource.AddScope(apiResourceScopeDto.Scope);
            }
        }

        using var enumerator2 = apiResource.Scopes.ToList().GetEnumerator();
        while (enumerator2.MoveNext())
        {
            var scope = enumerator2.Current;
            if (!input.Scopes.Any((ApiResourceScopeDto c) => scope.Equals(apiResource.Id, c.Scope)))
            {
                apiResource.RemoveScope(scope.Scope);
            }
        }
    }

    protected virtual void UpdateApiResourceSecrets(UpdateApiResourceDto input, Volo.Abp.IdentityServer.ApiResources.ApiResource apiResource)
    {
        foreach (var apiResourceSecretDto in input.Secrets)
        {
            if (apiResource.FindSecret(apiResourceSecretDto.Value, apiResourceSecretDto.Type) == null)
            {
                apiResourceSecretDto.Value = apiResourceSecretDto.Value.Sha256();
                apiResource.AddSecret(apiResourceSecretDto.Value, apiResourceSecretDto.Expiration, apiResourceSecretDto.Type, apiResourceSecretDto.Description);
            }
        }

        using var enumerator2 = apiResource.Secrets.ToList().GetEnumerator();
        while (enumerator2.MoveNext())
        {
            var clientSecret = enumerator2.Current;
            if (input.Secrets.FirstOrDefault((ApiResourceSecretDto s) => clientSecret.Equals(apiResource.Id, s.Value, s.Type)) == null)
            {
                apiResource.RemoveSecret(clientSecret.Value, clientSecret.Type);
            }
        }
    }

    protected virtual void UpdateApiResourceClaims(UpdateApiResourceDto input, Volo.Abp.IdentityServer.ApiResources.ApiResource apiResource)
    {
        foreach (var apiResourceClaimDto in input.UserClaims)
        {
            if (apiResource.FindClaim(apiResourceClaimDto.Type) == null)
            {
                apiResource.AddUserClaim(apiResourceClaimDto.Type);
            }
        }

        using var enumerator2 = apiResource.UserClaims.ToList().GetEnumerator();
        while (enumerator2.MoveNext())
        {
            var claim = enumerator2.Current;
            if (input.UserClaims.FirstOrDefault((ApiResourceClaimDto c) => claim.Equals(apiResource.Id, c.Type)) == null)
            {
                apiResource.RemoveClaim(claim.Type);
            }
        }
    }

    protected virtual void UpdateApiResourceProperties(UpdateApiResourceDto input, Volo.Abp.IdentityServer.ApiResources.ApiResource apiResource)
    {
        foreach (var apiResourcePropertyDto in input.Properties)
        {
            if (apiResource.FindProperty(apiResourcePropertyDto.Key) == null)
            {
                apiResource.AddProperty(apiResourcePropertyDto.Key, apiResourcePropertyDto.Value);
            }
        }

        using var enumerator2 = apiResource.Properties.ToList().GetEnumerator();
        while (enumerator2.MoveNext())
        {
            var property = enumerator2.Current;
            if (input.Properties.FirstOrDefault((ApiResourcePropertyDto c) => property.Equals(apiResource.Id, c.Key, c.Value)) == null)
            {
                apiResource.RemoveProperty(property.Key);
            }
        }
    }
}
