// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.FileManagement.Directories;

namespace X.Abp.FileManagement.Web.Pages.FileManagement.Directory;

public class ChangeNameModalModel : FileManagementPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public RenameDirectoryInput DirectoryDescriptor { get; set; }

    protected IDirectoryDescriptorAppService DirectoryDescriptorAppService { get; }

    public ChangeNameModalModel(IDirectoryDescriptorAppService directoryDescriptorAppService)
    {
        DirectoryDescriptorAppService = directoryDescriptorAppService;
    }

    public virtual async Task OnGetAsync()
    {
        var directoryDescriptorDto = await DirectoryDescriptorAppService.GetAsync(Id);

        DirectoryDescriptor = ObjectMapper.Map<DirectoryDescriptorDto, RenameDirectoryInput>(directoryDescriptorDto);
    }

    public virtual async Task OnPostAsync()
    {
        await DirectoryDescriptorAppService.RenameAsync(Id, DirectoryDescriptor);
    }
}
