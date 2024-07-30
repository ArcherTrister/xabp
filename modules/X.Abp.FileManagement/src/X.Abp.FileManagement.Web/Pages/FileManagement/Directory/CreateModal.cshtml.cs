// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.FileManagement.Directories;

namespace X.Abp.FileManagement.Web.Pages.FileManagement.Directory;

public class CreateModalModel : FileManagementPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? ParentId { get; set; }

    [BindProperty]
    public CreateDirectoryInput CreateDirectoryInput { get; set; }

    protected IDirectoryDescriptorAppService DirectoryDescriptorAppService { get; }

    public CreateModalModel(IDirectoryDescriptorAppService directoryDescriptorAppService)
    {
        DirectoryDescriptorAppService = directoryDescriptorAppService;
    }

    public virtual async Task OnGetAsync()
    {
        await Task.CompletedTask;
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        CreateDirectoryInput.ParentId = ParentId;

        await DirectoryDescriptorAppService.CreateAsync(CreateDirectoryInput);

        return NoContent();
    }
}
