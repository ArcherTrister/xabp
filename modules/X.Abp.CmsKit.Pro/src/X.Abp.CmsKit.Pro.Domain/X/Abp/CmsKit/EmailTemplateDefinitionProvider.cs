// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Reflection;

using Volo.Abp.TextTemplating;

using X.Abp.CmsKit.Templates;

namespace X.Abp.CmsKit;

public class EmailTemplateDefinitionProvider : TemplateDefinitionProvider
{
    public override void Define(ITemplateDefinitionContext context)
    {
        context.Add(new TemplateDefinition(CmsKitEmailTemplates.ContactEmailTemplate, layout: "Abp.StandardEmailTemplates.Layout")
            .WithVirtualFilePath("/X/CmsKit/Templates/ContactEmail.tpl", true));

        context.Add(new TemplateDefinition(CmsKitEmailTemplates.PageFeedbackEmailTemplate, layout: "Abp.StandardEmailTemplates.Layout")
            .WithVirtualFilePath("/X/CmsKit/Templates/PageFeedbackEmail.tpl", true));
    }
}
