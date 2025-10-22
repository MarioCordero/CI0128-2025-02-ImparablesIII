¡Claro! Aquí tienes el README completo con toda la documentación lista para pegar al final de tu archivo:

---

## 🏗️ Project Architecture

This project is structured into two main parts: the **Frontend** and the **Backend**, each with clearly defined responsibilities.

---

### 📦 Frontend

* **Framework:** [Vue.js](https://vuejs.org/)
* **Directory:** `frontend/`
* **Role:** Manages the user interface and client-side logic.
* **Communication:** Sends HTTP requests to the backend API to fetch or send data.
* **Key Files:**

  * `App.vue`: Root component of the application.
  * `main.js`: Entry point of the Vue app.
  * Components located in `frontend/src/components/`.

#### 📁 Components

All UI components are located in:

```
frontend/src/components/
```

---

### 🧠 Backend

* **Framework:** [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
* **Directory:** `backend/`
* **Role:** Exposes RESTful API endpoints and handles:

  * Business logic
  * Data access
  * Database operations
* **Key Files:**

  * `Program.cs`: Application entry point.
  * `Controllers/`: Handles incoming HTTP requests.
  * `Services/`: Contains business logic.
  * `Repositories/`: Manages data persistence.

---

## 🚀 How to Start the Project

You can start the frontend and backend services simultaneously using the provided scripts for your operating system.

### 🟦 Windows (PowerShell)

* Script: `start-all.ps1`
* What it does:

  * Opens two new PowerShell windows.
  * Runs `npm install` and `npm run serve` in the `frontend` folder.
  * Runs `dotnet run` in the `backend` folder.

#### How to run:

1. Open PowerShell and navigate to the project root.

2. (If needed) Allow script execution temporarily:

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
   ```

3. Run the script:

   ```powershell
   ./start-all.ps1
   ```

---

### 🐧 Linux/macOS (Bash)

* Script: `start-all.sh`
* What it does:

  * Opens two new terminal windows.
  * Runs `npm install` and `npm run serve` in the `frontend` folder.
  * Runs `dotnet run` in the `backend` folder.

#### How to run:

1. Give execution permission:

   ```bash
   chmod +x start-all.sh
   ```

2. Run the script:

   ```bash
   ./start-all.sh
   ```

---

> **Note:**
>
> * Make sure you have `gnome-terminal` installed on Linux or use macOS Terminal (the script uses AppleScript to open it).
> * For Windows, scripts require PowerShell.

---