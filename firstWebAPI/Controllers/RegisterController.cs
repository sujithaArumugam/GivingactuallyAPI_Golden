using GivingActuallyAPI.Models;
using GivingActuallyAPI.Services;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace GivingActuallyAPI.Controllers
{
    public class RegisterController : ApiController
    {
        CommonService IService = new CommonService();
        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]InputRegisterModel Model)
        {
            try
            {
                InputRegisterModel m = new InputRegisterModel();
                var res = IService.RegisterUser(Model);
                ResponseObject response = new ResponseObject();

                if (res == "Registered Successfully. Please try logging in!")
                {
                    res = "OTP Is Sent Successfully";

                    response.userId = Model.UserName.ToString();
                    response.ResponseMsg = res;
                    return Request.CreateResponse(HttpStatusCode.OK, response);

                }
                else
                {
                    response.userId = Model.UserName.ToString();
                    response.ResponseMsg = res;
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Registeration with OTP  is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        //[AllowAnonymous]
        //[HttpPost]
        //public HttpResponseMessage Postold([FromBody]InputRegisterModel Model)
        //{
        //    try
        //    {
        //        InputRegisterModel m = new InputRegisterModel();
        //        var res = IService.RegisterUser(Model);
        //        ResponseObject response = new ResponseObject();

        //        if (res == "Registered Successfully. Please try logging in!")
        //        {
        //            res = "OTP Is Sent Successfully";

        //            response.userId = Model.UserName.ToString();
        //            response.ResponseMsg = res;
        //            return Request.CreateResponse(HttpStatusCode.OK, response);

        //        }
        //        else
        //        {
        //            response.userId = Model.UserName.ToString();
        //            response.ResponseMsg = res;
        //            return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ResponseObject response = new ResponseObject();
        //        response.ExceptionMsg = ex.InnerException.ToString();
        //        response.ResponseMsg = "Registeration with OTP  is ended with an exception";
        //        response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
        //    }
        //}

        [AllowAnonymous]
        [HttpPost]
        [System.Web.Http.Route("api/Register/CheckOTP")]
        public HttpResponseMessage CheckOTP([FromBody]InputRegisterModel Model)
        {
            try
            {
                InputRegisterModel m = new InputRegisterModel();
                var res = IService.CheckOTP(Model);
                ResponseObject response = new ResponseObject();

                if (res == "Successfully Updated!")
                {

                    response.userId = Model.UserName.ToString();
                    response.ResponseMsg = "Registered Successfully. Please try logging in!";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.userId = Model.UserName.ToString();
                    response.ResponseMsg = res;
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Registeration with OTP  is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [System.Web.Http.Route("api/Register/ResendOTP")]
        public HttpResponseMessage ResendOtp(string UserName)
        {
            try
            {
                var m = IService.GetUserDetailbyName(UserName);
                var res = IService.CreatenSendOTP(m.UserName, m.DisplayName);
                ResponseObject response = new ResponseObject();
                response.userId = UserName.ToString();
                response.ResponseMsg = "OTP Sent Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "resendig with OTP  is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [System.Web.Http.Route("api/Register/ExternalLogin")]
        public JObject GenerateLocalAccessTokenResponse(SocialRegisterModel socialRegisterModel)
        {
            //ToSocialregisterModel
            try
            {
                if (socialRegisterModel != null)
                {
                    var tokenExpiration = TimeSpan.FromSeconds(2);
                    var props = new AuthenticationProperties()
                    {
                        IssuedUtc = DateTime.UtcNow,
                        ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
                    };
                    var user = IService.ToSocialregisterModel(socialRegisterModel);
                    ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
                    identity.AddClaim(new Claim(ClaimTypes.Role, "SuperAdmin"));
                    identity.AddClaim(new Claim(ClaimTypes.Name, socialRegisterModel.UserName));
                    identity.AddClaim(new Claim("UserId", user.Id.ToString()));
                    identity.AddClaim(new Claim("DisplayName", user.DisplayName));
                    identity.AddClaim(new Claim("isAdmin", user.IsAdmin.ToString()));
                    identity.AddClaim(new Claim("LastLoginDate", user.LastLoginTime.ToString()));

                    AuthenticationProperties properties = AuthorizationServerProvider.CreateProperties(
                        socialRegisterModel.UserName, user.Id.ToString(), user.DisplayName,user.IsNGO.ToString(),user.CanEndorse.ToString(),
                    user.IsAdmin.ToString(), user.LastLoginTime.ToString());

                    //        //AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);

                    // var ticket = new AuthenticationTicket(identity, props);

                    var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

                    JObject tokenResponse = new JObject(
                                                new JProperty("userName", socialRegisterModel.UserName),
                                                 new JProperty("UserId", user.Id.ToString()),
                                                 new JProperty("DisplayName", user.DisplayName),
                                                  new JProperty("IsNGO", user.IsNGO.ToString()),
                                                   new JProperty("canEndorse", user.CanEndorse.ToString()),
                                                new JProperty("access_token", accessToken),
                                                new JProperty("token_type", "bearer"),
                                                new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                                new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                                new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString()),
                                                 new JProperty(".isAdmin", user.IsAdmin.ToString()),
                                                new JProperty(".LastLoginDate", user.LastLoginTime.ToString())
                );

                    return tokenResponse;
                }
                else
                {
                    JObject tokenResponse = new JObject(new JProperty("response", "BadRequest"));
                    return tokenResponse;
                }
            }
            catch (Exception ex)
            { throw ex; }

        }
    }
}