using GivingActuallyAPI.Models;
using GivingActuallyAPI.Services;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace GivingActuallyAPI
{

    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        CommonService IService = new CommonService();
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }



        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            using (UserAuthentication OBJ = new UserAuthentication())
            {
                UserModel userResult = new UserModel();
                var user = OBJ.ValidateUser(context.UserName, context.Password);
                if (user == "false")
                {
                    ///context.SetError("invalid_grant", "Username or password is incorrect");
                    ///context.SetError("invalid_grant", "Username or password is incorrect");
                    ResponseObject obj = new ResponseObject();
                    obj.ResponseMsg = "Username or password is incorrect";
                    obj.userId = context.UserName;
                    obj.ExceptionMsg = "Username or password is incorrect";
                    context.SetCustomError(obj);
                    context.Rejected();
                    return;
                }
                else
                {
                    userResult = IService.GetUserDetailbyName(context.UserName);
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Role, "SuperAdmin"));
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                identity.AddClaim(new Claim("UserId", userResult.Id.ToString()));
                identity.AddClaim(new Claim("isNGO", userResult.IsNGO.ToString()));
                identity.AddClaim(new Claim("canEndorse", userResult.CanEndorse.ToString()));
                identity.AddClaim(new Claim("DisplayName", userResult.DisplayName));
                identity.AddClaim(new Claim("isAdmin", userResult.IsAdmin.ToString()));
                identity.AddClaim(new Claim("LastLoginDate", userResult.LastLoginTime.ToString()));

                AuthenticationProperties properties = CreateProperties(context.UserName, userResult.Id.ToString(),
                    userResult.DisplayName.ToString(), userResult.IsNGO.ToString(), userResult.CanEndorse.ToString(),
                    userResult.IsAdmin.ToString(), userResult.LastLoginTime.ToString());

                AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);

                context.Validated(ticket);
                // context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            }
        }

        public static AuthenticationProperties CreateProperties(string userName, string UserId
            , string DisplayName, string IsNGO, string canEndorse, string IsAdmin, string LastLoginDate)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
        {
            { "userName", userName },
                {"UserId",UserId },
            {"DisplayName",DisplayName},
                {"IsNGO", IsNGO},
                {"canEndorse",canEndorse },
                {"IsAdmin", IsAdmin},
                {"LastLoginDate",LastLoginDate }
        };
            return new AuthenticationProperties(data);
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);

        }



    }

    public static class ContextHelper
    {
        public static void SetCustomError(this OAuthGrantResourceOwnerCredentialsContext context, ResponseObject errorMessage)
        {
            var json = errorMessage.ToJsonString();



            context.Response.ContentType = "application/json";
            context.Response.Write(json);

        }
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}