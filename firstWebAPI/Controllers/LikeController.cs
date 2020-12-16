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
    public class LikeController : ApiController
    {
        CampaignService IService = new CampaignService();
        public HttpResponseMessage Get(int Id)
        {
            try
            {
                LikesModel res = IService.GetLikesCount(Id);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
        // POST api/<controller>
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        public HttpResponseMessage Post([FromBody]Like model)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                string userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                model.LikebyUserId = string.IsNullOrEmpty(userId) ? 0 : Convert.ToInt32(userId);
                LikesModel returnmodel = IService.PostLike(model);
                return Request.CreateResponse(HttpStatusCode.OK, returnmodel);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet]
        [System.Web.Http.Route("api/Like/Details/{Id}")]
        public HttpResponseMessage Details(int Id)
        {
            try
            {
                LikesModel res = IService.GetLikesdetails(Id);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
        public void Delete(int id)
        {
        }
    }
}