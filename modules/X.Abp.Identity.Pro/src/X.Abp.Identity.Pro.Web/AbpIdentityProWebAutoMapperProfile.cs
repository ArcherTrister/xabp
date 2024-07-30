// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AutoMapper;

using X.Abp.Identity.Web.Pages.Identity.Users;

using CreateClaimTypeModalModel = X.Abp.Identity.Web.Pages.Identity.ClaimTypes.CreateModalModel;
using CreateOrganizationUnitModalModel = X.Abp.Identity.Web.Pages.Identity.OrganizationUnits.CreateModalModel;
using CreateRoleModalModel = X.Abp.Identity.Web.Pages.Identity.Roles.CreateModalModel;
using CreateUserModalModel = X.Abp.Identity.Web.Pages.Identity.Users.CreateModalModel;
using EditClaimTypeModalModel = X.Abp.Identity.Web.Pages.Identity.ClaimTypes.EditModalModel;
using EditModalModel = X.Abp.Identity.Web.Pages.Identity.Roles.EditModalModel;
using EditOrganizationUnitModalModel = X.Abp.Identity.Web.Pages.Identity.OrganizationUnits.EditModalModel;
using EditRoleModalModel = X.Abp.Identity.Web.Pages.Identity.Roles.EditModalModel;
using EditUserModalModel = X.Abp.Identity.Web.Pages.Identity.Users.EditModalModel;
using RoleClaimTypeEditModalModel = X.Abp.Identity.Web.Pages.Identity.Roles.ClaimTypeEditModalModel;
using UserClaimTypeEditModalModel = X.Abp.Identity.Web.Pages.Identity.Users.ClaimTypeEditModalModel;

namespace X.Abp.Identity.Web;

public class AbpIdentityProWebAutoMapperProfile : Profile
{
    public AbpIdentityProWebAutoMapperProfile()
    {
        CreateUserMappings();
        CreateRoleMappings();
        CreateClaimTypeMappings();
        CreateOrganizationUnitMappings();
    }

    protected void CreateUserMappings()
    {
        // List
        CreateMap<IdentityUserDto, EditUserModalModel.UserInfoViewModel>()
            .MapExtraProperties();

        // CreateModal
        CreateMap<CreateUserModalModel.UserInfoViewModel, IdentityUserCreateDto>()
            .MapExtraProperties()
            .Ignore(x => x.RoleNames)
            .Ignore(x => x.OrganizationUnitIds);

        CreateMap<IdentityRoleDto, CreateUserModalModel.AssignedRoleViewModel>()
            .Ignore(x => x.IsAssigned);

        CreateMap<OrganizationUnitWithDetailsDto, IdentityUserModalPageModel.AssignedOrganizationUnitViewModel>()
            .Ignore(x => x.IsAssigned);

        // EditModal
        CreateMap<EditUserModalModel.UserInfoViewModel, IdentityUserUpdateDto>()
            .MapExtraProperties()
            .Ignore(x => x.RoleNames)
            .Ignore(x => x.OrganizationUnitIds);

        CreateMap<IdentityRoleDto, EditUserModalModel.AssignedRoleViewModel>()
            .Ignore(x => x.IsAssigned)
            .Ignore(x => x.IsInheritedFromOu);
    }

    protected void CreateClaimTypeMappings()
    {
        CreateMap<ClaimTypeDto, EditClaimTypeModalModel.ClaimTypeInfoModel>()
            .MapExtraProperties();

        CreateMap<ClaimTypeDto, UserClaimTypeEditModalModel.ClaimsViewModel>()
            .Ignore(x => x.Value);

        CreateMap<ClaimTypeDto, RoleClaimTypeEditModalModel.ClaimsViewModel>()
            .Ignore(x => x.Value);

        CreateMap<EditClaimTypeModalModel.ClaimTypeInfoModel, UpdateClaimTypeDto>()
            .MapExtraProperties();

        CreateMap<CreateClaimTypeModalModel.ClaimTypeInfoModel, CreateClaimTypeDto>()
            .MapExtraProperties();
    }

    protected void CreateRoleMappings()
    {
        // List
        CreateMap<IdentityRoleDto, EditRoleModalModel.RoleInfoModel>()
            .MapExtraProperties();

        // CreateModal
        CreateMap<CreateRoleModalModel.RoleInfoModel, IdentityRoleCreateDto>()
            .MapExtraProperties();

        // EditModal
        CreateMap<EditModalModel.RoleInfoModel, IdentityRoleUpdateDto>()
            .MapExtraProperties();
    }

    protected void CreateOrganizationUnitMappings()
    {
        // CreateModal
        CreateMap<CreateOrganizationUnitModalModel.OrganizationUnitInfoModel, OrganizationUnitCreateDto>()
            .MapExtraProperties();

        // EditModal
        CreateMap<OrganizationUnitWithDetailsDto, EditOrganizationUnitModalModel.OrganizationUnitInfoModel>();
        CreateMap<EditOrganizationUnitModalModel.OrganizationUnitInfoModel, OrganizationUnitUpdateDto>()
            .MapExtraProperties();
    }
}
