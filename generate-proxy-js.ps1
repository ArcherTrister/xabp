# 清屏
Clear-Host

# 输出提示信息
Write-Host "Starting generate..."

# 执行生成代理的命令
& abp generate-proxy -t js -url https://localhost:44302/ -m accountAdmin -wd ./modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Admin.HttpApi.Client -o ../X.Abp.Account.Pro.Admin.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m account -wd ./modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.HttpApi.Client -o ../X.Abp.Account.Pro.Public.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m account -wd ./modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.HttpApi.Client -o ../X.Abp.Account.Pro.Public.Web.Shared/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m auditLogging -wd ./modules/X.Abp.AuditLogging/src/X.Abp.AuditLogging.HttpApi.Client -o ../X.Abp.AuditLogging.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m chat -wd ./modules/X.Abp.Chat/src/X.Abp.Chat.HttpApi.Client -o ../X.Abp.Chat.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m cms-kit -wd ./modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.HttpApi.Client -o ../X.Abp.CmsKit.Pro.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m cms-kit-pro-common -wd ./modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Public.HttpApi.Client -o ../X.Abp.CmsKit.Pro.Public.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m cms-kit-pro-admin -wd ./modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Admin.HttpApi.Client -o ../X.Abp.CmsKit.Pro.Admin.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m fileManagement -wd ./modules/X.Abp.FileManagement/src/X.Abp.FileManagement.HttpApi.Client -o ../X.Abp.FileManagement.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m form -wd ./modules/X.Abp.Forms/src/X.Abp.Forms.HttpApi.Client -o ../X.Abp.Forms.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m gdpr -wd ./modules/X.Abp.Gdpr/src/X.Abp.Gdpr.HttpApi.Client -o ../X.Abp.Gdpr.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m identity -wd ./modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.HttpApi.Client -o ../X.Abp.Identity.Pro.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m identityServer -wd ./modules/X.Abp.IdentityServer.Pro/src/X.Abp.IdentityServer.Pro.HttpApi.Client -o ../X.Abp.IdentityServer.Pro.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m languageManagement -wd ./modules/X.Abp.LanguageManagement/src/X.Abp.LanguageManagement.HttpApi.Client -o ../X.Abp.LanguageManagement.Web/wwwroot/client-proxies/

# & abp generate-proxy -t js -url https://localhost:44302/ -m leptonThemeManagement -wd ./modules/X.Abp.LeptonTheme/src/X.Abp.LeptonTheme.Management.HttpApi.Client -o ../X.Abp.LeptonTheme.Management.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m notification -wd ./modules/X.Abp.Notification/src/X.Abp.Notification.HttpApi.Client -o ../X.Abp.Notification.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m openiddictpro -wd ./modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.HttpApi.Client -o ../X.Abp.OpenIddict.Pro.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m payment -wd ./modules/X.Abp.Payment/src/X.Abp.Payment.HttpApi.Client -o ../X.Abp.Payment.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m paymentAdmin -wd ./modules/X.Abp.Payment/src/X.Abp.Payment.Admin.HttpApi.Client -o ../X.Abp.Payment.Admin.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m quartz -wd ./modules/X.Abp.Quartz/src/X.Abp.Quartz.HttpApi.Client -o ../X.Abp.Quartz.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m saas -wd ./modules/X.Abp.Saas/src/X.Abp.Saas.HttpApi.Client -o ../X.Abp.Saas.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m textTemplateManagement -wd ./modules/X.Abp.TextTemplateManagement/src/X.Abp.TextTemplateManagement.HttpApi.Client -o ../X.Abp.TextTemplateManagement.Web/wwwroot/client-proxies/

& abp generate-proxy -t js -url https://localhost:44302/ -m versionManagement -wd ./modules/X.Abp.VersionManagement/src/X.Abp.VersionManagement.HttpApi.Client -o ../X.Abp.VersionManagement.Web/wwwroot/client-proxies/

# 输出完成信息
Write-Host ""
Write-Host "JS proxies have been successfully generated."

# 等待用户输入后退出
Read-Host "Press any key to exit."
