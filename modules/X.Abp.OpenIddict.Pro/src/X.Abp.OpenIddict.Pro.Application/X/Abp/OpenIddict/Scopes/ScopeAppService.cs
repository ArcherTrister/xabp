// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using OpenIddict.Abstractions;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;
using Volo.Abp.OpenIddict.Scopes;

using X.Abp.OpenIddict.Permissions;
using X.Abp.OpenIddict.Scopes.Dtos;

namespace X.Abp.OpenIddict.Scopes;

[Authorize(AbpOpenIddictProPermissions.Scope.Default)]
public class ScopeAppService : OpenIddictProAppServiceBase, IScopeAppService
{
    private static readonly List<ScopeDto> ScopeList = new List<ScopeDto>(5)
    {
      new ScopeDto()
      {
        Name = "address",
        BuildIn = true
      },
      new ScopeDto()
      {
        Name = "email",
        BuildIn = true
      },
      new ScopeDto()
      {
        Name = "phone",
        BuildIn = true
      },
      new ScopeDto()
      {
        Name = "profile",
        BuildIn = true
      },
      new ScopeDto()
      {
        Name = "roles",
        BuildIn = true
      }
    };

    protected IOpenIddictScopeManager ScopeManager { get; }

    protected IOpenIddictScopeRepository ScopeRepository { get; }

    public ScopeAppService(
      IOpenIddictScopeManager scopeManager,
      IOpenIddictScopeRepository scopeRepository)
    {
        ScopeManager = scopeManager;
        ScopeRepository = scopeRepository;
    }

    public virtual async Task<ScopeDto> GetAsync(Guid id)
    {
        var scope = await ScopeManager.FindByIdAsync(ConvertIdentifierToString(id)) ?? throw new EntityNotFoundException(typeof(OpenIddictScopeModel), id);
        return ObjectMapper.Map<OpenIddictScopeModel, ScopeDto>(scope.As<OpenIddictScopeModel>());
    }

    public virtual async Task<PagedResultDto<ScopeDto>> GetListAsync(
      GetScopeListInput input)
    {
        var scopes = await ScopeRepository.GetListAsync(input.Sorting, input.SkipCount, input.MaxResultCount, input.Filter);
        return new PagedResultDto<ScopeDto>(await ScopeRepository.GetCountAsync(input.Filter), ObjectMapper.Map<List<OpenIddictScopeModel>, List<ScopeDto>>(scopes.Select(x => x.ToModel()).ToList()));
    }

    [Authorize(AbpOpenIddictProPermissions.Scope.Create)]
    public virtual async Task<ScopeDto> CreateAsync(CreateScopeInput input)
    {
        await CheckInputDtoAsync(input);
        OpenIddictScopeDescriptor descriptor = new()
        {
            Description = input.Description,
            DisplayName = input.DisplayName,
            Name = input.Name
        };
        if (!input.Resources.IsNullOrEmpty())
        {
            descriptor.Resources.UnionWith(input.Resources);
        }

        var scope = await ScopeManager.CreateAsync(descriptor);
        input.MapExtraPropertiesTo(scope.As<OpenIddictScopeModel>(), null, null);
        await ScopeManager.UpdateAsync(scope);
        return ObjectMapper.Map<OpenIddictScopeModel, ScopeDto>(scope.As<OpenIddictScopeModel>());
    }

    [Authorize(AbpOpenIddictProPermissions.Scope.Update)]
    public virtual async Task<ScopeDto> UpdateAsync(Guid id, UpdateScopeInput input)
    {
        var scope = (await ScopeManager.FindByIdAsync(ConvertIdentifierToString(id))).As<OpenIddictScopeModel>() ?? throw new EntityNotFoundException(typeof(OpenIddictScopeModel), id);
        await CheckInputDtoAsync(input, scope);
        OpenIddictScopeDescriptor descriptor = new();
        await ScopeManager.PopulateAsync(descriptor, scope);
        descriptor.Description = input.Description;
        descriptor.DisplayName = input.DisplayName;
        descriptor.Name = input.Name;
        descriptor.Resources.Clear();
        if (!input.Resources.IsNullOrEmpty())
        {
            descriptor.Resources.UnionWith(input.Resources);
        }

        input.MapExtraPropertiesTo(scope.As<OpenIddictScopeModel>(), null);
        await ScopeManager.UpdateAsync(scope, descriptor);
        return ObjectMapper.Map<OpenIddictScopeModel, ScopeDto>(scope);
    }

    [Authorize(AbpOpenIddictProPermissions.Scope.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var scope = await ScopeManager.FindByIdAsync(ConvertIdentifierToString(id)) ?? throw new EntityNotFoundException(typeof(OpenIddictScopeModel), id);
        await ScopeManager.DeleteAsync(scope);
    }

    public virtual async Task<List<ScopeDto>> GetAllScopesAsync()
    {
        List<ScopeDto> scopes = ScopeList;

        var asyncEnumerator = ScopeManager.ListAsync().GetAsyncEnumerator();
        try
        {
            while (await asyncEnumerator.MoveNextAsync())
            {
                var current = asyncEnumerator.Current;
                scopes.Add(ObjectMapper.Map<OpenIddictScopeModel, ScopeDto>(current.As<OpenIddictScopeModel>()));
            }
        }
        finally
        {
            if (asyncEnumerator != null)
            {
                await asyncEnumerator.DisposeAsync();
            }
        }

        return scopes;
    }

    protected virtual async Task CheckInputDtoAsync(
      ScopeCreateOrUpdateDtoBase dto,
      OpenIddictScopeModel scope = null)
    {
        if (await ScopeManager.FindByNameAsync(dto.Name, default) != null && (scope == null || scope.Name != dto.Name))
        {
            throw new UserFriendlyException(L["TheNameIsAlreadyTakenByAnotherScope"]);
        }
    }
}
