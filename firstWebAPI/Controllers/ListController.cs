using firstWebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using static GivingActuallyAPI.Models.Helper;

namespace GivingActuallyAPI.Controllers
{
    [CacheFilter(3600, 3600, false)]
    public class ListController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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

        [System.Web.Http.Route("api/List/Categories")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Categories()
        {
            try
            {
                List<SelectListItem> result = new List<SelectListItem>();
                result = Enum.GetValues(typeof(StoryCategory)).Cast<StoryCategory>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList().OrderBy(a => a.Text).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException);
            }
        }

        [System.Web.Http.Route("api/List/NGOTypes")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage NGOTypes()
        {
            try
            {
                List<SelectListItem> result = new List<SelectListItem>();
                result = Enum.GetValues(typeof(NGOType)).Cast<NGOType>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList().OrderBy(a => a.Text).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException);
            }
        }

        [System.Web.Http.Route("api/List/NGOSectors")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage NGOSectors()
        {
            try
            {
                List<SelectListItem> result = new List<SelectListItem>();
                result = Enum.GetValues(typeof(StoryCategory)).Cast<StoryCategory>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList().OrderBy(a => a.Text).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException);
            }
        }




        [System.Web.Http.Route("api/List/Category")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Category()
        {
            try
            {
                List<string> result = new List<string>();
                result = Enum.GetValues(typeof(StoryCategory)).Cast<StoryCategory>().Select(v =>                 
                    v.ToString()                  
                ).ToList().OrderBy(a => a).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException);
            }
        }

        [System.Web.Http.Route("api/List/NGOType")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage NGOType()
        {
            try
            {
                List<string> result = new List<string>();
                result = Enum.GetValues(typeof(NGOType)).Cast<NGOType>().Select(v =>
                    v.ToString()
                ).ToList().OrderBy(a => a).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException);
            }
        }

        [System.Web.Http.Route("api/List/NGOSector")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage NGOSector()
        {
            try
            {
                List<string> result = new List<string>();
                result = Enum.GetValues(typeof(StoryCategory)).Cast<StoryCategory>().Select(v =>
                    v.ToString()
                ).ToList().OrderBy(a => a).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException);
            }
        }


        [System.Web.Http.Route("api/List/MoneyType")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage MoneyType()
        {
            try
            {
                List<string> result = new List<string>();
                result = Enum.GetValues(typeof(MoneyType)).Cast<MoneyType>().Select(v =>
                    v.ToString()
                ).ToList().OrderBy(a => a).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException);
            }
        }


        [System.Web.Http.Route("api/List/BeneficiaryType")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage BeneficiaryType()
        {
            try
            {
                List<string> result = new List<string>();
                result = Enum.GetValues(typeof(BeneficiaryType)).Cast<BeneficiaryType>().Select(v =>
                    v.ToString()
                ).ToList().OrderBy(a => a).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);

                //List<SelectListItem> result = new List<SelectListItem>();
                //result = Enum.GetValues(typeof(BeneficiaryType)).Cast<BeneficiaryType>().Select(v => new SelectListItem
                //{
                //    Text = v.ToString(),
                //    Value = ((int)v).ToString()
                //}).ToList().OrderBy(a => a.Text).ToList();

                //return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException);
            }
        }
    }
}