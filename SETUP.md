# Toucan - Setup

## Prerequisites

* [.NET Core SDK 1.0.1](https://www.microsoft.com/net/core/#windowscmd)
* [Visual Studio Code](https://code.visualstudio.com/download/) (or a similar text-based editor like Sublime)
* [TypeScript](https://www.typescriptlang.org/)
* [Node.js](https://nodejs.org/en/)
* [PostgreSQL](https://www.postgresql.org/) or [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Installing

The easiest way is to install via Yeoman

```DOS
npm install generator-toucan
yo toucan
```

The alternative is to Git clone the project to a local folder and then follow steps below ...


### Project Dependencies

Update and build the .NET Core projects by switching to to ./src/server and running

```DOS
dotnet restore
dotnet build
```

Update and build the TypeScript UI project by switching to ./src/ui and running

```DOS
npm install
npm install webpack -g
npm install typings -g
typings install
webpack -p --config webpack/development.js
```

### Configuration

The first step in configuration is to decide what backing database to use, and then invoke scaffolding tools to generate initial data migrations.

#### For PostgreSQL

* update *<data:connectionString>* configuration key inside *./src/server/app.development.json*
* update `ConfigureServices()` method in *./src/server/startup.cs*, and uncomment the code block starting `services.AddDbContext<NpgSqlContext>`
* add the same connection string details to *./src/data/npgsql.json* (required for EF tooling)
* scaffold migrations by switching to *./src/data* and running
```DOS
dotnet ef --startup-project ../server migrations add Initial -c NpgSqlContext
```

#### For SQL Server

* update <data:connectionString>* configuration key inside *./src/server/app.development.json*
* update the `ConfigureServices()` method in *./src/server/startup.cs*, and uncomment the code block starting `services.AddDbContext<MsSqlContext>`
* add the same connection string details to *./src/data/mssql.json* (required for EF tooling)
* create migrations by switching to *./src/data* and running
```DOS
dotnet ef --startup-project ../server migrations add Initial -c MsSqlContext
```

#### External Authentication

The system by default enables users to create a local login account using a signup page, which stores their credentials in the database.

It also enables users to login via an external authentication provider, using the [OAuth 2.0 Implicit](https://tools.ietf.org/html/rfc6749#section-1.3.2) grant workflow.

The project currently provides support for

- [X] [Google](https://developers.google.com/identity/protocols/OAuth2UserAgent)
- [X] [Microsoft](https://msdn.microsoft.com/en-us/library/hh243647.aspx)

To configure the

* Google Provider - edit `const GOOGLE_CLIENT_ID` key in *google-provider.ts* file, and `clientId` in *app.development.json*
* Microsoft Provider - edit `const MICROSOFT_CLIENT_ID` key in *microsoft-provider.ts* file, and `clientId` in *app.development.json*

To remove a provider from appearing the UI, edit `externalProviders` class property in *login.ts*

### Startup
Run the project by switching to ./src/server and running

```DOS
dotnet run -p server.csproj -c Development
```
You should now be able to load the site at [https://localhost:5000/](https://localhost:5000/) 
