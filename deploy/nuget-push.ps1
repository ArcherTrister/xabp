param(
    [string]$url,
    [string]$key
)

. ..\nupkg\common.ps1

if (!$key) {
    #reading password from file content
    $passwordFileName = "nuget-api-key.txt"
    $pathExists = Test-Path -Path $passwordFileName -PathType Leaf
    if ($pathExists) {
        $key = Get-Content $passwordFileName
        echo "Using NuGet API Key from $passwordFileName ..."
    }
}

Write-Info "Pushing packages to NuGet"
echo "`n-----=====[ PUSHING PACKAGES TO NUGET ]=====-----`n"
cd ..\nupkg
powershell -File push_packages.ps1 $url $key
echo "`n-----=====[ PUSHING PACKAGES TO NUGET COMPLETED ]=====-----`n"
Write-Info "Completed: Pushing packages to NuGet"

cd ..\deploy #always return to the deploy directory
