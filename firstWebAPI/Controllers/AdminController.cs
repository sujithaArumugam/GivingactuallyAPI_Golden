using firstWebAPI.Services;
using GivingActuallyAPI.Models;
using GivingActuallyAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static GivingActuallyAPI.Models.Helper;

namespace firstWebAPI.Controllers
{
    public class AdminController : ApiController
    {
        // GET api/<controller>
        CampaignService IService = new CampaignService();
        UserService userservice = new UserService();
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [System.Web.Http.HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [System.Web.Http.Route("api/Admin/GetAdminCampaigns")]
        public async Task<HttpResponseMessage> GetAdminCampaigns(string Category = "All", int page = 1, int page_size = 500, string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0", int Distance = 0)
        {
            try
            {
                int CategoryId = -1;
                Category = string.IsNullOrEmpty(Category) ? "All" : Category;
                if (Category == "All")
                {
                    CategoryId = -1;
                }
                else
                {
                    StoryCategory cate = (StoryCategory)Enum.Parse(typeof(StoryCategory), Category);
                    int CateId = (int)cate;
                    CategoryId = CateId;
                }
                var res = await IService.GetAdminCampaigns_sp(CategoryId, page, page_size, SortBy, order, Lat, Lon, Distance);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [System.Web.Http.HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [System.Web.Http.Route("api/Admin/GetUsers")]
        public HttpResponseMessage GetUsers(string Status = "All", int page = 1, int page_size = 50)
        {
            try
            {
                var result = userservice.GetUserDetails(Status, page, page_size);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Error Getting the user Details";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);

            }
        }


        [System.Web.Http.HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [System.Web.Http.Route("api/Admin/ApproveCampaign")]
        public HttpResponseMessage ApproveCampaign(bool Status = true, int CampaignId=0)
        {
            try
            {
                var result = IService.ApproveCampaign(CampaignId, Status);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Campaign Approved Successfully");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Campaign Approval is not successful. Please try again later");
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Error Getting the user Details";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);

            }
        }

        [System.Web.Http.HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [System.Web.Http.Route("api/Admin/GetNewCampaigns")]
        public async Task<HttpResponseMessage> GetNewCampaigns( int page = 1, int page_size = 500)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                var LastLogin = claims.Where(a => a.type == "LastLoginDate").Select(a => a.value).SingleOrDefault().ToString();
                DateTime LastLoginDate = Convert.ToDateTime(LastLogin);
                var res = await IService.GetAdminnewCampaigns(LastLoginDate, page, page_size);

              
               // var nextsetres = res.CampaignViewModelList.Except(StoryList.NewStoriesViewModel.CampaignViewModelList).ToList();


               // StoryList.PendingStoriesViewModel.CampaignViewModelList = nextsetres.Where(a => a.IsApprovedbyAdmin != true).ToList();
               // var nexttsetres = nextsetres.Except(StoryList.PendingStoriesViewModel.CampaignViewModelList).ToList();
               // StoryList.FraudStoriesViewModel.CampaignViewModelList = nexttsetres.Where(a => a.IsApprovedbyAdmin != true).ToList();
                


                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [System.Web.Http.HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [System.Web.Http.Route("api/Admin/GetUnApprovalCampaigns")]
        public async Task<HttpResponseMessage> GetUnApprovalCampaigns(int page = 1, int page_size = 500)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;

                var res = await IService.GetAdminUnApprovalCampaigns( page, page_size);


                // var nextsetres = res.CampaignViewModelList.Except(StoryList.NewStoriesViewModel.CampaignViewModelList).ToList();


                // StoryList.PendingStoriesViewModel.CampaignViewModelList = nextsetres.Where(a => a.IsApprovedbyAdmin != true).ToList();
                // var nexttsetres = nextsetres.Except(StoryList.PendingStoriesViewModel.CampaignViewModelList).ToList();
                // StoryList.FraudStoriesViewModel.CampaignViewModelList = nexttsetres.Where(a => a.IsApprovedbyAdmin != true).ToList();



                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [System.Web.Http.HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [System.Web.Http.Route("api/Admin/GetFraudulenceCampaigns")]
        public async Task<HttpResponseMessage> GetFraudulenceCampaigns(int page = 1, int page_size = 500)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;

                var res = await IService.GetFraudulenceCampaigns(page, page_size);


                // var nextsetres = res.CampaignViewModelList.Except(StoryList.NewStoriesViewModel.CampaignViewModelList).ToList();


                // StoryList.PendingStoriesViewModel.CampaignViewModelList = nextsetres.Where(a => a.IsApprovedbyAdmin != true).ToList();
                // var nexttsetres = nextsetres.Except(StoryList.PendingStoriesViewModel.CampaignViewModelList).ToList();
                // StoryList.FraudStoriesViewModel.CampaignViewModelList = nexttsetres.Where(a => a.IsApprovedbyAdmin != true).ToList();



                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Admin/ApproveWithdraw")]
        public async Task<HttpResponseMessage> ApproveWithdraw(ApproveWithdrawModel model)
        {
            try
            {
                bool res = await IService.ApproveWithdrawRequest(model);
                ResponseObject response = new ResponseObject();
                response.campaignId = model.CampaignId.ToString();
                if (res)
                {
                    response.ResponseMsg = "Campaign Withdrawal request Approved successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Approving Campaign Withdrawal request ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Approving Campaign Withdrawal request ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Admin/RejectWithdraw")]
        public async Task<HttpResponseMessage> RejectWithdraw(ApproveWithdrawModel model)
        {
            try
            {
                bool res = await IService.RejectWithdrawRequest(model);
                ResponseObject response = new ResponseObject();
                response.campaignId = model.CampaignId.ToString();
                if (res)
                {
                    response.ResponseMsg = "Campaign Withdrawal request Approved successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Approving Campaign Withdrawal request ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Approving Campaign Withdrawal request ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Admin/ApproveBank")]
        public async Task<HttpResponseMessage> ApproveBank(ApproveBankVwModel model)
        {
            try
            {
                bool res = await IService.ApproveCampaignBank(model);
                ResponseObject response = new ResponseObject();
                response.campaignId = model.CampaignId.ToString();
                if (res)
                {
                    response.ResponseMsg = "Campaign Bank approved successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Campaign Bank approval ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Campaign Bank approval ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Admin/RejectBank")]
        public async Task<HttpResponseMessage> RejectBank(ApproveBankVwModel model)
        {
            try
            {
                bool res = await IService.RejectCampaignBank(model);
                ResponseObject response = new ResponseObject();
                response.campaignId = model.CampaignId.ToString();
                if (res)
                {
                    response.ResponseMsg = "Campaign Bank Rejected successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Campaign Bank rejection ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Campaign Bank rejection ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }



        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Admin/BankDetailForApproval")]
        public async Task<HttpResponseMessage> GetBankDetailsforApproval()
        {
            try
            {
                var List = await IService.GetCampaignBankforApproval();
                return Request.CreateResponse(HttpStatusCode.OK, List);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "error getting the Campaign Bank details ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Admin/GetCampaignBankList")]
        public async Task<HttpResponseMessage> GetBankDetailsForCampaign(int CampaignId)
        {
            try
            {
                var List = await IService.GetCampaignBankDetail(CampaignId);
                return Request.CreateResponse(HttpStatusCode.OK, List);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "error getting the Campaign Bank details ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Admin/GetApprovedCampaignBank")]
        public async Task<HttpResponseMessage> GetApprovedBankDetailsForCampaign(int CampaignId)
        {
            try
            {
                var List = await IService.GetCampaignApprovedBankDetail(CampaignId);
                return Request.CreateResponse(HttpStatusCode.OK, List);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "error getting the Campaign Bank details ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Admin/GetBankDetailById")]
        public async Task<HttpResponseMessage> GetBankDetailsById(int BankId)
        {
            try
            {
                var List = await IService.GetCampaignBankDetailById(BankId);
                return Request.CreateResponse(HttpStatusCode.OK, List);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "error getting the Campaign Bank details ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Admin/WithdrawalListForApproval")]
        public async Task<HttpResponseMessage> GetWithdrawalListForApproval()
        {
            try
            {
                var List = await IService.GetwithdrawalRequestForApproval();
                return Request.CreateResponse(HttpStatusCode.OK, List);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "error getting the Campaign Bank details ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Admin/GetWithdrawalListForCampaign")]
        public async Task<HttpResponseMessage> GetWithdrawalListForCampaign(int CampaignId)
        {
            try
            {
                var List = await IService.GetCampaignwithdrawalRequest(CampaignId);
                return Request.CreateResponse(HttpStatusCode.OK, List);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "error getting the Campaign Bank details ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Admin/ApprovedWthdwListForCamp")]
        public async Task<HttpResponseMessage> GetCampaignApprovedwithdrawal(int CampaignId)
        {
            try
            {
                var List = await IService.GetCampaignApprovedwithdrawal(CampaignId);
                return Request.CreateResponse(HttpStatusCode.OK, List);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "error getting the Campaign Bank details ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Admin/GetWithdrawalDetailById")]
        public async Task<HttpResponseMessage> GetWithdrawalDetailById(int WithdrawalId)
        {
            try
            {
                var List = await IService.GetwithdrawalRequestById(WithdrawalId);
                return Request.CreateResponse(HttpStatusCode.OK, List);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "error getting the Campaign Bank details ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
    }
}