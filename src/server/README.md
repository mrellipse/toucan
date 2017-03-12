Below is a list of middleware and filters found in this template, and information on the purpose they serve when processing browser requests.

## Authentication Tokens
If you want to alter the data contained in tokens issued to clients you can do this by editing the extension method `ToClaimsIdentity()` in the './src/service/extensions/user.cs'

This method is responsible for converting the user model into a [ClaimsIdentity](http://bitoftech.net/2015/03/31/asp-net-web-api-claims-authorization-with-asp-net-identity-2-1/), which is later transformed into a JWT token

## Middleware

The following customizations have been made

### Authentication Challenges (302)
Normally, if an authorization check on the web server fails, a challenge is issued to the client browser. This results in a 302 redirect, but this behaviour has been modified using custom middleware in *./src/server/Startup.Auth.cs*.

This middleware will instead
- return a 200 OK response
- set the Location header with a value that should be passed to the client-side SPA router

It then relies on the client SPA to interpret this response correctly, which should either prompt the user to login, or display 'you do not have required permissions' message

### Client & Server Side Routing
Custom middleware is used to help with scenarios when the browser is requesting a page, and the path may translate into a _meaningful route on the client side_ - but its not known to the web server.

In this situation, if no other handler wants to process an inbound request, this middleware will try and return the contents of a specific html (ie. a html file emitted by the Webpack build process that can serve as an entry point into the SPA)

```csharp
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        // ...

        app.UseStaticFiles(staticFileOptions);
        app.UseMvc();
        app.UseHtml5HistoryMode(webRoot, cfg.Server.Areas); // last middleware handler registered
    }
```

## Filters

The following customizations have been made

### ApiResult Filter
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

### Identity Mapping Filter

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