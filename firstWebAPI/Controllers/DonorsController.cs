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
    public class DonorsController : ApiController
    {
        CampaignService IService = new CampaignService();
        public HttpResponseMessage Get(int Id)
        {
            try
            {

                DonationsModel  res = IService.GetDonorsList(Id);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not fetch the details of Donors";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        // POST api/<controller>
        //public HttpResponseMessage Post([FromBody]commentModel model)
        //{
        //    try
        //    {
        //        // commentsModel returnmodel = IService.PostComments(model);
        //        return Request.CreateResponse(HttpStatusCode.OK, returnmodel);
        //    }
        //    catch (Exception)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        //}
        //[HttpGet]
        //[System.Web.Http.Route("api/Comments/count")]
        //public HttpResponseMessage count(int Id)
        //{
        //    try
        //    {
        //        commentsModel res = IService.GetMiniComments(Id);
        //        return Request.CreateResponse(HttpStatusCode.OK, res);
        //    }
        //    catch (Exception)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        }
    }

