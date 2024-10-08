﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Services;

using X.Abp.FileManagement.Directories;

namespace X.Abp.FileManagement.Files;

public class FileManager : DomainService, IFileManager
{
    protected IFileDescriptorRepository FileDescriptorRepository { get; }

    protected IDirectoryDescriptorRepository DirectoryDescriptorRepository { get; }

    protected IBlobContainer<FileManagementContainer> BlobContainer { get; }

    public FileManager(
        IFileDescriptorRepository fileDescriptorRepository,
        IBlobContainer<FileManagementContainer> blobContainer,
        IDirectoryDescriptorRepository directoryDescriptorRepository)
    {
        FileDescriptorRepository = fileDescriptorRepository;
        BlobContainer = blobContainer;
        DirectoryDescriptorRepository = directoryDescriptorRepository;
    }

    public virtual async Task<FileDescriptor> CreateAsync(string name, string mimeType, IRemoteStreamContent content, Guid? directoryId = null, Guid? tenantId = null, bool overrideExisting = false)
    {
        var fileDescriptor = await SaveFileDescriptorAsync(name, mimeType, (int)(content.ContentLength ?? 0), directoryId, tenantId, overrideExisting);

        await BlobContainer.SaveAsync(fileDescriptor.Id.ToString(), content.GetStream(), true);

        return fileDescriptor;
    }

    protected virtual async Task<FileDescriptor> SaveFileDescriptorAsync(string name, string mimeType, int contentLength, Guid? directoryId, Guid? tenantId, bool overrideExisting)
    {
        var fileDescriptor = await FileDescriptorRepository.FindAsync(name, directoryId);

        if (fileDescriptor != null)
        {
            if (!overrideExisting)
            {
                throw new FileAlreadyExistException(name);
            }

            fileDescriptor.SetSize(contentLength);
            await FileDescriptorRepository.UpdateAsync(fileDescriptor);
        }
        else
        {
            fileDescriptor =
                new FileDescriptor(GuidGenerator.Create(), name, mimeType, directoryId, contentLength, tenantId);
            await FileDescriptorRepository.InsertAsync(fileDescriptor);
        }

        return fileDescriptor;
    }

    public virtual async Task RenameAsync(FileDescriptor file, string newName)
    {
        var existingFile = await FileDescriptorRepository.FindAsync(newName, file.DirectoryId);
        if (existingFile != null)
        {
            throw new FileAlreadyExistException(newName);
        }

        file.SetName(newName);
    }

    public virtual async Task DeleteAllAsync(Guid? directoryId)
    {
        foreach (var file in await FileDescriptorRepository.GetListAsync(directoryId))
        {
            await DeleteAsync(file);
        }
    }

    public virtual async Task DeleteAsync(FileDescriptor file)
    {
        await BlobContainer.DeleteAsync(file.Id.ToString());
        await FileDescriptorRepository.DeleteAsync(file, cancellationToken: CancellationToken.None);
    }

    public virtual async Task MoveAsync(FileDescriptor file, Guid? newDirectoryId)
    {
        if (newDirectoryId.HasValue)
        {
            if (await DirectoryDescriptorRepository.FindAsync(newDirectoryId.Value) == null)
            {
                throw new DirectoryNotExistException();
            }
        }

        file.SetDirectoryId(newDirectoryId);
    }
}
