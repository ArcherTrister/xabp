// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Validation;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class GetTemplateContentInput
{
    [DynamicStringLength(typeof(TextTemplateConsts), "MaxNameLength", null)]
    [Required]
    public string TemplateName { get; set; }

    [DynamicStringLength(typeof(TextTemplateConsts), "MaxCultureNameLength", null)]
    public string CultureName { get; set; }
}
