using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.Infrastructure;
using WebApiTokenAuth.DAL;
using WebApiTokenAuth.Models;

namespace WebApiTokenAuth.Providers
{
    public class AppRefreshTokenProvider : AuthenticationTokenProvider
    {
        //public Task CreateAsync(AuthenticationTokenCreateContext context)
        //{
        //    Create(context);
        //    return context;
        //}
        public override void Create(AuthenticationTokenCreateContext context)
        {
            var clientId = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("N");

            using (var repository = new AuthRepository())
            {
                var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

                var token = new RefreshToken()
                {
                    Token = refreshTokenId,
                    ClientId = clientId,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiryUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
                };

                context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                context.Ticket.Properties.ExpiresUtc = token.ExpiryUtc;

                token.ProtectedTicket = context.SerializeTicket();

                var result = repository.AddRefreshToken(token);

                if (result)
                {
                    context.SetToken(refreshTokenId);
                }
            }
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            var allowOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new [] {allowOrigin});

            string refreshTokenId = context.Token;

            using (var repository = new AuthRepository())
            {
                var refreshToken = repository.FindRefreshToken(refreshTokenId);

                if (refreshToken != null)
                {
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    var result = repository.RemoveRefreshToken(refreshTokenId);
                }
            }
        }

        //public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        //{
        //    throw new NotImplementedException();
        //}
    }
}