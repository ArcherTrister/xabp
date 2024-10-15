// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.Csv;
using MiniExcelLibs.OpenXml;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Uow;

using X.Abp.Identity.Permissions;

namespace X.Abp.Identity;

[Authorize(AbpIdentityProPermissions.Users.Default)]
public class IdentityUserAppService : IdentityAppServiceBase, IIdentityUserAppService
{
    protected IdentityUserManager UserManager { get; }

    protected IIdentityUserRepository UserRepository { get; }

    protected IIdentityRoleRepository RoleRepository { get; }

    protected IOrganizationUnitRepository OrganizationUnitRepository { get; }

    protected IIdentityClaimTypeRepository IdentityClaimTypeRepository { get; }

    protected IdentityProTwoFactorManager IdentityProTwoFactorManager { get; }

    protected IOptions<IdentityOptions> IdentityOptions { get; }

    protected IOptions<AbpIdentityOptions> AbpIdentityOptions { get; }

    protected IApplicationInfoAccessor ApplicationInfoAccessor { get; }

    protected IDistributedEventBus DistributedEventBus { get; }

    protected IPermissionChecker PermissionChecker { get; }

    protected IDistributedCache<IdentityUserDownloadTokenCacheItem, string> DownloadTokenCache { get; }

    protected IDistributedCache<ImportInvalidUsersCacheItem, string> ImportInvalidUsersCache { get; }

    public IdentityUserAppService(
        IdentityUserManager userManager,
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IOrganizationUnitRepository organizationUnitRepository,
        IIdentityClaimTypeRepository identityClaimTypeRepository,
        IdentityProTwoFactorManager identityProTwoFactorManager,
        IOptions<IdentityOptions> identityOptions,
        IOptions<AbpIdentityOptions> abpIdentityOptions,
        IApplicationInfoAccessor applicationInfoAccessor,
        IDistributedEventBus distributedEventBus,
        IPermissionChecker permissionChecker,
        IDistributedCache<IdentityUserDownloadTokenCacheItem, string> downloadTokenCache,
        IDistributedCache<ImportInvalidUsersCacheItem, string> importInvalidUsersCache)
    {
        UserManager = userManager;
        UserRepository = userRepository;
        RoleRepository = roleRepository;
        OrganizationUnitRepository = organizationUnitRepository;
        IdentityClaimTypeRepository = identityClaimTypeRepository;
        IdentityProTwoFactorManager = identityProTwoFactorManager;
        IdentityOptions = identityOptions;
        AbpIdentityOptions = abpIdentityOptions;
        ApplicationInfoAccessor = applicationInfoAccessor;
        DistributedEventBus = distributedEventBus;
        PermissionChecker = permissionChecker;
        DownloadTokenCache = downloadTokenCache;
        ImportInvalidUsersCache = importInvalidUsersCache;
    }

    public virtual async Task<IdentityUserDto> GetAsync(Guid id)
    {
        return await FindByIdInternalAsync(id)
            ?? throw new EntityNotFoundException(typeof(IdentityUser), id);
    }

