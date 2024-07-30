// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Features;

using X.Abp.FileManagement.Permissions;

namespace X.Abp.FileManagement.Files;

[RequiresFeature(FileManagementFeatures.Enable)]
[Authorize(AbpFileManagementPermissions.FileDescriptor.Default)]
public class FileDescriptorAppService : FileManagementAppServiceBase, IFileDescriptorAppService
{
    protected IFileManager FileManager { get; }

    protected IFileDescriptorRepository FileDescriptorRepository { get; }

    protected IBlobContainer<FileManagementContainer> BlobContainer { get; }

    protected IDistributedCache<FileDownloadTokenCacheItem, string> DownloadTokenCache { get; }

    public FileDescriptorAppService(
        IFileManager fileManager,
        IFileDescriptorRepository fileDescriptorRepository,
        IBlobContainer<FileManagementContainer> blobContainer,
        IDistributedCache<FileDownloadTokenCacheItem, string> downloadTokenCache)
    {
        FileManager = fileManager;
        FileDescriptorRepository = fileDescriptorRepository;
        BlobContainer = blobContainer;
        DownloadTokenCache = downloadTokenCache;
    }

    [AllowAnonymous]
    public virtual async Task<FileDescriptorDto> GetAsync(Guid id)
    {
        var fileDescriptor = await FileDescriptorRepository.GetAsync(id);

        return ObjectMapper.Map<FileDescriptor, FileDescriptorDto>(fileDescriptor);
    }

    public virtual async Task<ListResultDto<FileDescriptorDto>> GetListAsync(Guid? directoryId)
    {
        var fileDescriptors = ObjectMapper.Map<List<FileDescriptor>, List<FileDescriptorDto>>(
            await FileDescriptorRepository.GetListAsync(directoryId));

        return new ListResultDto<FileDescriptorDto>(fileDescriptors);
    }

    [Authorize(AbpFileManagementPermissions.FileDescriptor.Update)]
    public virtual async Task<FileDescriptorDto> RenameAsync(Guid id, RenameFileInput input)
    {
        var file = await FileDescriptorRepository.GetAsync(id);

        file.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        await FileManager.RenameAsync(file, input.Name);
        await FileDescriptorRepository.UpdateAsync(file);

        return ObjectMapper.Map<FileDescriptor, FileDescriptorDto>(file);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> DownloadAsync(Guid id, string token)
    {
        var downloadToken = await DownloadTokenCache.GetAsync(token);
        if (downloadToken == null || downloadToken.FileDescriptorId != id)
        {
            throw new AbpAuthorizationException("Invalid download token: " + token);
        }

        await DownloadTokenCache.RemoveAsync(token);
        var stream = await BlobContainer.GetAsync(id.ToString());

        return new RemoteStreamContent(stream);
    }

    [Authorize(AbpFileManagementPermissions.FileDescriptor.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var file = await FileDescriptorRepository.GetAsync(id);

        await FileManager.DeleteAsync(file);
    }

    [Authorize(AbpFileManagementPermissions.FileDescriptor.Create)]
    public virtual async Task<FileDescriptorDto> CreateAsync(Guid? directoryId, CreateFileInputWithStream inputWithStream)
    {
        await CheckSizeAsync(inputWithStream.File.ContentLength ?? 0);

        var fileDescriptor = await FileManager.CreateAsync(inputWithStream.Name, inputWithStream.File.ContentType, inputWithStream.File, directoryId, CurrentTenant.Id, true);

        return ObjectMapper.Map<FileDescriptor, FileDescriptorDto>(fileDescriptor);
    }

    [Authorize(AbpFileManagementPermissions.FileDescriptor.Update)]
    public virtual async Task<FileDescriptorDto> MoveAsync(MoveFileInput input)
    {
        var fileDescriptor = await FileDescriptorRepository.GetAsync(input.Id);

        fileDescriptor.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        await FileManager.MoveAsync(fileDescriptor, input.NewDirectoryId);
        await FileDescriptorRepository.UpdateAsync(fileDescriptor);

        return ObjectMapper.Map<FileDescriptor, FileDescriptorDto>(fileDescriptor);
    }

    public virtual async Task<List<FileUploadPreInfoDto>> GetPreInfoAsync(List<FileUploadPreInfoRequest> input)
    {
        var totalSize = input.Sum(x => x.Size);

        await CheckSizeAsync(totalSize);

        var result = new List<FileUploadPreInfoDto>();

        foreach (var infoRequest in input)
        {
            result.Add(new FileUploadPreInfoDto
            {
                FileName = infoRequest.FileName,
                HasValidName = FileNameValidator.IsValidName(infoRequest.FileName),
                DoesExist = await FileDescriptorRepository.FindAsync(infoRequest.FileName, infoRequest.DirectoryId) != null
            });
        }

        return result;
    }

    public virtual async Task<byte[]> GetContentAsync(Guid id)
    {
        return await BlobContainer.GetAllBytesAsync(id.ToString());
    }

    public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync(Guid id)
    {
        var token = Guid.NewGuid().ToString();

        await DownloadTokenCache.SetAsync(
            token,
            new FileDownloadTokenCacheItem { FileDescriptorId = id },
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            });

        return new DownloadTokenResultDto
        {
            Token = token
        };
    }

    protected virtual async Task CheckSizeAsync(long contentLength)
    {
        if (contentLength > FileDescriptorConsts.MaxSizeLength)
        {
            throw new UserFriendlyException(L["FileTooBig", BeautifySize(FileDescriptorConsts.MaxSizeLength)]);
        }

        var maxSizeDescription = await FeatureChecker.GetOrNullAsync(FileManagementFeatures.StorageSize);

        if (maxSizeDescription is null or "0")
        {
            return;
        }

        var maxSize = long.Parse(maxSizeDescription);

        var totalStorage = await FileDescriptorRepository.GetTotalSizeAsync();

        if (totalStorage + contentLength < maxSize)
        {
            return;
        }

        var remainedSize = maxSize - totalStorage;
        throw new NotEnoughStorageSizeException(BeautifySize(maxSize), BeautifySize(remainedSize));
    }

    protected virtual string BeautifySize(long size)
    {
        if (size is 0 or 1)
        {
            return $"{size} Byte";
        }

        if (size >= AbpFileManagementConsts.Terabyte)
        {
            var fixedSize = size / (float)AbpFileManagementConsts.Terabyte;
            return $"{FormatSize(fixedSize)} TB";
        }

        if (size >= AbpFileManagementConsts.Gigabyte)
        {
            var fixedSize = size / AbpFileManagementConsts.Gigabyte;
            return $"{FormatSize(fixedSize)} GB";
        }

        if (size >= AbpFileManagementConsts.Megabyte)
        {
            var fixedSize = size / AbpFileManagementConsts.Megabyte;
            return $"{FormatSize(fixedSize)} MB";
        }

        if (size >= AbpFileManagementConsts.Kilobyte)
        {
            var fixedSize = size / AbpFileManagementConsts.Kilobyte;
            return $"{FormatSize(fixedSize)} KB";
        }

        return $"{size} B";
    }

    protected virtual string FormatSize(float size)
    {
        var s = $"{size:0.00}";

        return s.EndsWith("00", StringComparison.InvariantCultureIgnoreCase) ? size.ToString() : s;
    }
}
