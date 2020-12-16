using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace GivingActuallyAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });
            var ss = claims.Where(a => a.type == "UserId").Select(a => a.value).SingleOrDefault().ToString(); ;
            //foreach (Claim claim in identity.Claims)
            //{
            //    string v= "CLAIM TYPE: " + claim.Type + "; CLAIM VALUE: " + claim.Value ;
            //    if (claim.Type == "UserId")
            //    {
            //        var s = claim.Value.ToString();
            //    }
            //}

            
            return Ok("Hello: " + identity.Name);
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
