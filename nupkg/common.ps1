# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$rootFolder = Join-Path $packFolder "../"

function Write-Info {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $text
    )

    Write-Host $text -ForegroundColor Black -BackgroundColor Green

    try {
        $host.UI.RawUI.WindowTitle = $text
    }
    catch {
        #Changing window title is not suppoerted!
    }
}

function Write-Error {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $text
    )

    Write-Host $text -ForegroundColor Red -BackgroundColor Black
}

function Seperator {
    Write-Host ("_" * 100)  -ForegroundColor gray
}

function Get-Current-Version {
    $commonPropsFilePath = resolve-path "../common.props"
    $commonPropsXmlCurrent = [xml](Get-Content $commonPropsFilePath )
    $currentVersion = $commonPropsXmlCurrent.Project.PropertyGroup.Version.Trim()
    return $currentVersion
}

function Get-Current-Branch {
    return git branch --show-current
}

function Read-File {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $filePath
    )

    $pathExists = Test-Path -Path $filePath -PathType Leaf
    if ($pathExists) {
        return Get-Content $filePath
    }
    else {
        Write-Error  "$filePath path does not exist!"
    }
}

# List of solutions
$solutions = (
    # Lepton Theme
    # Lepton X Pro
    "modules/X.Abp.Account.Pro",
    "modules/X.Abp.AuditLogging",
    "modules/X.Abp.Chat",
    "modules/X.Abp.Cli",
    "modules/X.Abp.CmsKit.Pro",
    "modules/X.Abp.FileManagement",
    "modules/X.Abp.Forms",
    "modules/X.Abp.Gdpr",
    "modules/X.Abp.Identity.Pro",
    "modules/X.Abp.IdentityServer.Pro",
    "modules/X.Abp.LanguageManagement",
    "modules/X.Abp.LeptonTheme",
    "modules/X.Abp.Localization",
    "modules/X.Abp.Notification",
    "modules/X.Abp.OpenIddict.Pro",
    "modules/X.Abp.Payment",
    "modules/X.Abp.Quartz",
    "modules/X.Abp.Saas",
    "modules/X.Abp.TextTemplateManagement",
    "modules/X.Abp.VersionManagement"
)

