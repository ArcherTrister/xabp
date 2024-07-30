// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application.Dtos;

namespace X.Abp.LanguageManagement.Dto;

public class GetLanguagesTextsInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }

    public string ResourceName { get; set; }

    public string BaseCultureName { get; set; }

    public string TargetCultureName { get; set; }

    public bool GetOnlyEmptyValues { get; set; }
}
