# param(
#     [string]$key,
#     [string]$version
# )

# # NuGet API URL
# $sourceUrl = "https://api.nuget.org/v3/index.json"

# # 获取当前文件夹中的所有 .nupkg 文件
# $nupkgFiles = Get-ChildItem -Filter *.nupkg

# $i = 0
# $errorCount = 0
# $totalProjectsCount = $nupkgFiles.Count

# # 遍历每个 .nupkg 文件
# foreach ($file in $nupkgFiles) {
#     $i += 1
#     # $nupkgPath = $file.FullName
#     # $packageInfo = [System.IO.Path]::GetFileNameWithoutExtension($nupkgPath)
#     $nupkgFileName = $file.Name
#     # 移除版本号和 .nupkg 后缀
#     $packageId = $nupkgFileName.Replace(".$version.nupkg", "")

#     # 输出正在尝试删除的包信息
#     Write-Info "[$i / $totalProjectsCount] - Deleting package $packageId.$version"

#     # 构建删除命令
#     $deleteCommand = "../tools/nuget/nuget delete $packageId $version -Source $sourceUrl -ApiKey $key -NonInteractive"

#     # 执行删除命令
#     try {
#         Invoke-Expression $deleteCommand
#         Write-Host "Package '$packageId.$version' deleted successfully."
#     }
#     catch {
#         Write-Error "Failed to delete package '$packageId.$version'. Error: $_"
#         $errorCount += 1
#     }
#     Write-Host "--------------------------------------------------------------`r`n"
# }

# # Write-Host "All specified packages have been processed."

# if ($errorCount -gt 0) {
#     Write-Host ("******* $errorCount error(s) occured *******") -ForegroundColor red
# }

. ".\common.ps1"

$nugetUrl = $args[0]
if (!$nugetUrl) {
    Write-Host ("********** ERROR NUGET URL IS NULL") -ForegroundColor Red
    $nugetUrl = Read-Host "Please Input The Nuget Url"
}

$nugetApiKey = $args[1]
if (!$nugetApiKey) {
    Write-Host ("********** ERROR NUGET APIKEY IS NULL") -ForegroundColor Red
    $nugetApiKey = Read-Host "Please Input The Nuget ApiKey"
}

$version = $args[2]
if (!$version) {
    Write-Host ("********** ERROR NUGET PACKAGE VERSION IS NULL") -ForegroundColor Red
    $nugetApiKey = Read-Host "Please Input The Nuget Package Version"
}

# Publish all packages
$i = 0
$errorCount = 0
$totalProjectsCount = $projects.length
Set-Location $packFolder

foreach ($project in $projects) {
    $i += 1
    $projectName = ($project -split '/')[-1]
    $nugetPackageId = $projectName

    Write-Info "[$i / $totalProjectsCount] - Deleting package $nugetPackageId.$version"

    # 构建删除命令
    $deleteCommand = "../tools/nuget/nuget delete $nugetPackageId $version -Source $sourceUrl -ApiKey $key -NonInteractive"

    # 执行删除命令
    try {
        Invoke-Expression $deleteCommand
        Write-Host "Package '$nugetPackageId.$version' deleted successfully."
    }
    catch {
        Write-Error "Failed to delete package '$nugetPackageId.$version'. Error: $_"
        $errorCount += 1
    }

    Write-Host "--------------------------------------------------------------`r`n"
}

if ($errorCount -gt 0) {
    Write-Host ("******* $errorCount error(s) occured *******") -ForegroundColor red
}
