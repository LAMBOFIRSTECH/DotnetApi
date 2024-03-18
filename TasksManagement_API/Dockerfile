# Phase de construction
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Définition du répertoire de travail
WORKDIR /source

# Copie des fichiers du projet
COPY . . 
# Restauration des dépendances
RUN dotnet restore "TasksManagement_API.csproj" --disable-parallel

# Publication de la solution
RUN dotnet publish "TasksManagement_API.csproj"  -c  Release -o /app --no-restore


# Phase finale
FROM mcr.microsoft.com/dotnet/aspnet:6.0 
WORKDIR /apiRepo
COPY --from=build /app/ ./

EXPOSE 5163
EXPOSE 7082

ENTRYPOINT ["dotnet", "TasksManagement_API.dll"]
