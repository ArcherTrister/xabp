// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using X.Abp.FileManagement.Directories;
using X.Abp.FileManagement.Files;

namespace X.Abp.FileManagement;

public class AbpFileManagementDomainMappingProfile : Profile
{
    public AbpFileManagementDomainMappingProfile()
    {
        CreateMap<FileDescriptor, FileDescriptorEto>();
        CreateMap<DirectoryDescriptor, DirectoryDescriptorEto>();
    }
}
