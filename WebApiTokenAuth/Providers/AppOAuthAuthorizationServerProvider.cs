using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using WebApiTokenAuth.DAL;
using WebApiTokenAuth.Models;

namespace WebApiTokenAuth.Providers
{
    public class AppOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            var clientId = string.Empty;
            var clientSecret = string.Empty;

            Client client = null;

            // To acquire those values if present in the request header.
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                // To acquire those values if present in the request body (x-www-form-urlencoded)
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            //if (context.ClientId == null)
            //{
            //    //Remove the comments from the below line context.SetError, and invalidate context 
            //    //if you want to force sending clientId/secrects once obtain access tokens. 
            //    context.Validated();
            //    //context.SetError("invalid_clientId", "ClientId should be sent.");
            //    return Task.FromResult<object>(null);
            //}

            // Check if client id is empty or null.
            if (string.IsNullOrWhiteSpace(clientId))
            {
                context.SetError("invalid_client", string.Format("client id should be sent."));
                return Task.FromResult<object>(null);
            }

            // Get the details regarding the client id provided.
            using (var repository = new AuthRepository())
            {
                client = repository.FindClient(clientId);
            }

            // Check if client is null.
            if (client == null)
            {
                context.SetError("invalid_client", string.Format("client id '{0}' is not registered in the system.", clientId));
                return Task.FromResult<object>(null);
            }

            // Check if application is Native application or not. 
            if (client.ApplicationType == Models.ApplicationType.NativeConfidential)
            {
                // Native application should provide client secret.
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_client", "client secret should be sent.");
                    return Task.FromResult<object>(null);
                }

                // Check if client secret is matches with the one in database.
                if (client.ClientSecret != clientSecret)
                {
                    context.SetError("invalid_client", "client secret is invalid.");
                    return Task.FromResult<object>(null);
                }
            }

            // Check if client is active 
            if (!client.Active)
            {
                context.SetError("invalid_client", "client is inactive.");
                return Task.FromResult<object>(null);
            }

            // Set key value in Owin environment for further use.
            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            // Validate the context.
            context.Validated(clientId);

            return base.ValidateClientAuthentication(context);
        }

        // Called when grant_type is set as client_credentials and after ValidateClientAuthentication method.
        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            // Get the value from Owin envrironment.
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin") ?? "*";

            // Set the reponse Access-Control-Allow-Origin header.
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            // Create Claims Identity and add the Claims.
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("username", context.ClientId));
            //identity.AddClaim(new Claim("role", "user"));

            // Set soem Authentication Properties
            var props = new AuthenticationProperties(new Dictionary<string, string>()
            {
                {
                    "as:client_id", context.ClientId ?? string.Empty
                }
            });

            // Create new Authentication Ticket
            var ticket = new AuthenticationTicket(identity, props);

            // Validated the ticket.
            context.Validated(ticket);
            return base.GrantClientCredentials(context);
        }

        // Methods calls at the end before creating the access token to add some extra properties to Access Token Json.
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            //foreach (var property in context.Properties.Dictionary)
            //{
            //    context.AdditionalResponseParameters.Add(property.Key,property.Value);
            //}
            return base.TokenEndpoint(context);
        }

        // Called when grant_type is refresh_token and after validating ValidateClientAuthentication method.
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            // Check if client id for token is same as the one which is passed by the user in request.
            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            //newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);

            context.Validated(newTicket);

            return base.GrantRefreshToken(context);
        }

        //public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        //{
        //    var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin") ?? "*";

        //    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new [] {allowedOrigin});

        //    using (var authRepository = new AuthRepository())
        //    {
        //        var user = new User
        //        {
        //            Username = context.UserName,
        //            Password = context.Password
        //        };

        //        var result = authRepository.FindUser(user);

        //        if (result == null)
        //        {
        //            context.SetError("invalid_grant","The username and password is incorrect");
        //            return Task.FromResult<object>(null);
        //        }

        //        var identity = new ClaimsIdentity(context.Options.AuthenticationType);
        //        identity.AddClaim(new Claim("username",context.UserName));
        //        identity.AddClaim(new Claim("role","user"));

        //        context.Validated(identity);
        //    }

        //    return base.GrantResourceOwnerCredentials(context);
        //}
    }
}