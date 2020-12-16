using GivingActuallyAPI.Models;
using GivingActuallyAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace firstWebAPI.Controllers
{
    public class EndorsementsController : ApiController
    {
        // GET api/<controller>
        CampaignService IService = new CampaignService();
        public HttpResponseMessage Get(int Id)
        {
            try
            {
                ENdorsementList res = IService.GetEndorsements(Id);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not fetch the details of endorsements";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        // POST api/<controller>
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        public HttpResponseMessage Post([FromBody]EndorseModel model)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                string userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                model.NGOId = string.IsNullOrEmpty(userId) ? 0 : Convert.ToInt32(userId);
                if (IService.ISUserNGO(model.NGOId))
                {
                    Endorsement returnmodel = IService.PostEndorsements(model);
                    return Request.CreateResponse(HttpStatusCode.OK, returnmodel);
                }
                else
                {
                    ResponseObject response = new ResponseObject();
                    response.ExceptionMsg = "You Are not Authorized NGO";
                    response.ResponseMsg = "You Are not Authorized NGO";
                    response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                    return Request.CreateResponse(HttpStatusCode.Conflict, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not Post the details of Endorse";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [HttpGet]
        [System.Web.Http.Route("api/Endorsements/count/{Id}")]
        public HttpResponseMessage count(int Id)
        {
            try
            {
                ENdorsementList res = IService.GetEndorsementsCount(Id);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of Endorsements";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
    }
}