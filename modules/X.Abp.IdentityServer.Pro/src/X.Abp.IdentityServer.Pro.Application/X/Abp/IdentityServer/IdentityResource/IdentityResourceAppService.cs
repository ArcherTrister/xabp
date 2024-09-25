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
using Volo.Abp.IdentityServer.IdentityResources;
using Volo.Abp.ObjectExtending;

using X.Abp.IdentityServer.IdentityResource.Dtos;
using X.Abp.IdentityServer.Permissions;

namespace X.Abp.IdentityServer.IdentityResource;

[Authorize(AbpIdentityServerProPermissions.IdentityResource.Default)]
public class IdentityResourceAppService : IdentityServerAppServiceBase, IIdentityResourceAppService
{
    protected IIdentityResourceDataSeeder IdentityResourceDataSeeder { get; }

    protected IIdentityResourceRepository IdentityResourceRepository { get; }

    public IdentityResourceAppService(IIdentityResourceRepository identityResourceRepository, IIdentityResourceDataSeeder identityResourceDataSeeder)
    {
        IdentityResourceRepository = identityResourceRepository;
        IdentityResourceDataSeeder = identityResourceDataSeeder;
    }

    public virtual async Task<PagedResultDto<IdentityResourceWithDetailsDto>> GetListAsync(GetIdentityResourceListInput input)
    {
        var identityResources = await IdentityResourceRepository.GetListAsync(input.Sorting, input.SkipCount, input.MaxResultCount);
        var count = await IdentityResourceRepository.GetCountAsync();
        var list = ObjectMapper.Map<List<Volo.Abp.IdentityServer.IdentityResources.IdentityResource>, List<IdentityResourceWithDetailsDto>>(identityResources);
        return new PagedResultDto<IdentityResourceWithDetailsDto>(count, list);
    }

    public virtual async Task<List<IdentityResourceWithDetailsDto>> GetAllListAsync()
    {
        var list = await IdentityResourceRepository.GetListAsync();
        return ObjectMapper.Map<List<Volo.Abp.IdentityServer.IdentityResources.IdentityResource>, List<IdentityResourceWithDetailsDto>>(list);
    }

    public virtual async Task<IdentityResourceWithDetailsDto> GetAsync(Guid id)
    {
        var identityResource = await IdentityResourceRepository.GetAsync(id);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.IdentityResources.IdentityResource, IdentityResourceWithDetailsDto>(identityResource);
    }

    [Authorize(AbpIdentityServerProPermissions.IdentityResource.Create)]
    public virtual async Task<IdentityResourceWithDetailsDto> CreateAsync(CreateIdentityResourceDto input)
    {
        var nameExist = await IdentityResourceRepository.CheckNameExistAsync(input.Name);
        if (nameExist)
        {
            throw new BusinessException("Volo.IdentityServer:DuplicateIdentityResourceName").WithData("Name", input.Name);
        }

        var identityResource = new Volo.Abp.IdentityServer.IdentityResources.IdentityResource(
            GuidGenerator.Create(),
            input.Name,
            input.DisplayName,
            input.Description,
            input.Enabled,
            input.Required,
            input.Emphasize,
            input.ShowInDiscoveryDocument);

        foreach (var item in input.UserClaims)
        {
            identityResource.AddUserClaim(item.Type);
        }

        input.MapExtraPropertiesTo(identityResource);
        var result = await IdentityResourceRepository.InsertAsync(identityResource);

        identityResource = result;
        return ObjectMapper.Map<Volo.Abp.IdentityServer.IdentityResources.IdentityResource, IdentityResourceWithDetailsDto>(identityResource);
    }

    [Authorize(AbpIdentityServerProPermissions.IdentityResource.Update)]
    public virtual async Task<IdentityResourceWithDetailsDto> UpdateAsync(Guid id, UpdateIdentityResourceDto input)
    {
        var flag = await IdentityResourceRepository.CheckNameExistAsync(input.Name, id);
        if (flag)
        {
            throw new BusinessException("Volo.IdentityServer:DuplicateIdentityResourceName").WithData("Name", input.Name);
        }

        var identityResource = await IdentityResourceRepository.GetAsync(id);
        identityResource.Name = input.Name;
        identityResource.DisplayName = input.DisplayName;
        identityResource.Description = input.Description;
        identityResource.Enabled = input.Enabled;
        identityResource.Required = input.Required;
        identityResource.Emphasize = input.Emphasize;
        identityResource.ShowInDiscoveryDocument = input.ShowInDiscoveryDocument;

        UpdateIdentityResourceClaims(input, identityResource);

        identityResource.RemoveAllProperties();
        foreach (var item in input.Properties)
        {
            identityResource.AddProperty(item.Key, item.Value);
        }

        input.MapExtraPropertiesTo(identityResource);
        identityResource = await IdentityResourceRepository.UpdateAsync(identityResource);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.IdentityResources.IdentityResource, IdentityResourceWithDetailsDto>(identityResource);
    }

    [Authorize(AbpIdentityServerProPermissions.IdentityResource.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await IdentityResourceRepository.DeleteAsync(id);
    }

    [Authorize(AbpIdentityServerProPermissions.IdentityResource.Create)]
    public virtual async Task CreateStandardResourcesAsync()
    {
        await IdentityResourceDataSeeder.CreateStandardResourcesAsync();
    }

    protected virtual void UpdateIdentityResourceClaims(UpdateIdentityResourceDto input, Volo.Abp.IdentityServer.IdentityResources.IdentityResource identityResource)
    {
        foreach (var identityResourceClaimDto in input.UserClaims)
        {
            if (identityResource.FindUserClaim(identityResourceClaimDto.Type) == null)
            {
                identityResource.AddUserClaim(identityResourceClaimDto.Type);
            }
        }

        foreach (var identityClaim in identityResource.UserClaims)
        {
            if (!input.UserClaims.Any(c => identityClaim.Equals(identityResource.Id, c.Type)))
            {
                identityResource.RemoveUserClaim(identityClaim.Type);
            }
        }
    }

    protected virtual void UpdateIdentityResourceProperties(UpdateIdentityResourceDto input, Volo.Abp.IdentityServer.IdentityResources.IdentityResource identityResource)
    {
        foreach (var identityResourcePropertyDto in input.Properties)
        {
            if (identityResource.FindProperty(identityResourcePropertyDto.Key) == null)
            {
                identityResource.AddProperty(identityResourcePropertyDto.Key, identityResourcePropertyDto.Value);
            }
        }

        using var enumerator = identityResource.Properties.ToList().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var property = enumerator.Current;
            if (!input.Properties.Any((c) => property.Equals(identityResource.Id, c.Key, c.Value)))
            {
                identityResource.RemoveProperty(property.Key);
            }
        }
    }
}
