using firstWebAPI.Services;
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
    public class DonationController : ApiController
    {
        CampaignService IService = new CampaignService();
        DonationService serve = new DonationService();
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public HttpResponseMessage Get(int Id)
        {
            try
            {

                DonationsModel res = IService.GetDonorsList(Id);
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
        public async Task<HttpResponseMessage> Post([FromBody]CampaignMiniDonation viewModel)
        {
            try
            {
                MiniDonationModel model = await serve.AddCampaignDonation(viewModel);
                ResponseObject response = new ResponseObject();
                response.campaignId = viewModel.CampaignId.ToString();
                response.ResponseMsg = "Campaign Donation with order id created successfully";
                
                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.ToString();
                response.ResponseMsg = "Campaign Donation with order id creation - is not successful";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]

        [System.Web.Http.Route("api/Donation/Charge")]
        public HttpResponseMessage Charge([FromBody] RazorPayResponseclass model)
        {
            try
            {
                var CampaignId = serve.UpdateCampaignDonationSuccess(model.razorpay_order_id, model.razorpay_payment_id, model.razorpay_signature);
                ResponseObject response = new ResponseObject();
                response.campaignId = CampaignId.ToString();
                var res = serve.GetDonorscount(CampaignId);

                response.ResponseMsg = "Campaign Donation with order  updated successfully";
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.ToString();
                response.ResponseMsg = "Campaign Donation with order update - is not successful";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [System.Web.Http.Route("api/Donation/Capture")]
        public HttpResponseMessage Capture(string razorpay_payment_id,decimal Amount,string Type)
        {
            try
            {
                var CampaignId = serve.CapturePayment(razorpay_payment_id,Amount,Type);
                ResponseObject response = new ResponseObject();
                response.campaignId = CampaignId.ToString();
                var res = serve.GetDonorscount(CampaignId);

                response.ResponseMsg = "Campaign Donation with order  updated successfully";
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.ToString();
                response.ResponseMsg = "Campaign Donation with order update - is not successful";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
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