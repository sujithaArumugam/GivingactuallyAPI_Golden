using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;

[assembly: OwinStartup(typeof(GivingActuallyAPI.Startup))]

namespace GivingActuallyAPI
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
      //  public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                //The Path For generating the Toekn
                TokenEndpointPath = new PathString("/token"),
                //Setting the Token Expired Time (24 hours)
               // AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(2),
                //MyAuthorizationServerProvider class will validate the user credentials
                Provider = new AuthorizationServerProvider()  
            };
            
            //Token Generations  
            app.UseOAuthAuthorizationServer(options);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }
    }
}