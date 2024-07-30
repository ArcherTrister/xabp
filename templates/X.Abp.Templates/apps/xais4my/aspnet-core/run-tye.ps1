<# Check development certificates #>

if (! (  Test-Path ".\etc\dev-cert\localhost.pfx" -PathType Leaf ) ) {
    Write-Information "Creating dev certificates..."
    cd ".\etc\dev-cert"
    .\create-certificate.ps1
    cd ..
}

<# Check Docker containers #>

$requiredServices = @(
    'grafana',
    'prometheus',
    'kibana',
    'rabbitmq',
    'elasticsearch',
    'redis'
)

foreach ($requiredService in $requiredServices) {

    $nameParam = -join ("name=", $requiredService)
    $serviceRunningStatus = docker ps --filter $nameParam
    $isDockerImageUp = $serviceRunningStatus -split " " -contains $requiredService

    if ( $isDockerImageUp ) {
        Write-Host ($requiredService + " [up]")
    }
    else {
        cd "./etc/docker/"
        docker network create abpvnext.microservice-network
        docker-compose -f docker-compose.infrastructure.yml -f docker-compose.infrastructure.override.yml up -d
        cd ../..
        break;
    }
}


<# Run all services #>

tye run --port 8070 --watch
