#!/bin/sh

echo "Starting migration phase..."
dotnet tool install --global dotnet-ef --version 6.0.20
export PATH="$PATH:/root/.dotnet/tools"
dotnet-ef database update --project TasksManagement_API/TasksManagement_API.csproj || { echo 'EF migration failed'; exit 1; }
