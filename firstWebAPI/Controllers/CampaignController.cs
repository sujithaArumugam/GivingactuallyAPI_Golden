using GivingActuallyAPI.Services;
using GivingActuallyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using static GivingActuallyAPI.Models.Helper;
using System.Web;
using System.Threading.Tasks;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using System.Security.Claims;
using System.Web.Http.Cors;
using firstWebAPI;


namespace GivingActuallyAPI.Controllers
{
    [DeflateCompression]
    public class CampaignController : ApiController
    {
        CampaignService IService = new CampaignService();
        CommonService IcmnService = new CommonService();
        // GET api/<controller>

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Campaign/Test2")]
        public async Task<HttpResponseMessage> Test2(string Category = "All", int page = 1, int page_size = 12, string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0")
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
                var res = await IService.GetCampaignsAsync(CategoryId, page, page_size, SortBy, order, Lat, Lon);

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
        [System.Web.Http.Route("api/Campaign/Test1")]
        public async Task<HttpResponseMessage> Test1(string Category = "All", int page = 1, int page_size = 12, string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0")
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
                var res = await IService.GetCampaigns1(CategoryId, page, page_size, SortBy, order, Lat, Lon);

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

        public async Task<HttpResponseMessage> Get(string Category = "All", int page = 1, int page_size = 12, string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0", int Distance = 0)
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
                var res = await IService.GetCampaigns_sp(CategoryId, page, page_size, SortBy, order, Lat, Lon, Distance);

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
        [System.Web.Http.Route("api/Campaign/summarytest")]
        public async Task<HttpResponseMessage> summarytest(string Category = "All", int page = 1, int page_size = 12, string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0", int Distance = 0)
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
                var res = await IService.GetCampaigns2(CategoryId, page, page_size, SortBy, order, Lat, Lon, Distance);

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
        [System.Web.Http.Route("api/Campaign/Test3")]
        public async Task<HttpResponseMessage> Test3(string Category = "All", int page = 1, int page_size = 12, string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0")
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
                var res = await IService.GetCampaigns3(CategoryId, page, page_size, SortBy, order, Lat, Lon);

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
        [System.Web.Http.Route("api/Campaign/getold")]
        public async Task<HttpResponseMessage> Get_old(int id)
        {
            try
            {
                int UserId = 0;
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                if (claims != null)
                {
                    var UserIdStr = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault();
                    var userIdv = UserIdStr != null ? UserIdStr.ToString() : "0"; ;
                    // var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;
                    //  var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;

                    UserId = Convert.ToInt32(userIdv);
                }

                CampaignDetailModel Model = new CampaignDetailModel();
                if (id > 0)
                {
                    Model = await IService.GetCamapignDetailold(id, UserId);
                }

                return Request.CreateResponse(HttpStatusCode.OK, Model);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of Campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

       


        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> Get(int id)
        {
            try
            {
                int UserId = 0;
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                if (claims != null)
                {
                    var UserIdStr = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault();
                    var userIdv = UserIdStr != null ? UserIdStr.ToString() : "0"; ;
                    // var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;
                    //  var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;

                    UserId = Convert.ToInt32(userIdv);
                }

                CampaignDetailModel Model = new CampaignDetailModel();
                if (id > 0)
                {
                    Model = await IService.GetCamapignDetail(id, UserId);
                }

                return Request.CreateResponse(HttpStatusCode.OK, Model);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of Campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [System.Web.Http.Route("api/Campaign/TopCampaigns")]
        [System.Web.Http.HttpGet]
        [CacheFilter(1000, 1000, false)]
        public async Task<HttpResponseMessage> GetTopCampaigns(string Category = "All", int page = 1, int page_size = 12)
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
                var res = await IService.GetTopCampaigns(CategoryId, page, page_size);

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


        [System.Web.Http.Route("api/Campaign/CalculateTopCampaigns")]
        [System.Web.Http.HttpGet]
        [CacheFilter(1000, 1000, false)]
        public HttpResponseMessage CalculateTopCampaigns(string Category = "All", int page = 1, int page_size = 12)
        {
            try
            {

                var res =  IService.UpdateTopCampaigns();

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


        [System.Web.Http.Route("api/Campaign/CampaignSummary")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetCampaignSummary(int id)
        {
            try
            {

                var res = await IService.GetCampaignSummary(id);

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

        [System.Web.Http.Route("api/Campaign/UserCampaigns")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetUserCampaign(int UserId = 0, string Category = "All", int page = 1, int page_size = 12)
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
                //var identity = (ClaimsIdentity)User.Identity;
                //var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                //var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                //var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;
                //int UserId = Convert.ToInt32(userId);
                var res = await IService.GetUserCampaigns(CategoryId, page, page_size, UserId);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of Campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [System.Web.Http.Route("api/Campaign/UserStatistics")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetUserStatistics(int UserId = 0)
        {
            try
            {

                //var identity = (ClaimsIdentity)User.Identity;
                //var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                //var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                //var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); 
                if (UserId > 0)
                {
                    var res = await IService.GetUserStatistics(UserId);
                    return Request.CreateResponse(HttpStatusCode.OK, res);
                }
                else
                {
                    ResponseObject response = new ResponseObject();
                    response.ExceptionMsg = "invalid userId";
                    response.ResponseMsg = "Could not get the details of User Statistics";
                    response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of User Statistics";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Campaign/UserDonations")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetUserDonations()
        {
            try
            {

                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                // var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;
                //  int UserId = Convert.ToInt32(userId);
                var res = await IService.GetUserDonations(userName);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of User Statistics";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [System.Web.Http.Route("api/Campaign/SearchCampaigns_SP")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetSearchCampaigns_SP(string searchtext, int page = 1, int page_size = 36)
        {
            try
            {
                string Category = "All";
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
                var t = await IService.SearchCampaigns(page, page_size, searchtext);
                return Request.CreateResponse(HttpStatusCode.OK, t);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of Campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [System.Web.Http.Route("api/Campaign/SearchCampaigns")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetSearchCampaigns(string searchtext, int page = 1, int page_size = 36)
        {
            try
            {
                string Category = "All";
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
                var t = await IService.SearchCampaigns_sp(page, page_size, searchtext);
                return Request.CreateResponse(HttpStatusCode.OK, t);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of Campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [System.Web.Http.Route("api/Campaign/SearchForCampaigns")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage SearchForCampaigns(string SearchText, int page = 1, int page_size = 36)
        {
            try
            {
                string Category = "All"; int CategoryId = -1;
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

                var res = IService.GetSearchUserCampaigns(CategoryId, page, page_size, SearchText);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of Campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }




        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Campaign/Phase1")]
        public async Task<HttpResponseMessage> Post([FromBody]CampaignVwModel1 Model)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString();
                var userDisName = claims.Where(a => a.type == "DisplayName").Select(a => a.value).SingleOrDefault().ToString();
                Model.UserId = Convert.ToInt32(userId);
                Model.UserName = userName;
                Model.CreatedBy = userName;
                Model.UserDisplayName = userDisName;
                int CampaignId = await IService.CreateCampaignPhase1(Model);
                ResponseObject response = new ResponseObject();
                response.campaignId = CampaignId.ToString();
                response.ResponseMsg = "Campaign created- Phase 1 successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Campaign Phase 1 creation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Campaign/Phase2old")]
        public HttpResponseMessage AddCampaignPhase2old([FromBody]CampaignPhase2Desc Model)
        {
            try
            {


                var httpContext = HttpContext.Current;
                List<HttpPostedFile> Files = new List<HttpPostedFile>();
                CampaignPhase2Full addModel = new CampaignPhase2Full();
                addModel.Id = Model.Id;
                addModel.campaignId = Model.campaignId;
                addModel.placeName = Model.placeName;
                addModel.Latitude = Model.Latitude;
                addModel.longitude = Model.longitude;
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
                //bool res = IService.CreateCampaignPhase2(addModel);
                if (true)
                    return Request.CreateResponse(HttpStatusCode.OK, "successfully updated");

            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not Post the details of campaign";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Campaign/Phase2")]
        public async Task<HttpResponseMessage> AddCampaignPhase2(int campaignId, string placeName, string Latitude, string Longitude)
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

                bool res = await IService.CreateCampaignPhase2(addModel);
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
        [System.Web.Http.Route("api/Campaign/Phase3")]
        public async Task<HttpResponseMessage> AddCampaignPhase3(CampaignPhase3Desc model)
        {
            try
            {
                var httpContext = HttpContext.Current;
                CampaignPhase3Desc addModel = new CampaignPhase3Desc();
                addModel.campaignId = model.campaignId;
                addModel.StoryDescription = model.StoryDescription;

                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString();
                var userDisName = claims.Where(a => a.type == "DisplayName").Select(a => a.value).SingleOrDefault().ToString();
                addModel.UserDisplayName = userDisName;
                addModel.UserName = userName;
                bool res = await IService.CreateCampaignPhase3Desc(addModel);
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
        [System.Web.Http.Route("api/Campaign/Phase3Image")]
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
                bool res = await IService.CreateCampaignPhase3Image(addModel);
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
        [System.Web.Http.Route("api/Campaign/CampaignBank")]
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


                int benId = await IService.createCampaignBank(AddModel);
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
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Campaign/CampaignBank")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> AddCampaignBankUpdate(CampaignBankViewModel model)
        {
            try
            {

                CampaignBankVwModel AddModel = new CampaignBankVwModel();
                AddModel.AccountNumber = model.AccountNumber;
                AddModel.BankBranch = model.BankBranch;
                AddModel.BankName = model.BankName;
                AddModel.BenName = model.BenName;
                AddModel.CampaignId = model.CampaignId;
                AddModel.IFSC = model.IFSC;
                AddModel.Id = model.BankId;
                int benId = await IService.createCampaignBank(AddModel);
                ResponseObject response = new ResponseObject();
                response.campaignId = model.CampaignId.ToString();
                if (benId > 0)
                {
                    response.ResponseMsg = "Campaign Bank details Updated successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Updating Campaign Bank details ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Updating Campaign Bank details ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Campaign/Phase1")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> Phase1(int campaignId, [FromBody]CampaignVwModel1 Model)
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
                int CampaignId = await IService.CreateCampaignPhase1(Model);
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
        [System.Web.Http.Route("api/Campaign/Phase2")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> Phase2(int campaignId, string placeName, string Latitude, string Longitude)
        {
            try
            {
                var httpContext = HttpContext.Current;
                List<HttpPostedFile> Files = new List<HttpPostedFile>();
                CampaignPhase2Full addModel = new CampaignPhase2Full();
                //  addModel.Id = campaignId;
                addModel.campaignId = campaignId;
                addModel.placeName = placeName;
                addModel.Latitude = Latitude;
                addModel.longitude = Longitude;
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                // var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;
                //  addModel.UserId = Convert.ToInt32(userId);
                addModel.UserName = userName;
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
                bool res = await IService.CreateCampaignPhase2(addModel);
                ResponseObject response = new ResponseObject();
                response.campaignId = campaignId.ToString();
                if (res)
                {
                    response.ResponseMsg = "Campaign Updated- Phase 2 successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Campaign Updated- Phase 2 ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Campaign Phase 2 Updation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Campaign/Phase3")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> Phase3(int campaignId, CampaignPhase3Desc model)
        {
            try
            {
                var httpContext = HttpContext.Current;
                CampaignPhase3Desc addModel = new CampaignPhase3Desc();
                addModel.campaignId = model.campaignId;
                addModel.StoryDescription = model.StoryDescription;
                addModel.Id = campaignId;
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                // var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;
                //  addModel.UserId = Convert.ToInt32(userId);
                addModel.UserName = userName;
                bool res = await IService.CreateCampaignPhase3Desc(addModel);
                ResponseObject response = new ResponseObject();
                response.campaignId = model.campaignId.ToString();
                if (res)
                {
                    response.ResponseMsg = "Campaign Updated- Phase 3 successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Campaign Updated- Phase 3 ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Campaign Phase 3 Updation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Campaign/Phase3Image")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> updateCampaignPhase3Image(int campaignId)
        {
            try
            {
                var httpContext = HttpContext.Current;
                CampaignPhase3Image addModel = new CampaignPhase3Image();
                addModel.Id = campaignId;
                // addModel.UserName = Model.UserName;
                List<Attachment> Attach = new List<Attachment>();
                // if(httpContext.Request.Files.)
                if (httpContext.Request.Files.Count > 0)
                {
                    int i = 0;
                    foreach (var ffile in httpContext.Request.Files)
                    {

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
                bool res = await IService.CreateCampaignPhase3Image(addModel);
                ResponseObject response = new ResponseObject();
                response.campaignId = campaignId.ToString();
                if (res)
                {
                    response.ResponseMsg = "Campaign Updated- Phase 3 images successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Campaign Updated- Phase 3 images ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.ToString();
                response.ResponseMsg = "Campaign Phase 3 images Updation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Campaign/Update")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> Update(int campaignId, CampaignPhase3Desc model)
        {
            try
            {
                var httpContext = HttpContext.Current;
                CampaignPhase3Desc addModel = new CampaignPhase3Desc();
                addModel.campaignId = campaignId;
                addModel.StoryDescription = model.StoryDescription;
                addModel.Id = campaignId;
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                // var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString(); ;
                //  addModel.UserId = Convert.ToInt32(userId);
                addModel.UserName = userName;
                int updateId = await IService.CreateUpdateDesc(addModel);
                ResponseObject response = new ResponseObject();
                response.campaignId = model.campaignId.ToString();
                response.UpdateId = updateId.ToString();

                if (updateId > 0)
                {
                    response.ResponseMsg = "Campaign Update posted successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Campaign Update ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Campaign Updation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.Route("api/Campaign/UpdateImage")]
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> updateImage(int campaignId, int updateId)
        {
            try
            {
                if (updateId > 0 && campaignId > 0)
                {

                    var httpContext = HttpContext.Current;
                    CampaignPhase3Image addModel = new CampaignPhase3Image();
                    addModel.Id = campaignId;
                    addModel.UpdateId = updateId;
                    // addModel.UserName = Model.UserName;
                    List<Attachment> Attach = new List<Attachment>();
                    // if(httpContext.Request.Files.)
                    if (httpContext.Request.Files.Count > 0)
                    {
                        int i = 0;
                        foreach (var ffile in httpContext.Request.Files)
                        {

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
                    bool res = await IService.CreateUpdatesImage(addModel);
                    ResponseObject response = new ResponseObject();
                    response.campaignId = campaignId.ToString();
                    if (res)
                    {
                        response.ResponseMsg = "Campaign Updated- Phase 3 images successfully";
                        return Request.CreateResponse(HttpStatusCode.OK, response);
                    }
                    else
                    {
                        response.ResponseMsg = "Campaign Update images ended with a error";
                        return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                    }
                }
                else
                {
                    ResponseObject response = new ResponseObject();
                    response.campaignId = campaignId.ToString();
                    response.ResponseMsg = "Campaign Update images failed - campaign id, update id is mandatory";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.ToString();
                response.ResponseMsg = "Campaign images Updation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Campaign/Withdraw")]
        public async Task<HttpResponseMessage> Withdraw(CampaignWithdrawModel model)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString();
                var userName = claims.Where(a => a.type == ClaimTypes.Name).Select(a => a.value).SingleOrDefault().ToString();
                model.CreatedBy = userName;
                model.CreatedOn = DateTime.UtcNow;


                int benId = await IService.createwithdrawalRequest(model);
                ResponseObject response = new ResponseObject();
                response.campaignId = model.CampaignId.ToString();
                if (benId > 0)
                {
                    response.ResponseMsg = "Campaign Withdrawal request Submitted successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Submitting Campaign Withdrawal request ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "Submitting Campaign Withdrawal request ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Campaign/WithdrawHistory")]
        public async Task<HttpResponseMessage> GetWithdrawHistory(int CampaignId)
        {
            try
            {
                var List = await IService.GetwithdrawalRequestHistory(CampaignId);
                return Request.CreateResponse(HttpStatusCode.OK, List);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "error getting the Campaign Withdrawal history ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Campaign/BankDetail")]
        public async Task<HttpResponseMessage> GetBankDetails(int CampaignId)
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
        [System.Web.Http.Route("api/Campaign/BankDetailForApproval")]
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
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                ResponseObject response = new ResponseObject();
                //var identity = (ClaimsIdentity)User.Identity;
                //var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                //var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString();
                //var UserId = Convert.ToInt32(userId);
                var res = IService.DeleteACampaign(id);
                if (res)
                {
                    response.ResponseMsg = "Campaign deleted Successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = "Campaign deletion ended with a error";
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.ToString();
                response.ResponseMsg = "Campaign images deletion is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Campaign/DeletePhase2Image")]
        public HttpResponseMessage DeletePhase2Image([FromBody] DeleteImageModel model)
        {
            try
            {
                ResponseObject response = new ResponseObject();

                var res = IService.deleteImagePhase2(model);
                if (res == "Image Deleted Successfully")
                {
                    response.ResponseMsg = res;
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = res;
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.ToString();
                response.ResponseMsg = "Campaign images deletion is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Campaign/DeletePhase3Image")]
        public HttpResponseMessage DeletePhase3Image([FromBody] DeleteImageModel model)
        {
            try
            {
                ResponseObject response = new ResponseObject();

                var res = IService.DeleteCampaignPhase3Image(model);
                if (res == "Image Deleted Successfully")
                {
                    response.ResponseMsg = res;
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.ResponseMsg = res;
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                }
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.ToString();
                response.ResponseMsg = "Campaign images deletion is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        [System.Web.Http.Route("api/Campaign/GetAllCampaigns")]
        public CampaignsListViewModel GetAllCampaigns(string Category = "", int page = 1)
        {
            try
            {
                int CategoryId = !string.IsNullOrEmpty(Category) ? Convert.ToInt16(Category) : -1;
                var res = IcmnService.GetCampaignsbyPage(CategoryId, page);
                var categoryList = Enum.GetValues(typeof(StoryCategory)).Cast<StoryCategory>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList().OrderBy(a => a.Text).ToList();

                res.SelectedOptionsList = categoryList;
                res.SCategoryType = Category;

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [System.Web.Http.Route("api/Campaign/GetAroundYouCampaigns")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAroundYouCampaigns(BaseViewModel Base)
        {
            try
            {
                string Category = "All"; int page = 1; int page_size = 12;
                int CategoryId = -1;
                Category = string.IsNullOrEmpty(Category) ? "All" : Category;
                if (Category == "All")
                {
                    CategoryId = -1;
                    Base.Category = "-1";
                }
                else
                {
                    StoryCategory cate = (StoryCategory)Enum.Parse(typeof(StoryCategory), Category);
                    int CateId = (int)cate;
                    CategoryId = CateId;
                    Base.Category = CateId.ToString();
                }
                var res = IService.GetCampaignbyLoc(Base);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of Campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [System.Web.Http.Route("api/Campaign/AskUpdate")]
        [System.Web.Http.HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        public HttpResponseMessage AskUpdate(int CampaignId = 0)
        {
            try
            {

                ResponseObject response = new ResponseObject();
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
                var userName = claims.Where(a => a.type == "DisplayName").Select(a => a.value).SingleOrDefault().ToString();
                // addModel.UserId = Convert.ToInt32(userId);
                var UserId = Convert.ToInt32(userId);
                var res = IService.AskForUpdate(CampaignId, UserId, userName);
                response.campaignId = CampaignId.ToString();
                response.ResponseMsg = res;
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Could not get the details of Campaigns";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
    }
}