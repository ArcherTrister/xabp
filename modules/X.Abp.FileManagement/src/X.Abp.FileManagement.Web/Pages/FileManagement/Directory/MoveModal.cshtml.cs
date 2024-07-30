// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.FileManagement.Directories;

namespace X.Abp.FileManagement.Web.Pages.FileManagement.Directory;

public class MoveModalModel : FileManagementPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public MoveDirectoryInput MoveDirectoryInput { get; set; }

    protected IDirectoryDescriptorAppService DirectoryDescriptorAppService { get; }

    public MoveModalModel(IDirectoryDescriptorAppService directoryDescriptorAppService)
    {
        DirectoryDescriptorAppService = directoryDescriptorAppService;
    }

    public virtual async Task OnGetAsync()
    {
        var directoryDescriptorDto = await DirectoryDescriptorAppService.GetAsync(Id);
        MoveDirectoryInput = new MoveDirectoryInput
        {
            Id = directoryDescriptorDto.Id,
            NewParentId = null,
            ConcurrencyStamp = directoryDescriptorDto.ConcurrencyStamp
        };
    }

    public virtual async Task OnPostAsync()
    {
        await DirectoryDescriptorAppService.MoveAsync(MoveDirectoryInput);
    }
}
