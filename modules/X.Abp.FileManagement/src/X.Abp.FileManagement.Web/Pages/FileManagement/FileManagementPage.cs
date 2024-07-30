// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

using X.Abp.FileManagement.Localization;

namespace X.Abp.FileManagement.Web.Pages.FileManagement;

public abstract class FileManagementPageModel : AbpPageModel
{
    protected FileManagementPageModel()
    {
        LocalizationResourceType = typeof(FileManagementResource);
        ObjectMapperContext = typeof(AbpFileManagementWebModule);
    }
}
