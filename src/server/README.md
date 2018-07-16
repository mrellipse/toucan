# Toucan - Api & Services

## ./src/contracts
This assembly contains Interface Contracts and Abstract Classes. This is in accordance with [Dependency Inversion Pattern](https://en.wikipedia.org/wiki/Dependency_inversion_principle#Traditional_layers_pattern)

> "interfaces belong to their clients not to their implementations"

## ./src/common
This assembly contains stand-alone classes, functions and extension methods to be used from any calling context.

Should only be _**referenced by**_ other assemblies in the solution.

## ./src/data
Since Entity Framework Core is being used as the ORM, this assembly contains all Entity Framework related code such as 

- Entity models
- Data context
- Data migrations
- Seeding routines

See [Getting started with Asp.NET Core MVC and Entity Framework Core using Visual Studio](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro) for an introduction

## ./src/service
This is the 'domain' or 'business' layer. It contains concrete classes to be injected into the [composition root](http://blog.ploeh.dk/2011/07/28/CompositionRoot/) of your web (or console) application.

> A composition root is a (preferably) unique location in an application where modules are composed together.

Any classes created in here should implement interfaces or abstract classes defined in the 'contracts' assembly. 

> Program to an interface, not an implementation.

## ./src/server (MVC)
Below is a list of middleware and filters found in this template, and information on the purpose they serve when processing browser requests.

### Authentication Tokens
If you want to alter the data contained in tokens issued to clients you can do this by editing the extension method `ToClaimsIdentity()` in the './src/service/extensions/user.cs'

This method is responsible for converting the user model into a [ClaimsIdentity](http://bitoftech.net/2015/03/31/asp-net-web-api-claims-authorization-with-asp-net-identity-2-1/), which is later transformed into a JWT token

### Middleware

The following customizations have been made

#### Authentication Challenges
If an authorization check on the web server fails, a challenge is issued to the client browser. Normally this results in a 302 redirect, but this behaviour has been modified using custom middleware.

The server instead returns a 401 Unauthorized response whilst setting Response header _Location=XXX_.

The client SPA must interpret this response correctly, and decide whether to prompt the user to login again, or display a simple 'you do not have required permissions' message

#### Client & Server Side Routing
Custom middleware is used to help with scenarios when the browser is requesting a page, and the path may translate into a _meaningful route on the client side_ - but its not known to the web server.

In this situation, if no other handler wants to process an inbound request, this middleware will try and return the contents of a specific html (ie. a html file emitted by the Webpack build process that can serve as an entry point into the SPA)

```csharp
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        // ...

        app.UseStaticFiles(staticFileOptions);
        app.UseMvc();
        app.UseHistoryModeMiddleware(webRoot, cfg.Server.Areas); // last middleware handler registered
    }
```

### Filters

The following customizations have been made

#### ApiResult Filter
When applied to a controller or action this filter returns a structured JSON payload to the client. When applied together with the `ApiExceptionFilter`, it can handle the two most common scenarios when calling an API (success/failure)

```csharp
public async Task<object> ControllerAction()
{
    var data = await this.myService.GetData();
    return data; // filter will wrap this into a structured Payload<T>
}
```

#### ApiExceptionFilter

When applied to a controller or action this filter will inspect unhandled errors thrown and

- intervene, and return a structured json payload to the client **or**
- pass the error on to be dealt with by global exception handling

```csharp
public class Payload<T>
    {
        public Payload()
        {
            this.Message = new PayloadMessage();
        }

        public T Data { get; set; }
        public PayloadMessage Message { get; set; }
    }
```

#### Identity Mapping Filter

This filter will retreive current user details from HTTP context, and resolve them from the underlying data store. 

```csharp

    [ServiceFilter(typeof(Filters.IdentityMappingFilter))]
    public class MyController : Controller
    {
        public async Task<object> ControllerAction()
        {
            Object data = null;
            Contract.IUser user = this.ApplicationUser(); // accessible via extension method
            
            if(user != null)
                data = await this.myService.GetData(user);

            return data;
        }
    }
```