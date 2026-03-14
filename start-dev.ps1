# Script per lanciare l'ambiente di sviluppo locale
# Uso: .\start-dev.ps1

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Starting Spendi Meno Semo DEV" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

# Terminal 1: Docker infrastructure
Write-Host "`n[1/3] Starting Docker infrastructure..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "docker-compose up postgres keycloak sqlserver pgadmin"
Start-Sleep -Seconds 2

# Terminal 2: Backend
Write-Host "[2/3] Starting Backend (.NET)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd $PSScriptRoot\server\services\expenses\Expenses.Api; dotnet run"
Start-Sleep -Seconds 2

# Terminal 3: Frontend
Write-Host "[3/3] Starting Frontend (Angular)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd $PSScriptRoot\client; npm start"

Write-Host "`n================================" -ForegroundColor Green
Write-Host "Dev environment started!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""
Write-Host "Services:" -ForegroundColor Cyan
Write-Host "  Frontend:   http://localhost:4200" -ForegroundColor White
Write-Host "  Backend:    http://localhost:5001" -ForegroundColor White
Write-Host "  Keycloak:   http://localhost:8080" -ForegroundColor White
Write-Host "  SQL Server: localhost:1433" -ForegroundColor White
Write-Host "  PgAdmin:    http://localhost:5050" -ForegroundColor White

