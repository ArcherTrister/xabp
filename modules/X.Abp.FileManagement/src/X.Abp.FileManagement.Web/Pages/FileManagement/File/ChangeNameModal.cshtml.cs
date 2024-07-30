// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.FileManagement.Files;

namespace X.Abp.FileManagement.Web.Pages.FileManagement.File;

public class ChangeNameModalModel : FileManagementPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public RenameFileInput RenameFileInput { get; set; }

    protected IFileDescriptorAppService FileDescriptorAppService { get; }

    public ChangeNameModalModel(IFileDescriptorAppService fileDescriptorAppService)
    {
        FileDescriptorAppService = fileDescriptorAppService;
    }

    public virtual async Task OnGetAsync()
    {
        var fileDescriptorDto = await FileDescriptorAppService.GetAsync(Id);

        RenameFileInput = new RenameFileInput
        {
            Name = fileDescriptorDto.Name,
            ConcurrencyStamp = fileDescriptorDto.ConcurrencyStamp
        };
    }

    public virtual async Task OnPostAsync()
    {
        await FileDescriptorAppService.RenameAsync(Id, RenameFileInput);
    }
}
