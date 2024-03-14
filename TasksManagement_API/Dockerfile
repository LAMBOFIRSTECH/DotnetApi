# Phase de construction
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Définition du répertoire de travail
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copie des fichiers du projet
COPY *.csproj ./dotnetapp/

# Restauration des dépendances
WORKDIR /app/dotnetapp
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore

# Copie du code source et publication de l'application
WORKDIR /app
COPY dotnetapp/. ./dotnetapp/
WORKDIR /app/dotnetapp
RUN dotnet publish -c Release  -o /app/out

# Phase de test
FROM build AS testrunner
WORKDIR /app/tests
COPY tests/. .
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

# Phase de développement
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS development
COPY . /app
WORKDIR /app/src
CMD dotnet run --no-launch-profile

# Phase finale
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/out .
ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid "${UID}" \
    appuser
USER appuser
ENTRYPOINT ["dotnet", "Tasks_WEB_API.dll"]
