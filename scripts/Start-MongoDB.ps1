#Requires -Version 5.1
$ErrorActionPreference = "Stop"

$port = 27017
$listening = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
if ($listening) {
    Write-Host "MongoDB already listening on port $port."
    exit 0
}

$serviceNames = @("MongoDB", "MongoDB Server")
foreach ($name in $serviceNames) {
    $service = Get-Service -Name $name -ErrorAction SilentlyContinue
    if ($null -ne $service) {
        if ($service.Status -ne "Running") {
            Write-Host "Starting Windows service '$($service.Name)'..."
            Start-Service -Name $service.Name
        }

        Start-Sleep -Seconds 3
        $listening = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
        if ($listening) {
            Write-Host "MongoDB is running on port $port."
            exit 0
        }
    }
}

$mongodCandidates = @(
    "C:\Program Files\MongoDB\Server\8.0\bin\mongod.exe",
    "C:\Program Files\MongoDB\Server\7.0\bin\mongod.exe",
    "C:\Program Files\MongoDB\Server\6.0\bin\mongod.exe"
)

foreach ($mongod in $mongodCandidates) {
    if (Test-Path $mongod) {
        $dataPath = Join-Path $env:LOCALAPPDATA "MCQSystem\mongo-data"
        $logPath = Join-Path $env:LOCALAPPDATA "MCQSystem\mongo-log"
        New-Item -ItemType Directory -Force -Path $dataPath, $logPath | Out-Null

        Write-Host "Starting mongod from $mongod ..."
        Start-Process -FilePath $mongod -ArgumentList @(
            "--dbpath", "`"$dataPath`"",
            "--logpath", "`"$(Join-Path $logPath 'mongod.log')`"",
            "--port", "$port"
        ) -WindowStyle Hidden

        Start-Sleep -Seconds 4
        $listening = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
        if ($listening) {
            Write-Host "MongoDB is running on port $port."
            exit 0
        }
    }
}

if (Get-Command docker -ErrorAction SilentlyContinue) {
    $repoRoot = Split-Path -Parent $PSScriptRoot
    Write-Host "Trying Docker Compose from $repoRoot ..."
    Push-Location $repoRoot
    docker compose up -d mongodb
    Pop-Location
    Start-Sleep -Seconds 5

    $listening = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    if ($listening) {
        Write-Host "MongoDB is running in Docker on port $port."
        exit 0
    }
}

Write-Host ""
Write-Host "MongoDB is not installed or could not be started." -ForegroundColor Red
Write-Host ""
Write-Host "Install MongoDB Community Server:"
Write-Host "  https://www.mongodb.com/try/download/community"
Write-Host ""
Write-Host "Or install Docker Desktop, then from the repo root run:"
Write-Host "  docker compose up -d"
Write-Host ""
Write-Host "Or use MongoDB Atlas and set MongoDb:ConnectionString in:"
Write-Host "  src\WebUI\appsettings.Development.json"
exit 1
