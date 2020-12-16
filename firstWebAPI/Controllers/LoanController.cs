using GivingActuallyAPI.Models;
using GivingActuallyAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static GivingActuallyAPI.Models.Helper;

namespace firstWebAPI.Controllers
{
    public class LoanController : ApiController
    {
        LoanService IService = new LoanService();
        //public async Task<HttpResponseMessage> Get(string Category = "All", int page = 1, int page_size = 12, string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0", int Distance = 0)
        //{
        //    try
        //    {
        //        int CategoryId = -1;
        //        Category = string.IsNullOrEmpty(Category) ? "All" : Category;
        //        if (Category == "All")
        //        {
        //            CategoryId = -1;
        //        }
        //        else
        //        {
        //            StoryCategory cate = (StoryCategory)Enum.Parse(typeof(StoryCategory), Category);
        //            int CateId = (int)cate;
        //            CategoryId = CateId;
        //        }
        //        var res = await IService.GetCampaigns_sp(CategoryId, page, page_size, SortBy, order, Lat, Lon, Distance);

        //        return Request.CreateResponse(HttpStatusCode.OK, res);
        //    }
        //    catch (Exception ex)
        //    {
        //        ResponseObject response = new ResponseObject();
        //        response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
        //        response.ResponseMsg = "Could not get the details of campaigns";
        //        response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
        //    }
        //}

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Loan/Phase1")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> Phase1(int campaignId, [FromBody]LoanViewModel Model)
        {
            try
            {
                Model.Id = campaignId;
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString();

                var userDisName = claims.Where(a => a.type == "DisplayName").Select(a => a.value).SingleOrDefault().ToString();
                Model.UserId = Convert.ToInt32(userId);
                Model.UserName = userName;

                Model.UserDisplayName = userDisName;
                int CampaignId = await IService.CreateLoanPhase1(Model);
                ResponseObject response = new ResponseObject();
                response.campaignId = CampaignId.ToString();
                response.ResponseMsg = "Campaign Updated- Phase 1 successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Campaign Phase 1 Updation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }





        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Loan/Phase2")]
        public async Task<HttpResponseMessage> AddLoanPhase2(int campaignId, string placeName, string Latitude, string Longitude)
        {
            try
            {
                var httpContext = HttpContext.Current;
                List<HttpPostedFile> Files = new List<HttpPostedFile>();
                CampaignPhase2Full addModel = new CampaignPhase2Full();

                addModel.campaignId = campaignId;
                addModel.placeName = placeName;
                addModel.Latitude = Latitude;
                addModel.longitude = Longitude;
                //addModel.UserName = Model.UserName;
                if (httpContext.Request.Files.Count > 0)
                {
                    HttpPostedFile httpPostedFile = httpContext.Request.Files[0];
                    if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
                    {
                        addModel.DisplayPicFile = httpPostedFile;
                        addModel.BDisplayPicName = System.IO.Path.GetFileName(httpPostedFile.FileName);
                        addModel.BDPContentType = httpPostedFile.ContentType;
                    }
                }
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                // var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;
                // addModel.UserId = Convert.ToInt32(userId);
                addModel.UserName = userName;

                bool res = await IService.CreateLoanPhase2(addModel);
                ResponseObject response = new ResponseObject();
                response.campaignId = campaignId.ToString();
                if (res)
                {
                    response.ResponseMsg = "Campaign created- Phase 2 successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Campaign created- Phase 2 ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Campaign Phase 2 creation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Loan/Phase3")]
        public async Task<HttpResponseMessage> AddCampaignPhase3(CampaignPhase3Desc model)
        {
            try
            {
                var httpContext = HttpContext.Current;
                CampaignPhase3Desc addModel = new CampaignPhase3Desc();
                addModel.campaignId = model.campaignId;
                addModel.StoryDescription = model.StoryDescription;

                bool res = await IService.CreateLoanPhase3Desc(addModel);
                ResponseObject response = new ResponseObject();
                response.campaignId = model.campaignId.ToString();
                if (res)
                {
                    response.ResponseMsg = "Campaign created- Phase 3 description successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Campaign created- Phase 3 description ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Campaign Phase 3 description creation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Loan/Phase3Image")]
        public async Task<HttpResponseMessage> AddCampaignPhase3Image(int campaignId)
        {
            try
            {
                var httpContext = HttpContext.Current;
                CampaignPhase3Image addModel = new CampaignPhase3Image();

                // addModel.UserName = Model.UserName;
                List<Attachment> Attach = new List<Attachment>();
                if (httpContext.Request.Files.Count > 0)
                {
                    foreach (var ffile in httpContext.Request.Files)
                    {
                        int i = 0;
                        Attachment att = new Attachment();
                        HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                        if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
                        {
                            att.File = httpPostedFile;
                            att.FileName = System.IO.Path.GetFileName(httpPostedFile.FileName);
                            att.ContentType = httpPostedFile.ContentType;
                        }
                        Attach.Add(att);
                        i++;
                    }
                }
                addModel.campaignId = campaignId;
                addModel.Attachments = new List<Attachment>();
                addModel.Attachments.AddRange(Attach);
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                // var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;
                // addModel.UserId = Convert.ToInt32(userId);
                addModel.UserName = userName;
                ResponseObject response = new ResponseObject();
                response.campaignId = campaignId.ToString();
                bool res = await IService.CreateLoanPhase3Image(addModel);
                if (res)
                {
                    response.ResponseMsg = "Campaign created- Phase 3 Images successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Campaign created- Phase 3 Images ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Campaign Phase 3 Images creation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Loan/CampaignBank")]
        public async Task<HttpResponseMessage> AddCampaignBank(CampaignBankViewModel model)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString();
                CampaignBankVwModel AddModel = new CampaignBankVwModel();
                AddModel.AccountNumber = model.AccountNumber;
                AddModel.BankBranch = model.BankBranch;
                AddModel.BankName = model.BankName;
                AddModel.BenName = model.BenName;
                AddModel.CampaignId = model.CampaignId;
                AddModel.IFSC = model.IFSC;
                AddModel.CreatedBy = userName;
                AddModel.CreatedOn = DateTime.UtcNow;


                int benId = await IService.createLoanBank(AddModel);
                ResponseObject response = new ResponseObject();
                response.campaignId = model.CampaignId.ToString();
                if (benId > 0)
                {
                    response.ResponseMsg = "Campaign Bank details added successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Adding Campaign Bank details ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Adding Campaign Bank details ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }



       



        //[System.Web.Http.Route("api/Campaign/GetAroundYouCampaigns")]
        //[System.Web.Http.HttpGet]
        //public HttpResponseMessage GetAroundYouCampaigns(BaseViewModel Base)
        //{
        //    try
        //    {
        //        string Category = "All"; int page = 1; int page_size = 12;
        //        int CategoryId = -1;
        //        Category = string.IsNullOrEmpty(Category) ? "All" : Category;
        //        if (Category == "All")
        //        {
        //            CategoryId = -1;
        //            Base.Category = "-1";
        //        }
        //        else
        //        {
        //            StoryCategory cate = (StoryCategory)Enum.Parse(typeof(StoryCategory), Category);
        //            int CateId = (int)cate;
        //            CategoryId = CateId;
        //            Base.Category = CateId.ToString();
        //        }
        //        var res = IService.GetCampaignbyLoc(Base);

        //        return Request.CreateResponse(HttpStatusCode.OK, res);
        //    }
        //    catch (Exception ex)
        //    {
        //        ResponseObject response = new ResponseObject();
        //        response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
        //        response.ResponseMsg = "Could not get the details of Campaigns";
        //        response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
        //    }
        //}

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