# DotnetApi project
----------------------------------------------------------------------------------------------------
Mettre en place une API swagger en net6.0 !!&&Artur01102021
----------------------------------------------------------------------------------------------------

## 1- Les prérequis

### 1.1- Dans le dépot DotnetApi on crée : TasksManagement_API et TasksManagement_Tests

- `dotnet new webapi -o TasksManagement_API`
- `dotnet new xunit -n TasksManagement_Tests`

![](ProjectFolders.png) 

#### 1.1.1- Ajout des packages via la console ou depuis NugetPackageManagement
 
- `dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 6.0.0`
- `dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version=6.0.0`
- `dotnet add package MySql.EntityFrameworkCore --version=6.0.0`
- `dotnet add package Microsoft.EntityFrameworkCore.Tools --version=6.0.0`
- `dotnet add package Microsoft.Extensions.Logging --version=6.0.0`
- `dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version=6.0.0`
- `dotnet add package BCrypt.Net-Next --version=4.0.3`
- `dotnet add package xunit --version=2.4.1`
- `dotnet add package xunit.runner.visualstudio --version=2.4.1`

#### 1.1.2- Ajout du csproj TasksManagement_API dans le projet de Tests Unitaire
```
  <ItemGroup>
    <ProjectReference Include="..\TasksManagement_API\TasksManagement_API.csproj"/>
  </ItemGroup>
``` 

## 2- Liens vers la version et l'etat de santé de l'api

- [ApiVersion](https://localhost:7082/version)
- [ApiHealthCheck](https://localhost:7082/health)

## 3- Organisation du code
- [X] Arborescence des dossiers
  > - [X] Création d'un dossier pour les filtres d'authentifications
  > - [X] Création d'un dossier pour les Interfaces
  > - [X] Création d'un dossier pour les filtres du Swagger
  > - [X] Création d'un dossier pour les services ou responsaiblités
  > - [X] Création d'un dossier pour un contexte de base de données
  > - [X] Création d'un dossier pour les modèles de données
  > - [X] Création d'un dossier pour les controllers

![](ProjectFolders_details.png) 

- [X] Principe SOLID
    > - [X] Single-Responsabilities
    > - [X] Open and Close
    > - [X] Interfaces-Segragation
    > - [X] Dependances-Inversion


## 4- Implémentation des tests 

- [X] Tests unitaires
    > - [X] Test de retour d'actions
    
- [ ] Tests de charge
    > - [ ] Mettre en place ...

- [ ] Tests de Sécurité
    > - [ ] Test d'injection SQL
    > - [ ] Test d'attaque XSS

## 5- Sécurisation des endpoints d'Api
 
- [X] Mettre en place les authorisations (de base et/ou via un token de connexion)
  >  - [X]  Utilisateur Admin : token JWT Bearer
  >  - [X]  Utilisateur non-Admin : une authentification de base {login:password}


## 6- Création de contexte de base de données par environnement
 
- [X] Environnement de Développement : Base de Données en mémoire
- [X] Environnement de Production :    Base de Données en SQL via MSSQL 


## 7- Endpoints d'API

### 7.1- UsersManagementController 

> - `/api/v1.1/GetAllUsers`
> - `/api/v1.1/GetSingleUser/{Nom}/{Role}`
> - `/api/v1.1/CreateUser`
> - `/api/v1.1/SetUserPassword/?{nom}&{mdp}`
> - `/api/v1.1/Delete/{Nom}/{Role}`

### 7.2- TasksManagementController

> - `/api/v1.1/SingleOrAllTasks` : Affiche la liste de toutes les taches.
> - `/api/v1.1/tache`
> - `/api/v1.1/tache/{titre}`
> - `/api/v1.1/tache`

### 7.3- AccessTokenController

> - `/api/v1.1/Login/{email}`

## 8- Representation du swagger d'api
![](TasksManagement_API.png)
![](schemaTaskManagement.png)
![](schema1TaskManagement.png)



