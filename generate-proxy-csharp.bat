@ECHO off
cls

ECHO Starting generate...
ECHO.

abp generate-proxy -t csharp -url https://localhost:44302/ -m accountAdmin -wd ./modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Admin.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m account -wd ./modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m auditLogging -wd ./modules/X.Abp.AuditLogging/src/X.Abp.AuditLogging.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m chat -wd ./modules/X.Abp.Chat/src/X.Abp.Chat.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m cms-kit -wd ./modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m cms-kit-pro -wd ./modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Public.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m cms-kit-pro-admin -wd ./modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Admin.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m fileManagement -wd ./modules/X.Abp.FileManagement/src/X.Abp.FileManagement.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m form -wd ./modules/X.Abp.Forms/src/X.Abp.Forms.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m gdpr -wd ./modules/X.Abp.Gdpr/src/X.Abp.Gdpr.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m identity -wd ./modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m identityServer -wd ./modules/X.Abp.IdentityServer.Pro/src/X.Abp.IdentityServer.Pro.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m languageManagement -wd ./modules/X.Abp.LanguageManagement/src/X.Abp.LanguageManagement.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m leptonTheme -wd ./modules/X.Abp.LeptonTheme/src/X.Abp.LeptonTheme.Management.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m notification -wd ./modules/X.Abp.Notification/src/X.Abp.Notification.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m openiddictpro -wd ./modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m payment -wd ./modules/X.Abp.Payment/src/X.Abp.Payment.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m paymentAdmin -wd ./modules/X.Abp.Payment/src/X.Abp.Payment.Admin.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m quartz -wd ./modules/X.Abp.Quartz/src/X.Abp.Quartz.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m saas -wd ./modules/X.Abp.Saas/src/X.Abp.Saas.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m textTemplateManagement -wd ./modules/X.Abp.TextTemplateManagement/src/X.Abp.TextTemplateManagement.HttpApi.Client --without-contracts --folder ClientProxies

abp generate-proxy -t csharp -url https://localhost:44302/ -m versionManagement -wd ./modules/X.Abp.VersionManagement/src/X.Abp.VersionManagement.HttpApi.Client --without-contracts --folder ClientProxies


ECHO.
ECHO.csharp-proxy have been successfully generate. Press any key to exit.
pause > nul
