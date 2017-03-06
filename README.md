# Toucan

This is a multi-project .Net Core template designed as a starting point for building an SPA application using SOLID principles.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

* [.NET Core 1.1 SDK](https://www.microsoft.com/net/core/)
* [Visual Studio Code](https://code.visualstudio.com/download/) (or a similar text-based editor like Sublime)
* [Node.js](https://nodejs.org/en/)
* [TypeScript](https://www.typescriptlang.org/)

### Installing

Update dependencies and build the .Dot Net based server projects by switching to the root directory of the solution and running

```
dotnet restore ./src
dotnet build ./src/server/project.json
```

Update dependencies and build the Nodejs based UI project by switching to ./src/ui and running

```
npm install
npm install webpack -g
npm install typings -g
typings install
webpack -p --config webpack/development.js
```


dotnet ef --startup-project ../server migrations add Initial -c MssqlContext
The final step is to update the configuration file *./src/server/app.development.json*. Edit *data:connectionString* configuration key to point to a local (or remote) instance of SQL server.

Entity Framework migrations code will then create and seed the database upon server startup.

```
    "data": {
      "connectionString": "Server=(localdb)\\mssqllocaldb;Database=ToucanDevelopment;Trusted_Connection=True;"
    }
```

You should now be able to start the server by switching to ./src/server and running

```
dotnet run -p ./src/server/project.json -c Development
```

## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [.NET Core](https://www.microsoft.com/net/core) - .NET Core is a general purpose development platform maintained by Microsoft and the .NET community on GitHub.
* [xUnit.net](https://xunit.github.io/) - xUnit.net is a free, open source, community-focused unit testing tool for the .NET Framework.
* [StructureMap](http://structuremap.github.io/) - IOC/DI container
* [TypeScript](https://www.typescriptlang.org/) - Typescript is a typed superset of Javascript that compiles to plain JavaScript
* [Vue.js](https://vuejs.org/v2/guide/) - Simple yet powerful library for building modern web interfaces

## Contributing

It is what it is what is.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* [mrellipse](https://github.com/mrellipse)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* [Wille Ristimäki](https://github.com/villeristi) for [vue.js-starter-template](https://github.com/villeristi/vue.js-starter-template)
* [Nate Barbettini](https://github.com/nbarbettini) for [SimpleTokenProvider](https://github.com/nbarbettini/SimpleTokenProvider)
* Discussions on [Importing files other than TS modules](https://github.com/Microsoft/TypeScript/issues/2709)
