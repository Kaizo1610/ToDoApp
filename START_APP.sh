#!/bin/bash

# Todo App - Quick Start Script for Linux/Mac
# This script starts both backend and frontend in separate terminal windows

echo ""
echo "========================================"
echo "  Todo App - Starting Application"
echo "========================================"
echo ""

# Check if .NET is installed
echo "Checking for .NET SDK..."
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET SDK not found. Please install .NET 10.0 or later."
    echo "Download from: https://dotnet.microsoft.com/download"
    exit 1
fi

# Check if Node is installed
echo "Checking for Node.js..."
if ! command -v node &> /dev/null; then
    echo "ERROR: Node.js not found. Please install Node.js 18 or later."
    echo "Download from: https://nodejs.org/"
    exit 1
fi

echo "✓ .NET SDK found: $(dotnet --version)"
echo "✓ Node.js found: $(node --version)"
echo ""

# Get the script's directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

# Function to detect OS
detect_os() {
    if [[ "$OSTYPE" == "darwin"* ]]; then
        echo "mac"
    else
        echo "linux"
    fi
}

OS=$(detect_os)

# Start Backend
echo "Starting Backend API..."
echo ""

if [ "$OS" == "mac" ]; then
    open -a Terminal.app "cd '$SCRIPT_DIR/TodoApp.Api' && dotnet run"
else
    # Linux - use xterm or gnome-terminal
    if command -v gnome-terminal &> /dev/null; then
        gnome-terminal -- bash -c "cd '$SCRIPT_DIR/TodoApp.Api' && dotnet run; bash"
    elif command -v xterm &> /dev/null; then
        xterm -e "cd '$SCRIPT_DIR/TodoApp.Api' && dotnet run" &
    else
        echo "Warning: Could not detect terminal. Please start backend manually:"
        echo "  cd $SCRIPT_DIR/TodoApp.Api && dotnet run"
    fi
fi

sleep 3

# Start Frontend
echo "Starting Frontend (React)..."
echo ""

# Check if node_modules exists, install if needed
if [ ! -d "todo-frontend/node_modules" ]; then
    echo "Installing dependencies..."
    cd "$SCRIPT_DIR/todo-frontend"
    npm install
    cd "$SCRIPT_DIR"
fi

if [ "$OS" == "mac" ]; then
    open -a Terminal.app "cd '$SCRIPT_DIR/todo-frontend' && npm run dev"
else
    # Linux
    if command -v gnome-terminal &> /dev/null; then
        gnome-terminal -- bash -c "cd '$SCRIPT_DIR/todo-frontend' && npm run dev; bash"
    elif command -v xterm &> /dev/null; then
        xterm -e "cd '$SCRIPT_DIR/todo-frontend' && npm run dev" &
    else
        echo "Warning: Could not detect terminal. Please start frontend manually:"
        echo "  cd $SCRIPT_DIR/todo-frontend && npm run dev"
    fi
fi

echo ""
echo "========================================"
echo "  Application Starting..."
echo "========================================"
echo ""
echo "Backend API:    http://localhost:5035"
echo "Swagger UI:     http://localhost:5035/swagger"
echo "Frontend:       http://localhost:5173"
echo ""
echo "Two new terminal windows should open for backend and frontend."
echo "If not, please start them manually."
echo ""
echo "Press Ctrl+C to stop."
