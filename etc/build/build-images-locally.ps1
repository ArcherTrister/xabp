param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../../"

$dbMigratorFolder = Join-Path $slnFolder "src/AbpVnext.Pro.DbMigrator"

Write-Host "********* BUILDING DbMigrator *********" -ForegroundColor Green
Set-Location $dbMigratorFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t abpvnext/pro-db-migrator:$version .



$angularAppFolder = Join-Path $slnFolder "../angular"
$hostFolder = Join-Path $slnFolder "src/AbpVnext.Pro.HttpApi.Host"

Write-Host "********* BUILDING Angular Application *********" -ForegroundColor Green
Set-Location $angularAppFolder
yarn
npm run build:prod
docker build -f Dockerfile.local -t abpvnext/pro-web:$version .

Write-Host "********* BUILDING Api.Host Application *********" -ForegroundColor Green
Set-Location $hostFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t abpvnext/pro-api:$version .




### ALL COMPLETED
Write-Host "COMPLETED" -ForegroundColor Green
Set-Location $currentFolder