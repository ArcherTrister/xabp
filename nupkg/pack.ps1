. ".\common.ps1"

# Delete old packages
del *.nupkg
# Get-ChildItem * -include *.nupkg -recurse | Remove-Item

# Rebuild all solutions
# foreach($solution in $solutions) {
#     $solutionFolder = Join-Path $rootFolder $solution
#     Write-Info $solutionFolder
#     Set-Location $solutionFolder
#     dotnet restore
# }

# Write-Host 'Press Any Key!' -NoNewline
# $null = [Console]::ReadKey('?')

# Create all packages
$i = 0
$projectsCount = $projects.length
Write-Info "Running dotnet pack on $projectsCount projects..."

foreach ($project in $projects) {
    $i += 1
    $projectFolder = Join-Path $rootFolder $project
    $projectName = ($project -split '/')[-1]

    # Create nuget pack
    Write-Info "[$i / $projectsCount] - Packing project: $projectName"
    Set-Location $projectFolder
    dotnet clean

    dotnet pack -c Release

    # dotnet build -c Release
    # Write-Info $PSScriptRoot
    # $releasePath = Join-Path $projectFolder ("/bin/Release")
    # $frameworks = Get-ChildItem $releasePath | ?{$_.psiscontainer -eq $true}
    # $projectDll = Join-Path $projectFolder ("/bin/Release/" + $frameworks[0] + "/" + $projectName + ".dll")
    # $protectedDll = Join-Path $projectFolder ("/bin/Release/" + $frameworks[0] + "/" + $projectName + "_Secure/" + $projectName + "*.*")
    # $projectPath = Join-Path $projectFolder ("/bin/Release/" + $frameworks[0] + "/")
    # dotNET_Reactor.Console.exe -file $projectDll -anti_debug 1 -hide_calls 1 -hide_calls_internals 1 -control_flow_obfuscation 1 -flow_level 9 -virtualization 1 -necrobit 1 -necrobit_comp 1 -short_strings 1 -all_params 1 -showloadscreen 0 -q
    # Start-Sleep -s 3
    # Copy-Item -Force $protectedDll $projectPath
    # dotnet pack -c Release --no-build

    if (-Not $?) {
        Write-Error "Packaging failed for the project: $projectName"
        exit $LASTEXITCODE
    }

    # Move nuget package
    $projectName = $project.Substring($project.LastIndexOf("/") + 1)
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $projectName + ".*.nupkg")
    Write-Info $projectPackPath
    Move-Item -Force $projectPackPath $packFolder

    Seperator
}

# Go back to the pack folder
Set-Location $packFolder
