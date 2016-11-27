using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApiTokenAuth.Providers;

[assembly: OwinStartup(typeof(WebApiTokenAuth.Startup))]

namespace WebApiTokenAuth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            ConfigureOAuth(app);

            var httpConfig = new HttpConfiguration();

            WebApiConfig.Register(httpConfig);

            // Wire-up ASP.NET WebApi to Owin pipeline
            app.UseWebApi(httpConfig);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            var oAuthAuthorizationServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new AppOAuthAuthorizationServerProvider(),
                RefreshTokenProvider = new AppRefreshTokenProvider()
            };

            app.UseOAuthAuthorizationServer(oAuthAuthorizationServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
