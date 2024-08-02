. ".\common.ps1"

# $nugetUrl = Read-Host "Please Input The Nuget Url(default: http://mynuget.lexun.cn/v3/index.json)"
# if (!$nugetUrl)
# {
#   # $nugetUrl = "https://api.nuget.org/v3/index.json"
#   $nugetUrl = "http://mynuget.lexun.cn/v3/index.json"
#   Write-Host ("NUGET URL IS NULL,USE DEFAULT URL:" + $nugetUrl) -ForegroundColor Green
# }

$apiKey = $args[0]
if (!$apiKey) {
  Write-Host ("********** ERROR NUGET APIKEY IS NULL") -ForegroundColor Red
  $apiKey = Read-Host "Please Input The Nuget ApiKey"
}

$nugetUrl = "http://127.0.0.1:8081/repository/xabp/"

# Get the version
[xml]$commonPropsXml = Get-Content (Join-Path $rootFolder "common.props")
$version = $commonPropsXml.Project.PropertyGroup.Version

# Publish all packages
$i = 0
$errorCount = 0
$totalProjectsCount = $projects.length
Set-Location $packFolder

foreach ($project in $projects) {
  $i += 1
  $projectFolder = Join-Path $rootFolder $project
  $projectName = ($project -split '/')[-1]
  $nugetPackageName = $projectName + "." + $version
  $nugetPackageName = $nugetPackageName.Trim() + ".nupkg"
  # $nugetPackageName = $projectName + "." + $version + ".nupkg"
  $nugetPackageExists = Test-Path $nugetPackageName -PathType leaf

  Write-Info "[$i / $totalProjectsCount] - Pushing: $nugetPackageName"

  if ($nugetPackageExists) {
    dotnet nuget push $nugetPackageName --skip-duplicate -s $nugetUrl --api-key "$apiKey"
    #Write-Host ("Deleting package from local: " + $nugetPackageName)
    #Remove-Item $nugetPackageName -Force
  }
  else {
    Write-Host ("********** ERROR PACKAGE NOT FOUND: " + $nugetPackageName) -ForegroundColor red
    $errorCount += 1
    #Exit
  }

  Write-Host "--------------------------------------------------------------`r`n"
}

if ($errorCount -gt 0) {
  Write-Host ("******* $errorCount error(s) occured *******") -ForegroundColor red
}
