// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using X.Abp.OpenIddict.Applications.Dtos;
using X.Abp.OpenIddict.Scopes.Dtos;
using X.Abp.OpenIddict.Web.Pages.OpenIddict.Applications;
using X.Abp.OpenIddict.Web.Pages.OpenIddict.Scopes;

namespace X.Abp.OpenIddict.Web;

public class AbpOpenIddictProWebAutoMapperProfile : Profile
{
    public AbpOpenIddictProWebAutoMapperProfile()
    {
        CreateMap<ScopeCreateModalView, CreateScopeInput>()
            .MapExtraProperties()
            .ForMember(createScopeInput => createScopeInput.Resources, opts => opts.ConvertUsing(HashSetStringConverter.Converter, s => s.Resources));

        CreateMap<ScopeDto, ScopeEditModelView>()
            .MapExtraProperties()
            .ForMember(scopeEditModelView => scopeEditModelView.Resources, opts => opts.ConvertUsing(HashSetStringConverter.Converter, s => s.Resources));

        CreateMap<ScopeEditModelView, UpdateScopeInput>()
            .MapExtraProperties()
            .ForMember(updateScopeInput => updateScopeInput.Resources, opts => opts.ConvertUsing(HashSetStringConverter.Converter, s => s.Resources));

        CreateMap<ApplicationCreateModalView, CreateApplicationInput>()
            .MapExtraProperties()
            .ForMember(applicationInput => applicationInput.RedirectUris, opts => opts.ConvertUsing(HashSetStringConverter.Converter, a => a.RedirectUris))
            .ForMember(applicationInput => applicationInput.PostLogoutRedirectUris, opts => opts.ConvertUsing(HashSetStringConverter.Converter, a => a.PostLogoutRedirectUris));

        CreateMap<ApplicationDto, ApplicationEditModalView>()
            .MapExtraProperties()
            .ForMember(applicationEditModalView => applicationEditModalView.RedirectUris, opts => opts.ConvertUsing(HashSetStringConverter.Converter, a => a.RedirectUris))
            .ForMember(applicationEditModalView => applicationEditModalView.PostLogoutRedirectUris, opts => opts.ConvertUsing(HashSetStringConverter.Converter, a => a.PostLogoutRedirectUris));

        CreateMap<ApplicationEditModalView, UpdateApplicationInput>()
            .MapExtraProperties()
            .ForMember(applicationInput => applicationInput.RedirectUris, opts => opts.ConvertUsing(HashSetStringConverter.Converter, a => a.RedirectUris))
            .ForMember(applicationInput => applicationInput.PostLogoutRedirectUris, opts => opts.ConvertUsing(HashSetStringConverter.Converter, a => a.PostLogoutRedirectUris));
    }
}