# List of projects
$projects = (
    ## modules/X.Abp.Account.Pro
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Admin.Application",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Admin.Application.Contracts",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Admin.HttpApi",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Admin.HttpApi.Client",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Admin.Web",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.Application",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.Application.Contracts",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.HttpApi",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.HttpApi.Client",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.Web",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.Web.IdentityServer",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.Web.Impersonation",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.Web.OpenIddict",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Public.Web.Shared",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Shared.Application",
    "modules/X.Abp.Account.Pro/src/X.Abp.Account.Pro.Shared.Application.Contracts",

    ## modules/X.Abp.AuditLogging
    "modules/X.Abp.AuditLogging/src/X.Abp.AuditLogging.Application",
    "modules/X.Abp.AuditLogging/src/X.Abp.AuditLogging.Application.Contracts",
    "modules/X.Abp.AuditLogging/src/X.Abp.AuditLogging.HttpApi",
    "modules/X.Abp.AuditLogging/src/X.Abp.AuditLogging.HttpApi.Client",
    "modules/X.Abp.AuditLogging/src/X.Abp.AuditLogging.Web",

    ## modules/X.Abp.BasicData
    # "modules/X.Abp.BasicData/src/X.Abp.BasicData.Application",
    # "modules/X.Abp.BasicData/src/X.Abp.BasicData.Application.Contracts",
    # "modules/X.Abp.BasicData/src/X.Abp.BasicData.Domain",
    # "modules/X.Abp.BasicData/src/X.Abp.BasicData.Domain.Shared",
    # "modules/X.Abp.BasicData/src/X.Abp.BasicData.EntityFrameworkCore",
    # "modules/X.Abp.BasicData/src/X.Abp.BasicData.HttpApi",
    # "modules/X.Abp.BasicData/src/X.Abp.BasicData.HttpApi.Client",
    # "modules/X.Abp.BasicData/src/X.Abp.BasicData.Web",

    ## modules/X.Abp.Chat
    "modules/X.Abp.Chat/src/X.Abp.Chat.Application",
    "modules/X.Abp.Chat/src/X.Abp.Chat.Application.Contracts",
    "modules/X.Abp.Chat/src/X.Abp.Chat.Domain",
    "modules/X.Abp.Chat/src/X.Abp.Chat.Domain.Shared",
    "modules/X.Abp.Chat/src/X.Abp.Chat.EntityFrameworkCore",
    "modules/X.Abp.Chat/src/X.Abp.Chat.HttpApi",
    "modules/X.Abp.Chat/src/X.Abp.Chat.HttpApi.Client",
    "modules/X.Abp.Chat/src/X.Abp.Chat.SignalR",
    "modules/X.Abp.Chat/src/X.Abp.Chat.Web",

    ## modules/X.Abp.Cli
    "modules/X.Abp.Cli/src/X.Abp.Cli",

    ## modules/X.Abp.CmsKit.Pro
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Admin.Application",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Admin.Application.Contracts",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Admin.HttpApi",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Admin.HttpApi.Client",
    # "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Admin.Web",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Application",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Application.Contracts",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Domain",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Domain.Shared",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.EntityFrameworkCore",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.HttpApi",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.HttpApi.Client",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Public.Application",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Public.Application.Contracts",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Public.HttpApi",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Public.HttpApi.Client",
    "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Public.Web",
    # "modules/X.Abp.CmsKit.Pro/src/X.Abp.CmsKit.Pro.Web",

    ## modules/X.Abp.FileManagement
    "modules/X.Abp.FileManagement/src/X.Abp.FileManagement.Application",
    "modules/X.Abp.FileManagement/src/X.Abp.FileManagement.Application.Contracts",
    "modules/X.Abp.FileManagement/src/X.Abp.FileManagement.Domain",
    "modules/X.Abp.FileManagement/src/X.Abp.FileManagement.Domain.Shared",
    "modules/X.Abp.FileManagement/src/X.Abp.FileManagement.EntityFrameworkCore",
    "modules/X.Abp.FileManagement/src/X.Abp.FileManagement.HttpApi",
    "modules/X.Abp.FileManagement/src/X.Abp.FileManagement.HttpApi.Client",
    "modules/X.Abp.FileManagement/src/X.Abp.FileManagement.Web",

    ## modules/X.Abp.Forms
    "modules/X.Abp.Forms/src/X.Abp.Forms.Application",
    "modules/X.Abp.Forms/src/X.Abp.Forms.Application.Contracts",
    "modules/X.Abp.Forms/src/X.Abp.Forms.Domain",
    "modules/X.Abp.Forms/src/X.Abp.Forms.Domain.Shared",
    "modules/X.Abp.Forms/src/X.Abp.Forms.EntityFrameworkCore",
    "modules/X.Abp.Forms/src/X.Abp.Forms.HttpApi",
    "modules/X.Abp.Forms/src/X.Abp.Forms.HttpApi.Client",
    "modules/X.Abp.Forms/src/X.Abp.Forms.Web",

    ## "modules/X.Abp.Gdpr",
    "modules/X.Abp.Gdpr/src/X.Abp.Gdpr.Application",
    "modules/X.Abp.Gdpr/src/X.Abp.Gdpr.Application.Contracts",
    "modules/X.Abp.Gdpr/src/X.Abp.Gdpr.Domain",
    "modules/X.Abp.Gdpr/src/X.Abp.Gdpr.Domain.Shared",
    "modules/X.Abp.Gdpr/src/X.Abp.Gdpr.EntityFrameworkCore",
    "modules/X.Abp.Gdpr/src/X.Abp.Gdpr.HttpApi",
    "modules/X.Abp.Gdpr/src/X.Abp.Gdpr.HttpApi.Client",
    "modules/X.Abp.Gdpr/src/X.Abp.Gdpr.Web",

    ## modules/X.Abp.Identity.Pro
    "modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.Application",
    "modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.Application.Contracts",
    "modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.AspNetCore",
    "modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.Domain",
    "modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.Domain.Shared",
    "modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.EntityFrameworkCore",
    "modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.HttpApi",
    "modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.HttpApi.Client",
    "modules/X.Abp.Identity.Pro/src/X.Abp.Identity.Pro.Web",

    ## modules/X.Abp.IdentityServer.Pro
    "modules/X.Abp.IdentityServer.Pro/src/X.Abp.IdentityServer.Pro.Application",
    "modules/X.Abp.IdentityServer.Pro/src/X.Abp.IdentityServer.Pro.Application.Contracts",
    "modules/X.Abp.IdentityServer.Pro/src/X.Abp.IdentityServer.Pro.Domain",
    "modules/X.Abp.IdentityServer.Pro/src/X.Abp.IdentityServer.Pro.Domain.Shared",
    "modules/X.Abp.IdentityServer.Pro/src/X.Abp.IdentityServer.Pro.EntityFrameworkCore",
    "modules/X.Abp.IdentityServer.Pro/src/X.Abp.IdentityServer.Pro.HttpApi",
    "modules/X.Abp.IdentityServer.Pro/src/X.Abp.IdentityServer.Pro.HttpApi.Client",
    "modules/X.Abp.IdentityServer.Pro/src/X.Abp.IdentityServer.Pro.Web",

    ## modules/X.Abp.Iot
    # "modules/X.Abp.Iot/src/X.Abp.Iot.Application",
    # "modules/X.Abp.Iot/src/X.Abp.Iot.Application.Contracts",
    # "modules/X.Abp.Iot/src/X.Abp.Iot.Domain",
    # "modules/X.Abp.Iot/src/X.Abp.Iot.Domain.Shared",
    # "modules/X.Abp.Iot/src/X.Abp.Iot.EmqxGrpcService",
    # "modules/X.Abp.Iot/src/X.Abp.Iot.EntityFrameworkCore",
    # "modules/X.Abp.Iot/src/X.Abp.Iot.HttpApi",
    # "modules/X.Abp.Iot/src/X.Abp.Iot.HttpApi.Client",
    # "modules/X.Abp.Iot/src/X.Abp.Iot.Web",

    ## modules/X.Abp.LanguageManagement
    "modules/X.Abp.LanguageManagement/src/X.Abp.LanguageManagement.Application",
    "modules/X.Abp.LanguageManagement/src/X.Abp.LanguageManagement.Application.Contracts",
    "modules/X.Abp.LanguageManagement/src/X.Abp.LanguageManagement.Domain",
    "modules/X.Abp.LanguageManagement/src/X.Abp.LanguageManagement.Domain.Shared",
    "modules/X.Abp.LanguageManagement/src/X.Abp.LanguageManagement.EntityFrameworkCore",
    "modules/X.Abp.LanguageManagement/src/X.Abp.LanguageManagement.HttpApi",
    "modules/X.Abp.LanguageManagement/src/X.Abp.LanguageManagement.HttpApi.Client",
    "modules/X.Abp.LanguageManagement/src/X.Abp.LanguageManagement.Web",

    ## modules/X.Abp.LeptonTheme
    # "modules/X.Abp.LeptonTheme/src/X.Abp.AspNetCore.Components.Web.LeptonTheme",
    "modules/X.Abp.LeptonTheme/src/X.Abp.AspNetCore.Mvc.UI.Theme.Lepton",
    "modules/X.Abp.LeptonTheme/src/X.Abp.LeptonTheme.Management.Application",
    "modules/X.Abp.LeptonTheme/src/X.Abp.LeptonTheme.Management.Application.Contracts",
    "modules/X.Abp.LeptonTheme/src/X.Abp.LeptonTheme.Management.Domain",
    "modules/X.Abp.LeptonTheme/src/X.Abp.LeptonTheme.Management.Domain.Shared",
    # "modules/X.Abp.LeptonTheme/src/X.Abp.LeptonTheme.Management.EntityFrameworkCore",
    "modules/X.Abp.LeptonTheme/src/X.Abp.LeptonTheme.Management.HttpApi",
    "modules/X.Abp.LeptonTheme/src/X.Abp.LeptonTheme.Management.HttpApi.Client",
    "modules/X.Abp.LeptonTheme/src/X.Abp.LeptonTheme.Management.Web",

    ## modules/X.Abp.Localization
    "modules/X.Abp.Localization/src/X.Abp.Localization.CultureMap",

    ## modules/X.Abp.Notification
    "modules/X.Abp.Notification/src/X.Abp.Notification.Abstractions",
    "modules/X.Abp.Notification/src/X.Abp.Notification.Application",
    "modules/X.Abp.Notification/src/X.Abp.Notification.Application.Contracts",
    "modules/X.Abp.Notification/src/X.Abp.Notification.Domain",
    "modules/X.Abp.Notification/src/X.Abp.Notification.Domain.Shared",
    "modules/X.Abp.Notification/src/X.Abp.Notification.Email",
    "modules/X.Abp.Notification/src/X.Abp.Notification.EntityFrameworkCore",
    "modules/X.Abp.Notification/src/X.Abp.Notification.HttpApi",
    "modules/X.Abp.Notification/src/X.Abp.Notification.HttpApi.Client",
    "modules/X.Abp.Notification/src/X.Abp.Notification.SignalR",
    "modules/X.Abp.Notification/src/X.Abp.Notification.Sms",
    "modules/X.Abp.Notification/src/X.Abp.Notification.Web",

    ## modules/X.Abp.OpenIddict.Pro
    "modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.Application",
    "modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.Application.Contracts",
    # "modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.AspNetCore",
    "modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.Domain",
    "modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.Domain.Shared",
    "modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.EntityFrameworkCore",
    "modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.HttpApi",
    "modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.HttpApi.Client",
    "modules/X.Abp.OpenIddict.Pro/src/X.Abp.OpenIddict.Pro.Web",

    ## modules/X.Abp.Payment
    "modules/X.Abp.Payment/src/X.Abp.Payment.Admin.Application",
    "modules/X.Abp.Payment/src/X.Abp.Payment.Admin.Application.Contracts",
    "modules/X.Abp.Payment/src/X.Abp.Payment.Admin.HttpApi",
    "modules/X.Abp.Payment/src/X.Abp.Payment.Admin.HttpApi.Client",
    "modules/X.Abp.Payment/src/X.Abp.Payment.Admin.Web",
    "modules/X.Abp.Payment/src/X.Abp.Payment.Application",
    "modules/X.Abp.Payment/src/X.Abp.Payment.Application.Contracts",
    "modules/X.Abp.Payment/src/X.Abp.Payment.Domain",
    "modules/X.Abp.Payment/src/X.Abp.Payment.Domain.Shared",
    "modules/X.Abp.Payment/src/X.Abp.Payment.EntityFrameworkCore",
    "modules/X.Abp.Payment/src/X.Abp.Payment.HttpApi",
    "modules/X.Abp.Payment/src/X.Abp.Payment.HttpApi.Client",
    "modules/X.Abp.Payment/src/X.Abp.Payment.Web",

    ## modules/X.Abp.Quartz
    "modules/X.Abp.Quartz/src/X.Abp.Quartz.Application",
    "modules/X.Abp.Quartz/src/X.Abp.Quartz.Application.Contracts",
    "modules/X.Abp.Quartz/src/X.Abp.Quartz.Domain",
    "modules/X.Abp.Quartz/src/X.Abp.Quartz.Domain.Shared",
    "modules/X.Abp.Quartz/src/X.Abp.Quartz.HttpApi",
    "modules/X.Abp.Quartz/src/X.Abp.Quartz.HttpApi.Client",
    "modules/X.Abp.Quartz/src/X.Abp.Quartz.InstallScript.MySql",
    "modules/X.Abp.Quartz/src/X.Abp.Quartz.InstallScript.SqlServer",
    "modules/X.Abp.Quartz/src/X.Abp.Quartz.Web",

    ## modules/X.Abp.Saas
    "modules/X.Abp.Saas/src/X.Abp.Saas.Domain",
    "modules/X.Abp.Saas/src/X.Abp.Saas.Domain.Shared",
    "modules/X.Abp.Saas/src/X.Abp.Saas.EntityFrameworkCore",
    "modules/X.Abp.Saas/src/X.Abp.Saas.Application",
    "modules/X.Abp.Saas/src/X.Abp.Saas.Application.Contracts",
    "modules/X.Abp.Saas/src/X.Abp.Saas.HttpApi",
    "modules/X.Abp.Saas/src/X.Abp.Saas.HttpApi.Client",
    "modules/X.Abp.Saas/src/X.Abp.Saas.Web",

    ## modules/X.Abp.TextTemplateManagement
    "modules/X.Abp.TextTemplateManagement/src/X.Abp.TextTemplateManagement.Application",
    "modules/X.Abp.TextTemplateManagement/src/X.Abp.TextTemplateManagement.Application.Contracts",
    "modules/X.Abp.TextTemplateManagement/src/X.Abp.TextTemplateManagement.Domain",
    "modules/X.Abp.TextTemplateManagement/src/X.Abp.TextTemplateManagement.Domain.Shared",
    "modules/X.Abp.TextTemplateManagement/src/X.Abp.TextTemplateManagement.EntityFrameworkCore",
    "modules/X.Abp.TextTemplateManagement/src/X.Abp.TextTemplateManagement.HttpApi",
    "modules/X.Abp.TextTemplateManagement/src/X.Abp.TextTemplateManagement.HttpApi.Client",
    "modules/X.Abp.TextTemplateManagement/src/X.Abp.TextTemplateManagement.Web",

    ## templates/X.Abp.Templates
    "templates/X.Abp.Templates",

    ## modules/X.Abp.VersionManagement
    "modules/X.Abp.VersionManagement/src/X.Abp.VersionManagement.Application",
    "modules/X.Abp.VersionManagement/src/X.Abp.VersionManagement.Application.Contracts",
    "modules/X.Abp.VersionManagement/src/X.Abp.VersionManagement.Domain",
    "modules/X.Abp.VersionManagement/src/X.Abp.VersionManagement.Domain.Shared",
    "modules/X.Abp.VersionManagement/src/X.Abp.VersionManagement.EntityFrameworkCore",
    "modules/X.Abp.VersionManagement/src/X.Abp.VersionManagement.HttpApi",
    "modules/X.Abp.VersionManagement/src/X.Abp.VersionManagement.HttpApi.Client",
    "modules/X.Abp.VersionManagement/src/X.Abp.VersionManagement.Web"
)
