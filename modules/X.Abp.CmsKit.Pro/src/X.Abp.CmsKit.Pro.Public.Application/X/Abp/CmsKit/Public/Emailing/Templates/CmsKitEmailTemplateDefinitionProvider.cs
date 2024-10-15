// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.TextTemplating;
using Volo.CmsKit.Localization;

namespace X.Abp.CmsKit.Public.Emailing.Templates;

public class CmsKitEmailTemplateDefinitionProvider : TemplateDefinitionProvider
{
    public override void Define(ITemplateDefinitionContext context)
    {
        context.Add(new TemplateDefinition[]
        {
            new TemplateDefinition(CmsKitEmailTemplates.NewsletterEmailTemplate, typeof(CmsKitResource), null, false, "Abp.StandardEmailTemplates.Layout")
            .WithVirtualFilePath("/X/Abp/CmsKit/Public/Emailing/Templates/NewsletterEmailLayout.tpl", true)
        });
    }
}
