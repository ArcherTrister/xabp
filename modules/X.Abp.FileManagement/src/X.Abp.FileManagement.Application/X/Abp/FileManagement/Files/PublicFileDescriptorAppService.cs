// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Features;
using Volo.Abp.Validation;

using X.Abp.FileManagement.Directories;

namespace X.Abp.FileManagement.Files;

[RequiresFeature(FileManagementFeatures.Enable)]
[Authorize]
public class PublicFileDescriptorAppService : FileManagementAppServiceBase, IPublicFileDescriptorAppService
{
    protected IDirectoryDescriptorRepository DirectoryDescriptorRepository => LazyServiceProvider.LazyGetRequiredService<IDirectoryDescriptorRepository>();

    protected IFileManager FileManager => LazyServiceProvider.LazyGetRequiredService<IFileManager>();

    protected IFileDescriptorRepository FileDescriptorRepository => LazyServiceProvider.LazyGetRequiredService<IFileDescriptorRepository>();

    protected IBlobContainer<FileManagementContainer> BlobContainer => LazyServiceProvider.LazyGetRequiredService<IBlobContainer<FileManagementContainer>>();

    protected IDistributedCache<FileDownloadTokenCacheItem, string> DownloadTokenCache => LazyServiceProvider.LazyGetRequiredService<IDistributedCache<FileDownloadTokenCacheItem, string>>();

    /*
    public PublicFileDescriptorAppService(
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
    */
    /*
    // [AllowAnonymous]
    // public virtual async Task<IRemoteStreamContent> PreViewAsync(Guid id, string token)
    // {
    //    var downloadToken = await DownloadTokenCache.GetAsync(token);
    //    if (downloadToken == null || downloadToken.FileDescriptorId != id)
    //    {
    //        throw new AbpAuthorizationException("Invalid download token: " + token);
    //    }
    //    var entity = await FileDescriptorRepository.GetAsync(id);
    //    var stream = await BlobContainer.GetAsync(id.ToString());
    //    //RemoteStreamContentOutputFormatter
    //    //Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.ContentType] = entity.MimeType;
    //    //HttpContextAccessor.HttpContext.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.ContentType] = entity.MimeType;
    //    HttpContextAccessor.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment;filename=\"{entity.Name}\"");
    //    HttpContextAccessor.HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");
    //    HttpContextAccessor.HttpContext.Response.ContentType = entity.MimeType;
    //    return new RemoteStreamContent(stream, fileName: entity.Name, contentType: entity.MimeType);
    // }
    */

    public virtual async Task<RawFileDto> GetRawFileAsync(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));

        return new RawFileDto
        {
            Bytes = await BlobContainer.GetAllBytesAsync(name)
        };
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetFileAsync(string name)
    {
        var fileStream = await BlobContainer.GetAsync(name);
        return new RemoteStreamContent(fileStream, name, GetByExtension(Path.GetExtension(name)), disposeStream: true);
    }

    [Authorize]
    public virtual async Task<UploadOutput> UploadAsync(UploadInput input)
    {
        if (input.File == null)
        {
            ThrowValidationException("Bytes of file can not be null or empty!", nameof(input.File));
        }

        await CheckSizeAsync(input.File.ContentLength ?? 0);

        var position = input.File.GetStream().Position;

        /*
        if (!ImageFormatHelper.IsValidImage(input.File.GetStream(), FileDescriptorConsts.AllowedImageUploadFormats))
        {
            throw new UserFriendlyException("Invalid image format!");
        }
        */

        // IsValidImage may change the position of the stream
        if (input.File.GetStream().CanSeek)
        {
            input.File.GetStream().Position = position;
        }

        Guid? directoryId = null;
        if (!string.IsNullOrWhiteSpace(input.DirectoryName))
        {
            var directoryDescriptor = await DirectoryDescriptorRepository.FindAsync(input.DirectoryName, null);
            if (directoryDescriptor == null)
            {
                directoryDescriptor = new DirectoryDescriptor(GuidGenerator.Create(), input.DirectoryName, null, CurrentTenant.Id);

                await DirectoryDescriptorRepository.InsertAsync(directoryDescriptor);

                directoryId = directoryDescriptor.Id;
            }
        }

        var fileDescriptor = await FileDescriptorRepository.FindAsync(input.File.FileName, directoryId);

        if (fileDescriptor != null)
        {
            throw new FileAlreadyExistException(fileDescriptor.Name);
        }
        else
        {
            fileDescriptor = new FileDescriptor(GuidGenerator.Create(), input.File.FileName, input.File.ContentType, directoryId, (int)(input.File.ContentLength ?? 0), CurrentTenant.Id);
            await FileDescriptorRepository.InsertAsync(fileDescriptor);
        }

        await BlobContainer.SaveAsync(fileDescriptor.UniqueFileName, input.File.GetStream());

        return new UploadOutput
        {
            FileId = fileDescriptor.Id,
            UniqueFileName = fileDescriptor.UniqueFileName,
            WebUrl = "/api/file-management/public-file-descriptor/www/" + fileDescriptor.UniqueFileName
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

    private static string GetByExtension(string extension)
    {
        extension = extension.RemovePreFix(".").ToLowerInvariant();

        return extension switch
        {
            "png" => "image/png",
            "gif" => "image/gif",
            "jpg" or "jpeg" => "image/jpeg",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

            // TODO: Add other file extensions too..
            _ => "application/octet-stream",
        };
    }

    private static void ThrowValidationException(string message, string memberName)
    {
        throw new AbpValidationException(message,
            new List<ValidationResult>
            {
                new ValidationResult(message, new[] { memberName })
            });
    }
}
