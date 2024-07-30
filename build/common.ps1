$full = $args[0]

# COMMON PATHS 

$rootFolder = (Get-Item -Path "./" -Verbose).FullName

# List of solutions used only in development mode
$solutionPaths = @(
    "../modules/X.Abp.Account.Pro",
    "../modules/X.Abp.AuditLogging",
    "../modules/X.Abp.Chat",
    "../modules/X.Abp.Cli",
    "../modules/X.Abp.CmsKit.Pro",
    "../modules/X.Abp.FileManagement",
    "../modules/X.Abp.Forms",
    "../modules/X.Abp.Gdpr",
    "../modules/X.Abp.Identity.Pro",
    "../modules/X.Abp.IdentityServer.Pro",
    "../modules/X.Abp.LanguageManagement",
    "../modules/X.Abp.LeptonTheme",
    "../modules/X.Abp.Localization",
    "../modules/X.Abp.Notification",
    "../modules/X.Abp.OpenIddict.Pro",
    "../modules/X.Abp.Payment",
    "../modules/X.Abp.Quartz",
    "../modules/X.Abp.Saas",
    "../modules/X.Abp.TextTemplateManagement",
    "../modules/X.Abp.VersionManagement",
    "../templates/X.Abp.Templates",
	)

if ($full -eq "-f")
{
	# List of additional solutions required for full build
	$solutionPaths += (

	) 
}else{ 
	Write-host ""
	Write-host ":::::::::::::: !!! You are in development mode !!! ::::::::::::::" -ForegroundColor red -BackgroundColor  yellow
	Write-host "" 
} 
