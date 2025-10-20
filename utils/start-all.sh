#!/bin/bash
# start-all.sh

# Intenta abrir gnome-terminal (Linux)
if command -v gnome-terminal &> /dev/null
then
    gnome-terminal -- bash -c "cd frontend && npm install && npm run serve; exec bash"
    gnome-terminal -- bash -c "cd backend && dotnet run; exec bash"
# Si no hay gnome-terminal, intenta abrir Terminal en macOS con AppleScript
elif [[ "$OSTYPE" == "darwin"* ]]; then
    osascript -e 'tell app "Terminal"
        do script "cd '$(pwd)'/frontend && npm install && npm run serve"
    end tell'
    osascript -e 'tell app "Terminal"
        do script "cd '$(pwd)'/backend && dotnet run"
    end tell'
else
    echo "No se encontró un terminal compatible para abrir nuevas ventanas."
    echo "Puedes ejecutar manualmente:"
    echo "  cd frontend && npm install && npm run serve"
    echo "  cd backend && dotnet run"
fi