    public virtual async Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetIdentityUsersInput input)
    {
        var count = await UserRepository.GetCountAsync(
            input.Filter,
            input.RoleId,
            input.OrganizationUnitId,
            input.UserName,
            input.PhoneNumber,
            input.EmailAddress,
            input.Name,
            input.Surname,
            input.IsLockedOut,
            input.NotActive,
            input.EmailConfirmed,
            input.IsExternal,
            input.MaxCreationTime,
            input.MinCreationTime,
            input.MaxModifitionTime,
            input.MinModifitionTime);

        var users = await UserRepository.GetListAsync(
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.Filter,
            includeDetails: true,
            input.RoleId,
            input.OrganizationUnitId,
            input.UserName,
            input.PhoneNumber,
            input.EmailAddress,
            input.Name,
            input.Surname,
            input.IsLockedOut,
            input.NotActive,
            input.EmailConfirmed,
            input.IsExternal,
            input.MaxCreationTime,
            input.MinCreationTime,
            input.MaxModifitionTime,
            input.MinModifitionTime);

        var userRoleIds = users.SelectMany(x => x.Roles).Select(x => x.RoleId).Distinct().ToList();

        var userRoles = await RoleRepository.GetListAsync(userRoleIds);

        var userDtos = ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(users);

        var twoFactorEnabled = await IdentityProTwoFactorManager.IsOptionalAsync();
        for (var i = 0; i < users.Count; i++)
        {
            userDtos[i].IsLockedOut =
                users[i].LockoutEnabled
                && users[i].LockoutEnd != null
                && users[i].LockoutEnd > DateTime.UtcNow;
            if (!userDtos[i].IsLockedOut)
            {
                userDtos[i].LockoutEnd = null;
            }

            userDtos[i].SupportTwoFactor = twoFactorEnabled;
            userDtos[i].RoleNames = userRoles
                .Where(x => users[i].Roles.Any(q => q.RoleId == x.Id))
                .Select(x => x.Name)
                .ToList();
        }

        return new PagedResultDto<IdentityUserDto>(count, userDtos);
    }

    public virtual async Task<ListResultDto<IdentityRoleDto>> GetRolesAsync(Guid id)
    {
        var roles = await UserRepository.GetRolesAsync(id);
        return new ListResultDto<IdentityRoleDto>(ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(roles));
    }

    public virtual async Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
    {
        var list = await RoleRepository.GetListAsync();
        return new ListResultDto<IdentityRoleDto>(ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list));
    }

    public virtual async Task<ListResultDto<OrganizationUnitWithDetailsDto>> GetAvailableOrganizationUnitsAsync()
    {
        var organizationUnits = await OrganizationUnitRepository.GetListAsync(true);
        var roleLookup = await GetRoleLookup(organizationUnits);
        var ouDtos = new List<OrganizationUnitWithDetailsDto>();
        foreach (var ou in organizationUnits)
        {
            ouDtos.Add(await ConvertToOrganizationUnitWithDetailsDtoAsync(ou, roleLookup));
        }

        return new ListResultDto<OrganizationUnitWithDetailsDto>(ouDtos);
    }

    public virtual async Task<List<ClaimTypeDto>> GetAllClaimTypesAsync()
    {
        var claimTypes = await IdentityClaimTypeRepository.GetListAsync();

        var dtos = ObjectMapper.Map<List<IdentityClaimType>, List<ClaimTypeDto>>(claimTypes);
        foreach (var dto in dtos)
        {
            dto.ValueTypeAsString = dto.ValueType.ToString();
        }

        return dtos;
    }

    public virtual async Task<List<IdentityUserClaimDto>> GetClaimsAsync(Guid id)
    {
        var user = await UserRepository.GetAsync(id);
        return new List<IdentityUserClaimDto>(ObjectMapper.Map<List<IdentityUserClaim>, List<IdentityUserClaimDto>>(user.Claims.ToList()));
    }

    public virtual async Task<List<OrganizationUnitDto>> GetOrganizationUnitsAsync(Guid id)
    {
        var organizationUnits = await UserRepository.GetOrganizationUnitsAsync(id, true);
        return new List<OrganizationUnitDto>(ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(organizationUnits));
    }

    [Authorize(AbpIdentityProPermissions.Users.Create)]
    public virtual async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
    {
        await IdentityOptions.SetAsync();

        var user = new IdentityUser(
            GuidGenerator.Create(),
            input.UserName,
            input.Email,
            CurrentTenant.Id);

        input.MapExtraPropertiesTo(user);

        (await UserManager.CreateAsync(user, input.Password)).CheckIdentityErrors();
        await UpdateUserByInput(user, input);
        (await UserManager.UpdateAsync(user)).CheckIdentityErrors();
        await CurrentUnitOfWork.SaveChangesAsync();

        await DistributedEventBus.PublishAsync(
            new IdentityUserCreatedEto()
            {
                Id = user.Id,
                Properties =
                {
                    {
                        "SendConfirmationEmail",
                        input.SendConfirmationEmail.ToString().ToUpper(CultureInfo.CurrentCulture)
                    },
                    { "AppName", ApplicationInfoAccessor.ApplicationName }
                }
            });

        var userDto = ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);

        return userDto;
    }

    [Authorize(AbpIdentityProPermissions.Users.Update)]
    public virtual async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input)
    {
        await IdentityOptions.SetAsync();

        var user = await UserManager.GetByIdAsync(id);

        user.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        (await UserManager.SetUserNameAsync(user, input.UserName)).CheckIdentityErrors();
        await UpdateUserByInput(user, input);
        input.MapExtraPropertiesTo(user);
        (await UserManager.UpdateAsync(user)).CheckIdentityErrors();
        await CurrentUnitOfWork.SaveChangesAsync();

        var userDto = ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);

        return userDto;
    }

    [Authorize(AbpIdentityProPermissions.Users.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        if (CurrentUser.Id == id)
        {
            throw new BusinessException(code: IdentityErrorCodes.UserSelfDeletion);
        }

        var user = await UserManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return;
        }

        (await UserManager.DeleteAsync(user)).CheckIdentityErrors();
    }

    [Authorize(AbpIdentityProPermissions.Users.Update)]
    public virtual async Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input)
    {
        var user = await UserManager.GetByIdAsync(id);
        (await UserManager.SetRolesAsync(user, input.RoleNames)).CheckIdentityErrors();
        await UserRepository.UpdateAsync(user);
    }

    [Authorize(AbpIdentityProPermissions.Users.Update)]
    public virtual async Task UpdateClaimsAsync(Guid id, List<IdentityUserClaimDto> input)
    {
        var user = await UserRepository.GetAsync(id);

        foreach (var claim in input)
        {
            var existing = user.FindClaim(new Claim(claim.ClaimType, claim.ClaimValue));
            if (existing == null)
            {
                user.AddClaim(GuidGenerator, new Claim(claim.ClaimType, claim.ClaimValue));
            }
        }

        // Copied with ToList to avoid modification of the collection in the for loop
        foreach (var claim in user.Claims.ToList())
        {
            if (!input.Any(c => claim.ClaimType == c.ClaimType && claim.ClaimValue == c.ClaimValue))
            {
                user.RemoveClaim(new Claim(claim.ClaimType, claim.ClaimValue));
            }
        }

        await UserRepository.UpdateAsync(user);
    }

    [Authorize(AbpIdentityProPermissions.Users.Update)]
    public virtual async Task LockAsync(Guid id, DateTime lockoutEnd)
    {
        var user = await UserManager.GetByIdAsync(id);
        if (!await UserManager.GetLockoutEnabledAsync(user))
        {
            throw new UserFriendlyException(L["UserLockoutNotEnabled{0}", user.UserName]);
        }

        lockoutEnd = DateTime.SpecifyKind(lockoutEnd, DateTimeKind.Utc);
        (await UserManager.SetLockoutEndDateAsync(user, lockoutEnd)).CheckIdentityErrors();
    }

    [Authorize(AbpIdentityProPermissions.Users.Update)]
    public virtual async Task UnlockAsync(Guid id)
    {
        var user = await UserManager.GetByIdAsync(id);
        if (!await UserManager.GetLockoutEnabledAsync(user))
        {
            throw new UserFriendlyException(L["UserLockoutNotEnabled{0}", user.UserName]);
        }

        (await UserManager.SetLockoutEndDateAsync(user, null)).CheckIdentityErrors();
    }

    [Authorize(AbpIdentityProPermissions.Users.Update)]
    public virtual async Task UpdatePasswordAsync(Guid id, IdentityUserUpdatePasswordInput input)
    {
        await IdentityOptions.SetAsync();

        var user = await UserManager.GetByIdAsync(id);
        (await UserManager.RemovePasswordAsync(user)).CheckIdentityErrors();
        (await UserManager.AddPasswordAsync(user, input.NewPassword)).CheckIdentityErrors();
    }

    public virtual async Task<IdentityUserDto> FindByUsernameAsync(string username)
    {
        var userDto = ObjectMapper.Map<IdentityUser, IdentityUserDto>(await UserManager.FindByNameAsync(username));

        return userDto;
    }

    public virtual async Task<IdentityUserDto> FindByEmailAsync(string email)
    {
        var userDto = ObjectMapper.Map<IdentityUser, IdentityUserDto>(await UserManager.FindByEmailAsync(email));

        return userDto;
    }

    public virtual async Task<IdentityUserDto> FindByPhoneNumberAsync(string phoneNumber)
    {
        var userDto = ObjectMapper.Map<IdentityUser, IdentityUserDto>(await UserRepository.FindByPhoneNumberAsync(phoneNumber));

        return userDto;
    }

    public virtual async Task<bool> GetTwoFactorEnabledAsync(Guid id)
    {
        var user = await UserManager.GetByIdAsync(id);
        return await UserManager.GetTwoFactorEnabledAsync(user);
    }

    [Authorize(AbpIdentityProPermissions.Users.Update)]
    public virtual async Task SetTwoFactorEnabledAsync(Guid id, bool enabled)
    {
        if (await IdentityProTwoFactorManager.IsOptionalAsync())
        {
            var user = await UserManager.GetByIdAsync(id);
            if (user.TwoFactorEnabled != enabled)
            {
                (await UserManager.SetTwoFactorEnabledAsync(user, enabled)).CheckIdentityErrors();
            }
        }
        else
        {
            throw new BusinessException(code: IdentityErrorCodes.CanNotChangeTwoFactor);
        }
    }

    public virtual async Task<List<IdentityRoleLookupDto>> GetRoleLookupAsync()
    {
        var roles = await RoleRepository.GetListAsync();

        return ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleLookupDto>>(roles);
    }

    public virtual async Task<List<OrganizationUnitLookupDto>> GetOrganizationUnitLookupAsync()
    {
        var organizationUnits = await OrganizationUnitRepository.GetListAsync();

        return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitLookupDto>>(organizationUnits);
    }

    [Authorize(AbpIdentityProPermissions.Users.Import)]
    public virtual async Task<List<ExternalLoginProviderDto>> GetExternalLoginProvidersAsync()
    {
        var providers = new List<ExternalLoginProviderDto>();

        foreach (var externalLoginProvider in AbpIdentityOptions.Value.ExternalLoginProviders)
        {
            var provider = LazyServiceProvider
                .LazyGetRequiredService(externalLoginProvider.Value.Type)
                .As<IExternalLoginProvider>();

            if (await provider.IsEnabledAsync())
            {
                var canObtainUserInfoWithoutPassword = true;
                if (provider is IExternalLoginProviderWithPassword providerWithPassword)
                {
                    canObtainUserInfoWithoutPassword =
                        providerWithPassword.CanObtainUserInfoWithoutPassword;
                }

                providers.Add(new ExternalLoginProviderDto(externalLoginProvider.Value.Name, canObtainUserInfoWithoutPassword));
            }
        }

        return providers;
    }

    [Authorize(AbpIdentityProPermissions.Users.Import)]
    public virtual async Task<IdentityUserDto> ImportExternalUserAsync(ImportExternalUserInput input)
    {
        if (!AbpIdentityOptions.Value.ExternalLoginProviders.TryGetValue(input.Provider, out var providerInfo))
        {
            throw new BusinessException(IdentityProErrorCodes.InvalidExternalLoginProvider);
        }

        var provider = LazyServiceProvider
            .LazyGetRequiredService(providerInfo.Type)
            .As<IExternalLoginProvider>();
        var user =
            await UserManager.FindByNameAsync(input.UserNameOrEmailAddress)
            ?? await UserManager.FindByEmailAsync(input.UserNameOrEmailAddress);

        if (provider is IExternalLoginProviderWithPassword)
        {
            if (!provider.As<IExternalLoginProviderWithPassword>().CanObtainUserInfoWithoutPassword
                && !await provider.TryAuthenticateAsync(
                    input.UserNameOrEmailAddress,
                    input.Password))
            {
                throw new BusinessException(IdentityProErrorCodes.ExternalLoginProviderAuthenticateFailed);
            }
        }

        if (user == null)
        {
            user = provider is IExternalLoginProviderWithPassword providerWithPassword
                ? await providerWithPassword.CreateUserAsync(
                    input.UserNameOrEmailAddress,
                    input.Provider,
                    input.Password)
                : await provider.CreateUserAsync(input.UserNameOrEmailAddress, input.Provider);
        }
        else
        {
            if (!user.IsExternal)
            {
                throw new BusinessException(IdentityProErrorCodes.LocalUserAlreadyExists);
            }

            if (provider is IExternalLoginProviderWithPassword providerWithPassword)
            {
                await providerWithPassword.UpdateUserAsync(user, input.Provider, input.Password);
            }
            else
            {
                await provider.UpdateUserAsync(user, input.Provider);
            }
        }

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
    }

    public virtual async Task<IdentityUserDto> FindByIdAsync(Guid id)
    {
        return await FindByIdInternalAsync(id);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(GetIdentityUserListAsFileInput input)
    {
        List<IdentityUserExportDto> identityUserExportDtoList = await GetExportUsersAsync(input);
        using MemoryStream memoryStream = new MemoryStream();
        await MiniExcel.SaveAsAsync(
            memoryStream,
            identityUserExportDtoList,
            true,
            "Sheet1",
            ExcelType.XLSX,
            GetExcelConfiguration(ExcelType.XLSX));

        memoryStream.Seek(0L, SeekOrigin.Begin);
        return new RemoteStreamContent(memoryStream, "UserList.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsCsvFileAsync(GetIdentityUserListAsFileInput input)
    {
        List<IdentityUserExportDto> identityUserExportDtoList = await GetExportUsersAsync(input);
        using MemoryStream memoryStream = new MemoryStream();
        await MiniExcel.SaveAsAsync(
            memoryStream,
            identityUserExportDtoList,
            true,
            "Sheet1",
            ExcelType.CSV,
            GetExcelConfiguration(ExcelType.CSV));
        memoryStream.Seek(0L, SeekOrigin.Begin);
        return new RemoteStreamContent(memoryStream, "UserList.csv", "text/csv");
    }

    public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        string token = Guid.NewGuid().ToString("n");
        IdentityUserDownloadTokenCacheItem downloadTokenCacheItem =
            new IdentityUserDownloadTokenCacheItem { Token = token, TenantId = CurrentTenant.Id };
        DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = new TimeSpan?(TimeSpan.FromSeconds(30.0))
        };
        await DownloadTokenCache.SetAsync(token, downloadTokenCacheItem, options);
        return new DownloadTokenResultDto() { Token = token };
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetImportUsersSampleFileAsync(GetImportUsersSampleFileInput input)
    {
        await CheckDownloadTokenAsync(input.Token);
        return await GetImportUsersFileAsync(
            new List<ImportUsersFromFileDto>()
            {
                new ImportUsersFromFileDto()
                {
                    UserName = "zhangsan",
                    Name = "三",
                    Surname = "张",
                    EmailAddress = "zhangsan@qq.com",
                    PhoneNumber = "13888888888",
                    Password = "1q2w3E*",
                    AssignedRoleNames = "admin"
                },
                new ImportUsersFromFileDto()
                {
                    UserName = "wangwu",
                    Name = "五",
                    Surname = "王",
                    EmailAddress = "wangwu@qq.com",
                    PhoneNumber = "13666666666",
                    Password = "1q2w3E*",
                    AssignedRoleNames = "admin;test"
                }
            },
            "ImportUsersSampleFile",
            input.FileType);
    }

    [Authorize(AbpIdentityProPermissions.Users.Import)]
    public virtual async Task<ImportUsersFromFileOutput> ImportUsersFromFileAsync(ImportUsersFromFileInputWithStream input)
    {
        await IdentityOptions.SetAsync();
        MemoryStream stream = new MemoryStream();
        await input.File.GetStream().CopyToAsync(stream);
        List<InvalidImportUsersFromFileDto> invalidUsers =
            new List<InvalidImportUsersFromFileDto>();
        List<InvalidImportUsersFromFileDto> list;
        try
        {
            IConfiguration configuration = null;
            if (input.FileType == ImportUsersFromFileType.Csv)
            {
                configuration = new CsvConfiguration() { Seperator = ';' };
            }

            list = (await MiniExcel.QueryAsync<InvalidImportUsersFromFileDto>(
                    stream,
                    null,
                    input.FileType == ImportUsersFromFileType.Excel ? ExcelType.XLSX : ExcelType.CSV,
                    "A2",
                    configuration)).ToList();
        }
        catch (Exception)
        {
            throw new BusinessException("Volo.Abp.Identity:010014");
        }

        ImportUsersFromFileOutput resultDto =
            list.Count != 0
                ? new ImportUsersFromFileOutput() { AllCount = list.Count }
                : throw new BusinessException("Volo.Abp.Identity:010013");

        foreach (InvalidImportUsersFromFileDto waitingImportUser in list)
        {
            using (IUnitOfWork uow = UnitOfWorkManager.Begin(true, true))
            {
                try
                {
                    IdentityUser user = new IdentityUser(
                        GuidGenerator.Create(),
                        waitingImportUser.UserName,
                        waitingImportUser.EmailAddress,
                        CurrentTenant.Id)
                    {
                        Surname = waitingImportUser.Surname,
                        Name = waitingImportUser.Name
                    };
                    if (!waitingImportUser.PhoneNumber.IsNullOrWhiteSpace())
                    {
                        user.SetPhoneNumber(waitingImportUser.PhoneNumber, false);
                    }

                    if (!waitingImportUser.Password.IsNullOrWhiteSpace())
                    {
                        (await UserManager.CreateAsync(user, waitingImportUser.Password)).CheckIdentityErrors();
                    }
                    else
                    {
                        (await UserManager.CreateAsync(user)).CheckIdentityErrors();
                    }

                    if (!waitingImportUser.AssignedRoleNames.IsNullOrWhiteSpace())
                    {
                        (await UserManager.SetRolesAsync(user,
                            waitingImportUser.AssignedRoleNames.Split(";").Select(role => role.Trim()).Where(role => !role.IsNullOrWhiteSpace())))
                            .CheckIdentityErrors();
                    }

                    await uow.CompleteAsync();
                }
                catch (Exception ex)
                {
                    waitingImportUser.ErrorReason =
                        ex is UserFriendlyException ? ex.Message : ex.ToString();
                    Logger.LogWarning(ex, $"Import user failed: {waitingImportUser}");
                    await uow.RollbackAsync();
                }
            }
        }

        if (invalidUsers.Count != 0)
        {
            string token = Guid.NewGuid().ToString("n");

            ImportInvalidUsersCacheItem invalidUsersCacheItem = new ImportInvalidUsersCacheItem
            {
                Token = token,
                InvalidUsers = invalidUsers,
                FileType = input.FileType
            };
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = new TimeSpan?(TimeSpan.FromMinutes(1.0))
            };

            await ImportInvalidUsersCache.SetAsync(token, invalidUsersCacheItem, options);
            resultDto.InvalidUsersDownloadToken = token;
        }

        resultDto.SucceededCount = resultDto.AllCount - invalidUsers.Count;
        resultDto.FailedCount = invalidUsers.Count;
        return resultDto;
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetImportInvalidUsersFileAsync(GetImportInvalidUsersFileInput input)
    {
        IDownloadCacheItem downloadCacheItem = await CheckDownloadTokenAsync(input.Token, true);
        ImportInvalidUsersCacheItem invalidUsersCacheItem = await ImportInvalidUsersCache.GetAsync(input.Token);

        return await GetImportUsersFileAsync(
            invalidUsersCacheItem.InvalidUsers,
            "InvalidUsers",
            invalidUsersCacheItem.FileType);
    }

    /*
    /// <summary>
    /// 管理员重置密码
    /// </summary>
    /// <param name="id">用户Id</param>
    /// <param name="input">AdminResetPasswordInput</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task AdminResetPasswordAsync(Guid id, AdminResetPasswordInput input)
    {
        // var user = await UserManager.GetByIdAsync(id);
        // var token = await UserManager.GeneratePasswordResetTokenAsync(user);
        // await UserManager.ResetPasswordAsync(user, token, input.NewPassword);
        IdentityUser currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());
        if (currentUser.IsDeleted || !currentUser.IsActive)
        {
            throw new UserFriendlyException("当前管理员被删除或被禁用!");
        }

        bool check = await UserManager.CheckPasswordAsync(currentUser, input.AdminPassword);
        if (!check)
        {
            throw new UserFriendlyException("你的管理员密码不正确，请重试!");
        }

        IList<string> roles = await UserManager.GetRolesAsync(currentUser);
        if (!roles.Contains("admin"))
        {
            throw new UserFriendlyException("只有管理员才能重置密码!");
        }

        IdentityUser user = await UserManager.GetByIdAsync(id);
        if (user != null)
        {
            await UserManager.RemovePasswordAsync(user);

            // await UserManager.AddPasswordAsync(user, input.NewPassword);
            IdentityResult identityResult = await UserManager.AddPasswordAsync(user, input.NewPassword);
            if (!identityResult.Succeeded)
            {
                throw new UserFriendlyException("密码强度不符合,必须包含大、小写字母、数字和特殊符号!");
            }
        }
    }
    */

    protected virtual async Task UpdateUserByInput(
        IdentityUser user,
        IdentityUserCreateOrUpdateDtoBase input)
    {
        if (!string.Equals(user.Email, input.Email, StringComparison.OrdinalIgnoreCase))
        {
            (await UserManager.SetEmailAsync(user, input.Email)).CheckIdentityErrors();
        }

        if (
            !string.Equals(
                user.PhoneNumber,
                input.PhoneNumber,
                StringComparison.OrdinalIgnoreCase))
        {
            (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckIdentityErrors();
        }

        (await UserManager.SetLockoutEnabledAsync(user, input.LockoutEnabled)).CheckIdentityErrors();
        user.Name = input.Name;
        user.Surname = input.Surname;
        (await UserManager.UpdateAsync(user)).CheckIdentityErrors();
        user.SetIsActive(input.IsActive);
        user.SetShouldChangePasswordOnNextLogin(input.ShouldChangePasswordOnNextLogin);

        if (await PermissionChecker.IsGrantedAsync(AbpIdentityProPermissions.Users.ManageRoles)
            && input.RoleNames != null)
        {
            await UpdateUserRolesBasedOnOrganizationUnits(user, input);

            (await UserManager.SetRolesAsync(user, input.RoleNames)).CheckIdentityErrors();
        }

        if (
            await PermissionChecker.IsGrantedAsync(AbpIdentityProPermissions.Users.ManageOU)
            && input.OrganizationUnitIds != null)
        {
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnitIds);
        }
    }

    protected virtual async Task UpdateUserRolesBasedOnOrganizationUnits(
        IdentityUser user,
        IdentityUserCreateOrUpdateDtoBase input)
    {
        if (input.OrganizationUnitIds == null)
        {
            input.OrganizationUnitIds = Array.Empty<Guid>();
        }

        Guid[] organizationUnits = user
            .OrganizationUnits.Select(x => x.OrganizationUnitId)
            .Except(input.OrganizationUnitIds)
            .Union(input.OrganizationUnitIds)
            .Distinct()
            .ToArray();
        if (organizationUnits.Length != 0)
        {
            List<IdentityRole> source = await OrganizationUnitRepository.GetRolesAsync(
                organizationUnits,
                null,
                int.MaxValue,
                0,
                true);
            if (source.Count != 0)
            {
                IdentityRole[] array2 = source
                    .Where(role => user.Roles.Any(u => u.RoleId == role.Id))
                    .ToArray();
                if (array2.Length != 0)
                {
                    input.RoleNames = input
                        .RoleNames.Union(array2.Select(r => r.Name))
                        .Distinct()
                        .ToArray();
                }
            }
        }
    }

    protected virtual async Task<IdentityUserDto> FindByIdInternalAsync(Guid id)
    {
        IdentityUser identityUser = await UserManager.FindByIdAsync(id.ToString());
        IdentityUserDto userDto = ObjectMapper.Map<IdentityUser, IdentityUserDto>(identityUser);
        if (identityUser == null)
        {
            return userDto;
        }

        userDto.RoleNames = (await UserManager.GetRolesAsync(identityUser)).ToList();

        userDto.SupportTwoFactor = await IdentityProTwoFactorManager.IsOptionalAsync();

        return userDto;
    }

    protected virtual async Task<IRemoteStreamContent> GetImportUsersFileAsync<T>(
        List<T> users,
        string fileName,
        ImportUsersFromFileType fileType)
        where T : ImportUsersFromFileDto
    {
        IConfiguration iconfiguration = null;
        string contentType;
        ExcelType excelType;
        switch (fileType)
        {
            case ImportUsersFromFileType.Csv:
                fileName += ".csv";
                contentType = "text/csv";
                excelType = ExcelType.CSV;
                iconfiguration = new CsvConfiguration() { Seperator = ';' };
                break;
            default:
                fileName += ".xlsx";
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                excelType = ExcelType.XLSX;
                break;
        }

        using MemoryStream memoryStream = new MemoryStream();
        await MiniExcel.SaveAsAsync(memoryStream, users, true, "Sheet1", excelType, iconfiguration);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        return new RemoteStreamContent(memoryStream, fileName, contentType);
    }

    protected virtual async Task<List<IdentityUserExportDto>> GetExportUsersAsync(GetIdentityUserListAsFileInput input)
    {
        IDownloadCacheItem downloadCacheItem = await CheckDownloadTokenAsync(input.Token);

        using (CurrentTenant.Change(downloadCacheItem.TenantId))
        {
            List<IdentityUser> users = await UserRepository.GetListAsync(
                input.Sorting,
                filter: input.Filter,
                roleId: input.RoleId,
                organizationUnitId: input.OrganizationUnitId,
                userName: input.UserName,
                phoneNumber: input.PhoneNumber,
                emailAddress: input.EmailAddress,
                name: input.Name,
                surname: input.Surname,
                isLockedOut: input.IsLockedOut,
                notActive: input.NotActive,
                emailConfirmed: input.EmailConfirmed,
                isExternal: input.IsExternal,
                maxCreationTime: input.MaxCreationTime,
                minCreationTime: input.MinCreationTime,
                maxModifitionTime: input.MaxModifitionTime,
                minModifitionTime: input.MinModifitionTime);
            IEnumerable<Guid> userIds = users.Select(x => x.Id);

            List<IdentityUserIdWithRoleNames> source = await UserRepository.GetRoleNamesAsync(userIds);
            List<IdentityUserExportDto> identityUserExportDtoList = ObjectMapper.Map<
                List<IdentityUser>,
                List<IdentityUserExportDto>
            >(users);
            for (int i = 0; i < users.Count; ++i)
            {
                IdentityUserIdWithRoleNames userIdWithRoleNames = source.FirstOrDefault(x => x.Id == users[i].Id);
                if (userIdWithRoleNames != null)
                {
                    identityUserExportDtoList[i].Roles = userIdWithRoleNames.RoleNames.JoinAsString(";");
                }
            }

            return identityUserExportDtoList;
        }
    }

    protected virtual async Task<IDownloadCacheItem> CheckDownloadTokenAsync(
        string token,
        bool isInvalidUsersToken = false)
    {
        IDownloadCacheItem downloadCacheItem;
        if (!isInvalidUsersToken)
        {
            downloadCacheItem = await DownloadTokenCache.GetAsync(token);
        }
        else
        {
            downloadCacheItem = await ImportInvalidUsersCache.GetAsync(token);
        }

        if (downloadCacheItem == null || token != downloadCacheItem.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + token);
        }

        return downloadCacheItem;
    }

    private async Task<OrganizationUnitWithDetailsDto> ConvertToOrganizationUnitWithDetailsDtoAsync(
        OrganizationUnit organizationUnit,
        Dictionary<Guid, IdentityRole> roleLookup)
    {
        var dto = ObjectMapper.Map<OrganizationUnit, OrganizationUnitWithDetailsDto>(organizationUnit);
        dto.Roles = new List<IdentityRoleDto>();
        foreach (var ouRole in organizationUnit.Roles)
        {
            var role = roleLookup.GetOrDefault(ouRole.RoleId);
            if (role != null)
            {
                dto.Roles.Add(ObjectMapper.Map<IdentityRole, IdentityRoleDto>(role));
            }
        }

        return await Task.FromResult(dto);
    }

    private async Task<Dictionary<Guid, IdentityRole>> GetRoleLookup(IEnumerable<OrganizationUnit> organizationUnits)
    {
        var roleIds = organizationUnits
            .SelectMany(q => q.Roles)
            .Select(t => t.RoleId)
            .Distinct()
            .ToArray();

        return (await RoleRepository.GetListAsync(roleIds)).ToDictionary(u => u.Id, u => u);
    }

    private static IConfiguration GetExcelConfiguration(ExcelType excelType)
    {
        DynamicExcelColumn[] dynamicExcelColumnArray = new DynamicExcelColumn[13]
        {
            new(nameof(IdentityUser.UserName)) { Name = "User name", Width = 15.0 },
            new DynamicExcelColumn(nameof(IdentityUser.Email))
            {
                Name = "Email address",
                Width = 20.0
            },
            new DynamicExcelColumn(nameof(IdentityUser.Roles)) { Width = 10.0 },
            new DynamicExcelColumn(nameof(IdentityUser.PhoneNumber))
            {
                Name = "Phone number",
                Width = 15.0
            },
            new DynamicExcelColumn(nameof(IdentityUser.Name)) { Width = 10.0 },
            new DynamicExcelColumn(nameof(IdentityUser.Surname)) { Width = 10.0 },
            new DynamicExcelColumn(nameof(IdentityUser.IsActive)) { Name = "Active", Width = 10.0 },
            new DynamicExcelColumn(nameof(IdentityUser.LockoutEnabled))
            {
                Name = "Account lookout",
                Width = 15.0
            },
            new DynamicExcelColumn(nameof(IdentityUser.EmailConfirmed))
            {
                Name = "Email confirmed",
                Width = 15.0
            },
            new DynamicExcelColumn(nameof(IdentityUser.TwoFactorEnabled))
            {
                Name = "Two factor enabled",
                Width = 15.0
            },
            new DynamicExcelColumn(nameof(IdentityUser.AccessFailedCount))
            {
                Name = "Access failed count",
                Width = 15.0
            },
            new DynamicExcelColumn(nameof(IdentityUser.CreationTime))
            {
                Name = "Creation time",
                Width = 15.0
            },
            new DynamicExcelColumn(nameof(IdentityUser.LastModificationTime))
            {
                Name = "Last modification time",
                Width = 20.0
            }
        };

        return excelType == ExcelType.CSV
            ? new CsvConfiguration { DynamicColumns = dynamicExcelColumnArray, Seperator = ';' }
            : new OpenXmlConfiguration { DynamicColumns = dynamicExcelColumnArray };
    }
}
