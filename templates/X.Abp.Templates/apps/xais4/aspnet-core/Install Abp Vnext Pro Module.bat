@echo off
rem X.CmsKit.Pro inseart in Domain.Shared
rem GlobalFeatureManager.Instance.Modules.CmsKit(cmsKit =>
rem {
rem     cmsKit.EnableAll();
rem });
rem
rem GlobalFeatureManager.Instance.Modules.CmsKitPro(cmsKitPro =>
rem {
rem     cmsKitPro.EnableAll();
rem });


xabp install-module X.Chat -v 7.2.3-preview
xabp install-module X.CmsKit.Pro -v 7.2.3-preview
xabp install-module X.FileManagement -v 7.2.3-preview
xabp install-module X.Forms -v 7.2.3-preview
xabp install-module X.Payment -v 7.2.3-preview
xabp install-module X.TextTemplateManagement -v 7.2.3-preview
