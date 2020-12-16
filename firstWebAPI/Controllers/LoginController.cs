using GivingActuallyAPI.Models;
using GivingActuallyAPI.Models.HelperModels;
using GivingActuallyAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static GivingActuallyAPI.Models.Helper;

namespace GivingActuallyAPI.Controllers
{
    public class LoginController : ApiController
    {
        CommonService IService = new CommonService();
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Login([System.Web.Http.FromBody] LoginModel model)
        {
            try
            {
                if (model != null)
                {
                    var res = IService.ValidateUser(model);
                    return Request.CreateResponse(HttpStatusCode.OK, res);
                }
                return Request.CreateResponse(HttpStatusCode.Conflict, "");



            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
        // GET api/<controller>


    }
}