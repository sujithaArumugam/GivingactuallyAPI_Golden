using firstWebAPI.Services;
using GivingActuallyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace firstWebAPI.Controllers
{
    public class UserController : ApiController
    {
        // GET api/<controller>
        UserService userservice = new UserService();

        [Authorize(Roles = "SuperAdmin, Admin")]
        public HttpResponseMessage Get(string Status = "All", int page = 1, int page_size = 50)
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

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var result = userservice.GetUserDetailbyId(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.ResponseMsg = "Error Getting the user Detail";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);

            }
        }

        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.HttpPut]
        public HttpResponseMessage Post([FromBody]InputRegisterModel inputModel)
        {
            try
            {
                ResponseObject response = new ResponseObject();
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
                var userId = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString();
                inputModel.UserId = Convert.ToInt32(userId);
                var result = userservice.UpdateUser(inputModel);
                response.userId = result.ToString();
                response.ResponseMsg = "User Updated  successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "User Updation is ended with an exception";
                response.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        
        [System.Web.Http.Route("api/User/UploadDisplayPic")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage UploadUserImage()
        {
            try
            {
                var httpContext = HttpContext.Current;
                ResponseObject response = new ResponseObject();
                if (httpContext.Request.Files.Count > 0)
                {
                    int i = 0;
                    Attachment att = new Attachment();
                    HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                    if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
                    {
                        string res = userservice.uploadUserImage(httpPostedFile);
                        if (!(string.IsNullOrEmpty(res)))
                        {
                            response.UserDPpath = res;
                            response.ResponseMsg = "user Image uploaded successfully";
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            response.ResponseMsg = "User Image Upload ended with an error";
                            return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
                        }
                    }
                }
                response.ResponseMsg = "User Image Upload ended with an error - please send valid iMage";
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, response);
            }
            catch (Exception ex)
            {
                ResponseObject response = new ResponseObject();
                response.ExceptionMsg = ex.InnerException.ToString();
                response.ResponseMsg = "User Image Upload is ended with an exception";
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