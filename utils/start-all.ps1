# start-all.ps1

# Iniciar frontend en nueva ventana de PowerShell
Start-Process powershell -ArgumentList "cd ../frontend; npm install; npm run serve"

# Iniciar backend en nueva ventana de PowerShell
Start-Process powershell -ArgumentList "cd ../backend; dotnet run"