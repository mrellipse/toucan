# Toucan - Security

## Authentication
[JWT Bearer](https://tools.ietf.org/html/rfc7523) tokens are issued by the server. These tokens contain profile data and claims relating to a user. Authentication can be done in two different ways.

### via Local Provider
This requires a user to create an account using the signup form. In this scenario, the server

* enforces a minimum password complexity rule when signing up
* generates a salt, and stores hashed password to the database
* retreives and maps user profile data and permissions when generating client tokens

### via External Providers
This is performed using a variation on the [implicit workflow](https://tools.ietf.org/html/rfc6749#section-1.3.2). The sequence of events is as follows. 

* browser obtains a one-use nonce from local server (which must be redeemed/used before it expires)
* the browser is redirected to external provider, and authenticated
* external provider issues a redirect, and returns the nonce and access token details via uri (ie. _https://localhost:5001/#state=XYZ&access_token=4/P7q7W91&token_type=Bearer&expires_in=3600_)
* the Vue.js application is bootstrapped, and checks the uri hash for nonce and access_token
* if present, these are passed to the local server
* the local server validates the nonce and access token, and if satisfied, issues the client with a local token (the external access token is also revoked)

If this is the first time a user has logged in using this external provider, a user account will be created for them based on data from external profile data APIs.

## Authorization
Server-side routes are protected via [policies](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies) which provide more flexibility than traditional role-based models.

Client-side routes are protected via [navigation guards](http://router.vuejs.org/en/advanced/navigation-guards.html), which check user claims contained in access tokens issued by the server.

> If an authorization check on the web server fails, a challenge is issued to the client browser. Normally this results in a 302 redirect, but this behaviour has been modified using custom middleware in src/server/Startup.Auth.cs*. This middleware instead returns a 401 Unauthorized response whilst setting Response header _Location=XXX_. The client-side Axios library then handles this appropriately, by way of a global http response interceptor.

## Verification

Before a site user can access restricted content they need to verify their account details using some form of two-factor authentication. They are redirected to the verification page when appropriate.

The default implementation is as follows

* user asks site to issue a verification code
* server returns verification code
* client browser outputs the verification code using `console.info()` and displays form on page
* the verification code is submitted, and a new access token (with updated details) is issued by the server

The entire process is 'platform aware'. If a user logs in from a different browser or device, they will need to repeat the verification process again before the server will allow access to secured paths

## Requests

Support for [CSRF](https://en.wikipedia.org/wiki/Cross-site_request_forgery) has been provided.

> The site sets CSRF session cookies whenever an access token is issued

The Axios library automatically uses this cookie to append the anti-forgery token to outbound POST, PUT and DELETE requests.