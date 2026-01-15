@echo off
REM Todo App - Quick Start Script for Windows
REM This script starts both backend and frontend in separate windows

echo.
echo ========================================
echo   Todo App - Starting Application
echo ========================================
echo.

REM Check if .NET is installed
echo Checking for .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK not found. Please install .NET 10.0 or later.
    echo Download from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

REM Check if Node is installed
echo Checking for Node.js...
node --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Node.js not found. Please install Node.js 18 or later.
    echo Download from: https://nodejs.org/
    pause
    exit /b 1
)

echo ✓ .NET SDK found
echo ✓ Node.js found
echo.

REM Get the script's directory
cd /d "%~dp0"

REM Start Backend
echo Starting Backend API...
echo.
cd TodoApp.Api
start "Todo App - Backend" cmd /k "dotnet run"
cd ..
timeout /t 3 /nobreak

REM Start Frontend
echo Starting Frontend (React)...
echo.
cd todo-frontend

REM Check if node_modules exists, install if needed
if not exist "node_modules" (
    echo Installing dependencies...
    call npm install
)

start "Todo App - Frontend" cmd /k "npm run dev"
cd ..

echo.
echo ========================================
echo   Application Starting...
echo ========================================
echo.
echo Backend API:    http://localhost:5035
echo Swagger UI:     http://localhost:5035/swagger
echo Frontend:       http://localhost:5173
echo.
echo Note: Two new windows should open for backend and frontend.
echo If not, please start them manually.
echo.
echo Press any key to close this window...
pause
