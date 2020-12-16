using GivingActuallyAPI.Models;
using GivingActuallyAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace firstWebAPI.Controllers
{
    public class ScheduleController : ApiController
    {
        // GET api/<controller>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Schedule/Deactivate")]
        public HttpResponseMessage Deactive()
        {
            try
            {
                CampaignService Iservice = new CampaignService();
                bool res = Iservice.DeActivateCampaigns();
                return Request.CreateResponse(HttpStatusCode.OK, res.ToString());
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not deactivate the details of campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Schedule/FirstNotifyDeAct")]
        public async Task<HttpResponseMessage> NotifyDeactive()
        {
            try
            {
                CampaignService Iservice = new CampaignService();
                bool res =await Iservice.FirstNOtifyDeActCampaigns();
                return Request.CreateResponse(HttpStatusCode.OK, res.ToString());
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not deactivate the details of campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Schedule/SecondNotifyDeAct")]
        public async Task<HttpResponseMessage> SecondNotifyDeactive()
        {
            try
            {
                CampaignService Iservice = new CampaignService();
                bool res = await Iservice.SecondNOtifyDeActCampaigns();
                return Request.CreateResponse(HttpStatusCode.OK, res.ToString());
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not deactivate the details of campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}