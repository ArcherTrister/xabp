param(
    [string]$url,
    [string]$key,
    [string]$version
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

Write-Info "Deleting packages from NuGet"
echo "`n-----=====[ DELETING PACKAGES FROM NUGET ]=====-----`n"
cd ..\nupkg
powershell -File delete_packages.ps1 $url $key $version
echo "`n-----=====[ DELETING PACKAGES FROM NUGET COMPLETED ]=====-----`n"
Write-Info "Completed: Deleting packages from NuGet"

cd ..\deploy #always return to the deploy directory
