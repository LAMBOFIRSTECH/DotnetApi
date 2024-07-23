# Phase de base : téléchargement de l'image de base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

# Définition du répertoire de travail pour l'image finale
WORKDIR /source

# Exposer les ports nécessaires
#ENV PROD

EXPOSE 5195
EXPOSE 7250 
# ENV DEV
# EXPOSE 5163
# EXPOSE 7083

# Phase de construction
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copie des fichiers du projet et restauration des dépendances
COPY TasksManagement_API/*.csproj TasksManagement_API/
RUN dotnet restore "TasksManagement_API/TasksManagement_API.csproj" --disable-parallel

# Copie du reste du code et publication de l'application
COPY  TasksManagement_API/ TasksManagement_API/
RUN dotnet build TasksManagement_API/TasksManagement_API.csproj -c Release -o /app/build

# Phase de publication
FROM build AS publish
WORKDIR /src
COPY --from=build /app/build /app/build
RUN dotnet publish "./TasksManagement_API/TasksManagement_API.csproj" -c Release -o /app/publish || { echo 'dotnet publish failed'; exit 1; }

# Phase finale d'exécution (RUNTIME)
FROM base AS final

# Définir le répertoire de travail
WORKDIR /source

# Copier les fichiers publiés de l'image build
COPY --from=publish /app/publish .

# Copier le certificat
COPY ApiNet6Certificate.pfx /https/certificate.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/certificate.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=lambo
# Définissez la variable d'environnement
ENV ASPNETCORE_ENVIRONMENT=Production

# Définir le point d'entrée de l'application
ENTRYPOINT ["dotnet", "TasksManagement_API.dll"]

