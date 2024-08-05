# Phase de base : téléchargement de l'image de base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /source

# Exposer les ports nécessaires pour la production
EXPOSE 5195
EXPOSE 7251

# Phase de construction
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


# Copie des fichiers du projet et projet de test
COPY TasksManagement_API/*.csproj TasksManagement_API/
COPY TasksManagement_Tests/*.csproj TasksManagement_Tests/

# Restauration des dépendances
RUN dotnet restore "TasksManagement_API/TasksManagement_API.csproj" --disable-parallel
RUN dotnet restore "TasksManagement_Tests/TasksManagement_Tests.csproj"

# Copie du reste du code et construction du projet de build et de test
COPY  TasksManagement_API/ TasksManagement_API/
COPY TasksManagement_Tests/ TasksManagement_Tests/
RUN dotnet build TasksManagement_API/TasksManagement_API.csproj -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet build TasksManagement_Tests/TasksManagement_Tests.csproj -c $BUILD_CONFIGURATION -o /app/test-build

RUN dotnet tool install --global dotnet-ef --version 6.0.20
ENV PATH="$PATH:/root/.dotnet/tools"

# Migration du context de base de données
RUN echo "Starting migration phase..." && \
    /root/.dotnet/tools/dotnet-ef database update  --project TasksManagement_API/TasksManagement_API.csproj  || { echo 'EF migration failed'; exit 1; }

# Exécution des tests
RUN dotnet test TasksManagement_Tests/TasksManagement_Tests.csproj --no-build --collect:"XPlat Code Coverage" --results-directory /TestResults -v d
#----------------------------------------------------------------------------------------------------------------------------------------------------------

# Phase de publication
FROM build AS publish
WORKDIR /src
COPY --from=build /app/* /app/build
RUN dotnet publish "./TasksManagement_API/TasksManagement_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish || { echo 'dotnet publish failed'; exit 1; }
#-----------------------------------------------------------------------------------------------------------------------------------------------------------
  
# Phase finale d'exécution (RUNTIME)
FROM base AS runtime
WORKDIR /source
# Copier les fichiers publiés de l'image build
COPY --from=publish /app/publish .
COPY TasksManagement_API/appsettings.Production.json ./appsettings.Production.json
COPY ApiNet6Certificate.pfx /https/certificate.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/certificate.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=lambo
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "TasksManagement_API.dll"]
#------------------------------------------------------------------------------------------------------------------------------------------------------------