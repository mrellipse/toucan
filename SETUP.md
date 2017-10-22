# Toucan - Setup

## Prerequisites

* [.NET Core SDK 2.0.0](https://www.microsoft.com/net/core/#windowscmd)
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
dotnet build
```

Update and build the TypeScript UI project by switching to ./src/ui and running

```DOS
npm install
npm install webpack -g
npm install typings -g
typings install
webpack -p --config webpack.config.js
```

### Configuration

The first step in configuration is to decide what backing database to use. You can then ensure it is properly configured, and following that invoke scaffolding tools to generate initial data migrations.

#### For PostgreSQL

For proper localization support ensure the server defaults are set to create

* new databases with a [character set](https://www.postgresql.org/docs/9.1/static/multibyte.html) that supports unicode strings (ie. UTF8)
* client connections with default timezone set to 'UTC' for  (via postgresql.conf)

Above steps may require a restart of the service/daemon. You can then proceed to

* update *<data:connectionString>* configuration key inside src/server/app.development.json*
* update `ConfigureServices()` method in src/server/startup.cs*, and uncomment the code block starting `services.AddDbContext<NpgSqlContext>`
* update ./src/server/ContainerRegistry.cs, and uncomment the line stating`For<DbContextBase>().Use<NpgSqlContenxt>();`
* add the same connection string details to src/data/npgsql.json* (required for EF tooling)
* (optional) add a database schema name to src/data/npgsql.json*
* scaffold migrations by switching to src/data* and running
```DOS
dotnet ef --startup-project ../server migrations add Initial -c NpgSqlContext
```

#### For SQL Server

* update <data:connectionString>* configuration key inside src/server/app.development.json*
* update the `ConfigureServices()` method in src/server/startup.cs*, and uncomment the code block starting `services.AddDbContext<MsSqlContext>`
* update ./src/server/ContainerRegistry.cs, and uncomment the line stating`For<DbContextBase>().Use<MsSqlContext>();`
* add the same connection string details to src/data/mssql.json* (required for EF tooling)
* (optional) add a database schema name to src/data/mssql.json*
* create migrations by switching to src/data* and running
```DOS
dotnet ef --startup-project ../server migrations add Initial -c MsSqlContext
```

#### External Authentication

The system by default enables users to create a local login account using a signup page, which stores their credentials in the database.

It also enables users to login via an external authentication provider, using the [OAuth 2.0 Implicit](https://tools.ietf.org/html/rfc6749#section-1.3.2) grant workflow.

Most external providers will required https support. This is covered off fairly well by [Setting up HTTPS for development in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/https)

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
You should now be able to load the site at [http://localhost:5000/](http://localhost:5000/) 
