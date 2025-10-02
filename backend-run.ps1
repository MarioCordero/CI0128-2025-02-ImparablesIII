# Ruta al dotnet portable que descargaste
$dotnet = "C:\Users\C26240\Downloads\dotnet-sdk-8.0.414-win-x64\dotnet.exe"

# Ruta al backend (ajustá si tu repo está en otra carpeta)
$backendPath = "C:\Users\C26240\Desktop\PI\CI0128-2025-02-ImparablesIII\backend"

Write-Host "Usando .NET desde: $dotnet"
Write-Host "Proyecto backend en: $backendPath"

# Cambiar a la carpeta del backend
Set-Location $backendPath

# Forzar que use HTTP en el puerto 5000 (evita líos con certificados HTTPS)
$env:ASPNETCORE_URLS = "http://localhost:5000"

# Restaurar dependencias
& $dotnet restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Compilar
& $dotnet build
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Ejecutar
& $dotnet run