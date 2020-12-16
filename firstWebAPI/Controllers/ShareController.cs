using GivingActuallyAPI.Models;
using GivingActuallyAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;

namespace firstWebAPI.Controllers
{
    public class ShareController : ApiController
    {
        // GET api/<controller>
        CampaignService IService = new CampaignService();
        public HttpResponseMessage Get(int Id)
        {
            try
            {
                SharesModel res = IService.GetSharecount(Id);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not fetch the details of Share";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [System.Web.Http.Route("api/Share/Details/{Id}")]
        [HttpGet]
        public HttpResponseMessage Details(int Id)
        {
            try
            {
                SharesModel res = IService.GetSharesdetails(Id);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not fetch the details of share";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        // POST api/<controller>

        public HttpResponseMessage Post([FromBody]shareModel model)
        {
            try
            {
                int UserId = 0;
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                if (claims != null)
                {
                    var UserIdStr = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault();
                    var userIdv = UserIdStr != null ? UserIdStr.ToString() : "0";
                    UserId = Convert.ToInt32(userIdv);
                }
                model.UserId = UserId;
                SharesModel returnmodel = IService.PostShares(model);
                return Request.CreateResponse(HttpStatusCode.OK, returnmodel);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not Post the share";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


    }
}
