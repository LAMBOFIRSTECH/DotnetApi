# Phase de construction
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Définition du répertoire de travail
WORKDIR /source

# Copie des fichiers du projet et restauration des dépendances
COPY ./TasksManagement_API/*.csproj ./TasksManagement_API/
RUN dotnet restore "./TasksManagement_API/TasksManagement_API.csproj" --disable-parallel

# Copie du reste du code et publication de l'application
COPY ./TasksManagement_API/ ./TasksManagement_API/
RUN dotnet publish "./TasksManagement_API/TasksManagement_API.csproj" -c Release -o /app --no-restore || { echo 'dotnet publish failed'; exit 1; }

# Phase finale
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Définir le répertoire de travail
WORKDIR /apiRepo

# Copier les fichiers publiés de l'image build
COPY --from=build /app/ ./

# Copier le certificat
COPY ./Certs/ApiNet6Certificate.pfx /apiRepo/certificate.pfx

# Définir les variables d'environnement pour le mot de passe du certificat
ENV Certificate__Password="lambo"

# Exposer les ports nécessaires
EXPOSE 5163
EXPOSE 7082
EXPOSE 7083

# Définir le point d'entrée de l'application
ENTRYPOINT ["dotnet", "TasksManagement_API.dll"]
