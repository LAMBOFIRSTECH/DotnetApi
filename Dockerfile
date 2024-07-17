# Phase de construction
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Définition du répertoire de travail
WORKDIR /source

# Copie des fichiers du projet
COPY ./TasksManagement_API/*.csproj ./TasksManagement_API/
# Restauration des dépendances
RUN dotnet restore "./TasksManagement_API/TasksManagement_API.csproj" --disable-parallel

COPY ./TasksManagement_API/ ./TasksManagement_API/
# Publication de la solution
RUN dotnet publish "./TasksManagement_API/TasksManagement_API.csproj"  -c  Release -o /app --no-restore
# Phase finale
FROM mcr.microsoft.com/dotnet/aspnet:6.0 
WORKDIR /apiRepo
COPY --from=build /app/ ./
COPY ./Certs/ApiNet6Certificate.pfx /apiRepo/certificate.pfx
# Définir les variables d'environnement pour le mot de passe du certificat
ENV Certificate__Password="lambo"

EXPOSE 5163
EXPOSE 7082
EXPOSE 7083
ENTRYPOINT ["dotnet", "TasksManagement_API.dll"]

