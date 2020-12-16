using GivingActuallyAPI.Models;
using GivingActuallyAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GivingActuallyAPI
{
    public class UserAuthentication : IDisposable
    {
        CommonService Iservice = new CommonService();
        public string ValidateUser(string username, string password)
        {
           // string Name = username == "akash" ? "Valid" : "InValid";
          //  string Pass = password == "vidhate" ? "Valid" : "InValid";
            LoginModel model = new LoginModel();
            model.Username = username;
            model.Password = password;
            bool result=Iservice.ValidateUser(model);
            if (result)
                return "true";
            else
                return "false";
        }
        public void Dispose()
        {
            //Dispose();  
        }
    }
}