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
    public class CommentsController : ApiController
    {
        // GET api/<controller>
        CampaignService IService = new CampaignService();
        public HttpResponseMessage Get(int Id)
        {
            try
            {
                commentsModel res = IService.GetComments(Id);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException!=null ? ex.InnerException.ToString():ex.Message;
                response.ResponseMsg = "Could not fetch the details of comments";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        // POST api/<controller>
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        public HttpResponseMessage Post([FromBody]commentModel model)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                string userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                model.userId = string.IsNullOrEmpty(userId) ? 0 : Convert.ToInt32(userId);
                CommentsVM returnmodel = IService.PostComments(model);
                return Request.CreateResponse(HttpStatusCode.OK, returnmodel);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not post the comments";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [HttpGet]
        [System.Web.Http.Route("api/Comments/count/{Id}")]
        public HttpResponseMessage count(int Id)
        {
            try
            {
                commentsModel res = IService.GetMiniComments(Id);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not fetch the details of comments";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

    }
}