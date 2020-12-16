using firstWebAPI.DataLayer;
using GivingActuallyAPI.Models;
using GivingActuallyAPI.Models.HelperModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static GivingActuallyAPI.Models.Helper;

namespace GivingActuallyAPI.Services
{
    public class CommonService
    {
        GivingActuallyEntities Entity = new GivingActuallyEntities();

        public CampaignsListViewModel GetCampaignsForIndex(int CategoryId = -1)
        {
            try
            {
                CampaignsListViewModel ModelList = new CampaignsListViewModel();
                var res = (from S in Entity.Tbl_Campaign select S).Where(S => S.IsApprovedbyAdmin).OrderBy(s => s.CreatedOn).Take(6).ToList();
                if (res.Any())
                {
                    foreach (var item in res)
                    {
                        CampaignMainViewModel Model = new CampaignMainViewModel();
                        Model = GetCamapignForList(item.Id);
                        ModelList.CampaignViewModelList.Add(Model);
                    }

                }
                var cntry = GetUserCountryByIp();
                ModelList.CountryCode = cntry != null ? cntry.Name : "IN";
                ModelList.CurrencyCode = cntry != null ? cntry.CurrencySymbol : "";
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public CampaignsListViewModel GetCampaigns(int CategoryId = -1)
        {
            try
            {
                CampaignsListViewModel ModelList = new CampaignsListViewModel();
                var res = (from S in Entity.Tbl_Campaign select S).ToList();
                if (CategoryId > -1) res = res.Where(s => s.Category == CategoryId).ToList();
                if (res.Any())
                {
                    res = UserSession.UserRole != RolesEnum.Admin ? res.Where(S => S.IsApprovedbyAdmin && S.Status).ToList() : res;
                    foreach (var item in res)
                    {
                        CampaignMainViewModel Model = new CampaignMainViewModel();
                        Model = GetCamapignForList(item.Id);
                        ModelList.CampaignViewModelList.Add(Model);
                    }

                }
                var cntry = GetUserCountryByIp();
                ModelList.CountryCode = cntry != null ? cntry.Name : "IN";
                ModelList.CurrencyCode = cntry != null ? cntry.CurrencySymbol : "";

                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public CampaignsListViewModel GetCampaignsbyPage(int CategoryId = -1, int page = 1)
        {
            try
            {
                int maxRows = 12;
                CampaignsListViewModel ModelList = new CampaignsListViewModel();
                var result = (from S in Entity.Tbl_Campaign select S).Where(s => CategoryId != -1 ? s.Category == CategoryId : true).ToList()
                     ;
                result = result.Where(S => S.IsApprovedbyAdmin && S.Status).ToList();
                var res = result.OrderByDescending(a => a.CreatedOn)
                        .Skip((page - 1) * maxRows)
                        .Take(maxRows).ToList();
                if (res.Any())
                {
                    foreach (var item in res)
                    {
                        CampaignMainViewModel Model = new CampaignMainViewModel();
                        Model = GetCamapignForList(item.Id);
                        ModelList.CampaignViewModelList.Add(Model);
                    }

                }
                var cntry = GetUserCountryByIp();
                ModelList.CountryCode = cntry != null ? cntry.Name : "IN";
                ModelList.CurrencyCode = cntry != null ? cntry.CurrencySymbol : "";
                double pageCount = (double)((decimal)result.Count() / Convert.ToDecimal(maxRows));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);

                ModelList.CurrentPageIndex = page;
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public CampaignsListViewModel GetCampaignsForSearchbyPage(int CategoryId = -1, string searchtext = "", int page = 1)
        {
            try
            {
                int maxRows = 12;
                CampaignsListViewModel ModelList = new CampaignsListViewModel();
                var result = (from S in Entity.Tbl_Campaign select S).ToList();
                result = UserSession.UserRole != RolesEnum.Admin ?
                    result.Where(S => S.IsApprovedbyAdmin && S.Status).ToList() : result;
                var res = result.OrderByDescending(a => a.CreatedOn)
                        .Skip((page - 1) * maxRows)
                        .Take(maxRows).ToList();
                if (res.Any())
                {
                    foreach (var item in res)
                    {
                        CampaignMainViewModel Model = new CampaignMainViewModel();
                        Model = GetCamapignForList(item.Id);
                        ModelList.CampaignViewModelList.Add(Model);
                    }

                }
                var cntry = GetUserCountryByIp();
                ModelList.CountryCode = cntry != null ? cntry.Name : "IN";
                ModelList.CurrencyCode = cntry != null ? cntry.CurrencySymbol : "";
                double pageCount = (double)((decimal)result.Count() / Convert.ToDecimal(maxRows));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);

                ModelList.CurrentPageIndex = page;
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

      

        public CampaignsListViewModel GetCampaignsbySearchCriteria(string searchtext = "", int page = 1)
        {
            try
            {
                //int maxRows = 12;
                CampaignsListViewModel ModelList = new CampaignsListViewModel();
                var res = GetCampaigns(-1);
                var result = res.CampaignViewModelList;
                result = result.Where(s => s.NGOName.Contains(searchtext)).ToList();

                ModelList.CampaignViewModelList = new List<CampaignMainViewModel>();
                ModelList.CampaignViewModelList.AddRange(result);

                // ModelList.PageCount = (int)Math.Ceiling(pageCount);

                ModelList.CurrentPageIndex = page;


                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

       
       
        public bool ValidateUser(LoginModel model)
        {
            try
            {
                UserModel Model = new UserModel();
                var res = (from S in Entity.Tbl_User where S.IsActive && !S.IsAcLocked && S.UserName == model.Username && S.Password == model.Password select S).FirstOrDefault();
                if (res != null)
                {
                    Model.Id = res.Id;
                    Model.DisplayName = res.Name;
                    Model.UserName = res.UserName;
                    Model.IsAdmin = res.IsAdmin;
                    Model.LastLoginTime = res.LastLoginDate;
                    Model.CurrentLoginDate = res.CurrentLoginDate;
                    res.LastLoginDate = res.CurrentLoginDate;
                    res.CurrentLoginDate = DateTime.UtcNow;

                    Entity.SaveChanges();
                    //Model.IsNGO = res.IsNGO != null ? res.IsNGO.Value : false;
                    // Model.CanEndorse = res.CanEndorse != null ? res.CanEndorse.Value : false;
                    // var endorse = (from S in Entity.tbl_UserNGOEndorsement where S.UserId == res.Id select S).FirstOrDefault();
                    // if (endorse != null)
                    ///  {
                    //     Model.NGOSector = endorse.NGOSector.Value;
                    //     Model.NGOType = endorse.NGOType.Value;
                    // }
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string RegisterUser(InputRegisterModel model)
        {
            try
            {
                string returnVal = "";
                RegisterModel Model = new RegisterModel();
                string Username = model.UserName.ToLower();
                Model.UserName = Username;
                Model.DPName = model.FirstName + " " + model.LastName;
                Model.FirstName = model.FirstName;
                Model.LastName = model.LastName;
                Model.DPpath = model.DPPath;
                Model.ConfirmPassword = model.ConfirmPassword;
                Model.Password = model.Password;
                Model.IsNGO = model.IsNGO;
                Model.canEndorse = model.canEndorse;
                if (Model.Password == Model.ConfirmPassword)
                {
                    var result = (from S in Entity.Tbl_User where (S.UserName == Username && S.IsActive == true && S.IsSpamUser == false) select S).FirstOrDefault();

                    if (result != null)
                    {
                        returnVal = "UserName is Already Taken.Please try different UserName !";

                    }
                    else
                    {
                        var resq = (from S in Entity.Tbl_User where (S.UserName == Username && S.IsActive == false && S.IsSpamUser == true) select S).ToList();

                        if (resq.Any())
                            Entity.Tbl_User.RemoveRange(resq);
                        Tbl_User NewUser = new Tbl_User();
                        NewUser.UserName = model.UserName.ToLower();
                        NewUser.Password = model.Password;
                        NewUser.IsAdmin = false;
                        NewUser.IsNGO = model.IsNGO;
                        NewUser.CanEndorse = model.canEndorse;
                        NewUser.IsAcLocked = false;
                        string initial = "";
                        initial = !(string.IsNullOrEmpty(model.FirstName)) ? model.FirstName : "";
                        initial = initial + " "+ (!(string.IsNullOrEmpty(model.LastName)) ? model.LastName : "");
                        NewUser.Name = initial;
                        NewUser.DPPath = model.DPPath;
                        NewUser.FirstName = model.FirstName;
                        NewUser.LastName = model.LastName;
                        NewUser.IsSpamUser = true;
                        NewUser.IsActive = false;
                        NewUser.Name = model.FirstName + " " + model.LastName;
                        Entity.Tbl_User.Add(NewUser);
                        Entity.SaveChanges();

                        CreatenSendOTP(NewUser.UserName, NewUser.Name);

                        model.UserId = NewUser.Id;
                        if (model.IsNGO)
                            AddNGOModel(model);
                        returnVal = "Registered Successfully. Please try logging in!";



                    }
                }
                else
                {
                    returnVal = "Password and confirm Password doesn't Match";

                }
                return returnVal;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public RegisterModel addUser(RegisterModel model)
        {
            return new RegisterModel();
            //if (model.SecurityToken != null && !string.IsNullOrEmpty(model.SecurityToken))
            //{
            //    var res = (from S in Entity.Tbl_UserAuthentication where S.UserName == Username select S).ToList();
            //    DateTime ChecktimeUTCTime = DateTime.UtcNow.AddMinutes(-20);
            //    if (res.Any())
            //    {
            //        {
            //            var ValidModel = res.Where(c => c.CreatedTime >= ChecktimeUTCTime).FirstOrDefault();
            //            if (ValidModel != null)
            //            {
            //                if (ValidModel.SecurityCode.ToString() == model.SecurityToken)
            //                {
            //                    Tbl_User NewUser = new Tbl_User();
            //                    NewUser.UserName = model.UserName.ToLower();
            //                    NewUser.Password = model.Password;
            //                    NewUser.IsAdmin = false;
            //                    NewUser.IsSpamUser = false;
            //                    NewUser.IsActive = true;
            //                    NewUser.IsAcLocked = false;
            //                    NewUser.Name = model.DPName;
            //                    Entity.Tbl_User.Add(NewUser);
            //                    Entity.SaveChanges();
            //                    Model.IsResitered = true;
            //                }
            //                else
            //                {
            //                    return Model;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        return Model;
            //    }
            //}
            //else
            //{


            //    var result = (from S in Entity.Tbl_User where S.UserName == Username select S).FirstOrDefault();
            //    if (result != null)
            //    {
            //        Model.UserAlreadyExists = true;
            //        Model.IsResitered = true;

            //    }
            //    else
            //    {
            //        DateTime ChecktimeUTCTime = DateTime.UtcNow.AddMinutes(-20);
            //        var res = (from S in Entity.Tbl_UserAuthentication where S.UserName == Username select S).ToList();
            //        Tbl_UserAuthentication Authen = new Tbl_UserAuthentication();
            //        if (res.Any())
            //        {
            //            Authen = res.Where(c => c.CreatedTime >= ChecktimeUTCTime).FirstOrDefault();
            //        }
            //        if (Authen != null)
            //        {
            //            Model.IsSecurityTokenGenerated = true;
            //            Model.UserName = Username;
            //            Model.ConfirmPassword = model.ConfirmPassword;
            //            Model.Password = model.Password;
            //        }
            //        else
            //        {
            //            Model.IsSecurityTokenGenerated = true;
            //            Model.UserName = Username;
            //            Model.ConfirmPassword = model.ConfirmPassword;
            //            Model.Password = model.Password;
            //            Random generator = new Random();
            //            Tbl_UserAuthentication AuthModel = new Tbl_UserAuthentication();
            //            AuthModel.UserName = model.UserName.ToLower();
            //            AuthModel.SecurityCode = generator.Next(100000, 1000000);
            //            AuthModel.CreatedTime = DateTime.UtcNow;
            //            Entity.Tbl_UserAuthentication.Add(AuthModel);
            //            Entity.SaveChanges();

            //            SendMail(model.UserName, AuthModel.SecurityCode.ToString());

            //            //   MailMessage msg = new MailMessage();
            //            //  msg.From = new MailAddress("suji.arumugam@gmail.com");
            //            //  msg.To.Add(new MailAddress(model.UserName));
            //            //  msg.Subject = "One Time Password for your Account";
            //            //  msg.Body = string.Format("Hi {0},<br /><br />Your password is {1}{2}.<br /><br />Thank You.", Username, AuthModel.SecurityCode, "");

            //            //  msg.IsBodyHtml = true;

            //            //  SmtpClient smt = new SmtpClient();
            //            //smt.Host = "relay-hosting.secureserver.net";
            //            // // smt.Host = "smtp.gmail.com";
            //            //  System.Net.NetworkCredential ntwd = new NetworkCredential();
            //            //  ntwd.UserName = "suji.arumugam@gmail.com"; //Your Email ID  
            //            //  ntwd.Password = "coolMadhu2571!"; // Your Password  
            //            //  smt.UseDefaultCredentials = true;
            //            //  smt.Credentials = ntwd;
            //            //  smt.Port = 587;
            //            //  smt.EnableSsl = true;
            //            //  smt.Send(msg);


            //        }
            //    }

            //}
            //return false;
        }
        public UserModel ToregisterModel(RegisterModel reg)
        {
            UserModel um = new UserModel();
            var result = (from S in Entity.Tbl_User where S.UserName == reg.UserName && S.BasedOut == "facebook" select S).FirstOrDefault();
            if (result == null)
            {
                Tbl_User NewUser = new Tbl_User();
                NewUser.UserName = reg.UserName.ToLower();
                NewUser.Password = "";
                NewUser.IsAdmin = false;
                NewUser.IsSpamUser = false;
                NewUser.IsActive = true;
                NewUser.IsAcLocked = false;
                NewUser.Name = reg.DPName;
                NewUser.BasedOut = "facebook";
                NewUser.CurrentLoginDate = DateTime.Now;
                NewUser.LastLoginDate = DateTime.Now;
                Entity.Tbl_User.Add(NewUser);
                Entity.SaveChanges();
                um = GetUserDetailbyId(NewUser.Id);
            }
            else
            {
                result.LastLoginDate = result.CurrentLoginDate;
                result.CurrentLoginDate = DateTime.UtcNow;

                Entity.SaveChanges();
                um = GetUserDetailbyId(result.Id);
            }
            return um;
        }

        public bool AddNGOModel(InputRegisterModel reg)
        {
            try
            {
                UserModel um = new UserModel();
                var result = (from S in Entity.tbl_UserNGOEndorsement where S.UserId == reg.UserId select S).FirstOrDefault();
                if (result == null)
                {
                    tbl_UserNGOEndorsement NewNGO = new tbl_UserNGOEndorsement();

                    NewNGO.NGORegisterationNO = string.IsNullOrEmpty(reg.RegisterationNo) ? "" : reg.RegisterationNo;
                    NewNGO.NGORegAt = string.IsNullOrEmpty(reg.Registeredat) ? "" : reg.Registeredat;
                    if (reg.canEndorse)
                    {
                        if (!string.IsNullOrEmpty(reg.NGOSector))
                        {
                            StoryCategory cate = (StoryCategory)Enum.Parse(typeof(StoryCategory), reg.NGOSector);
                            int NGOSectorId = (int)cate;
                            NewNGO.NGOSector = NGOSectorId;
                        }
                        if (!string.IsNullOrEmpty(reg.NGOType))
                        {
                            NGOType NGOtype = (NGOType)Enum.Parse(typeof(NGOType), reg.NGOType);
                            int NGOTypeId = (int)NGOtype;
                            NewNGO.NGOType = NGOTypeId;
                        }

                    }

                    //NewNGO.StateId = reg.stateId;// string.IsNullOrEmpty(reg.stateId) ? -1: Convert.ToInt32(reg.stateId);

                    //NewNGO.CityId = reg.cityId;

                    //NewNGO.CountryId = reg.countryID;

                    NewNGO.CityName = reg.cityName;
                    NewNGO.StateName = reg.stateName;
                    NewNGO.CountryName = reg.countryName;
                    NewNGO.UserId = reg.UserId;



                    Entity.tbl_UserNGOEndorsement.Add(NewNGO);
                    Entity.SaveChanges();
                    return true;
                }
                else
                {
                    result.NGORegisterationNO = string.IsNullOrEmpty(reg.RegisterationNo) ? "" : reg.RegisterationNo;
                    result.NGORegAt = string.IsNullOrEmpty(reg.Registeredat) ? "" : reg.Registeredat;
                    if (reg.canEndorse)
                    {
                        if (!string.IsNullOrEmpty(reg.NGOSector))
                        {
                            StoryCategory cate = (StoryCategory)Enum.Parse(typeof(StoryCategory), reg.NGOSector);
                            int NGOSectorId = (int)cate;
                            result.NGOSector = NGOSectorId;
                        }
                        if (!string.IsNullOrEmpty(reg.NGOType))
                        {
                            NGOType NGOtype = (NGOType)Enum.Parse(typeof(NGOType), reg.NGOType);
                            int NGOTypeId = (int)NGOtype;
                            result.NGOType = NGOTypeId;
                        }
                    }
                    else
                    {
                        result.NGOSector = 0;
                        result.NGOType = 0;
                    }
                    result.CityName = reg.cityName;
                    result.StateName = reg.stateName;
                    result.CountryName = reg.countryName;
                    result.UserId = reg.UserId;

                    Entity.SaveChanges();

                }
                return true;
            }
            catch (Exception ex)
            { throw ex; }
        }
        public UserModel ToFBregisterModel(RegisterModel reg)
        {
            UserModel um = new UserModel();
            var result = (from S in Entity.Tbl_User where S.UserName.ToLower() == reg.UserName.ToLower() && S.BasedOut == "google" select S).FirstOrDefault();
            if (result == null)
            {
                Tbl_User NewUser = new Tbl_User();
                NewUser.UserName = reg.UserName.ToLower();
                NewUser.Password = "";
                NewUser.IsAdmin = false;
                NewUser.IsSpamUser = false;
                NewUser.IsActive = true;
                NewUser.IsAcLocked = false;
                NewUser.Name = reg.DPName;
                NewUser.BasedOut = "google";
                NewUser.CurrentLoginDate = DateTime.Now;
                NewUser.LastLoginDate = DateTime.Now;
                Entity.Tbl_User.Add(NewUser);
                Entity.SaveChanges();
                um = GetUserDetailbyId(NewUser.Id);
            }
            else
            {
                result.LastLoginDate = result.CurrentLoginDate;
                result.CurrentLoginDate = DateTime.UtcNow;

                Entity.SaveChanges();
                um = GetUserDetailbyId(result.Id);
            }
            return um;
        }

        public RegionInfo GetCountryByIP()
        {
            IpInfo ipInfo = new IpInfo();

            string info = new WebClient().DownloadString("http://ipinfo.io");

            //JavaScriptSerializer jsonObject = new JavaScriptSerializer();
            //ipInfo = jsonObject.Deserialize<IpInfo>(info);

            RegionInfo region = new RegionInfo("IN");

            return region;

        }
        public RegionInfo GetUserCountryByIp()
        {
            RegionInfo region = new RegionInfo("IN");

            try
            {

                IpInfo ipInfo = new IpInfo();


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return region;
        }

        public Locationmodel GetUserCountryByIpforlocation()
        {
            Locationmodel loc = new Locationmodel();

            try
            {
                using (var webClient = new System.Net.WebClient())
                {

                    var data = webClient.DownloadString("https://geolocation-db.com/json");
                    //JavaScriptSerializer jss = new JavaScriptSerializer();
                    //var d = jss.Deserialize<dynamic>(data);

                    ////string country_code = d["country_code"];
                    ////string country_name = d["country_name"];
                    ////string city = d["city"];
                    ////string postal = d["postal"];
                    ////string state = d["state"];
                    ////string ipv4 = d["IPv4"];
                    //decimal latitude = d["latitude"];
                    //decimal longitude = d["longitude"];
                    //loc.CountryCode = d["country_code"];
                    //loc.CityName = d["city"];
                    //loc.IPAddress = d["IPv4"];
                    //loc.Latitude = latitude.ToString();
                    //loc.Longitude = longitude.ToString();


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return loc;
        }

        public string ResetPassword(RegisterModel model)
        {
            try
            {
                RegisterModel Model = new RegisterModel();
                string Username = model.UserName.ToLower();
                Model.UserName = Username;
                Model.ConfirmPassword = model.ConfirmPassword;
                Model.Password = model.Password;

                string returnVal = "";

                if (Model.Password == Model.ConfirmPassword)
                {
                    var result = (from S in Entity.Tbl_User where S.UserName == Username select S).FirstOrDefault();
                    if (result != null)
                    {
                        returnVal = "Password Reset Successful!";
                        result.Password = model.Password;
                        Entity.SaveChanges();
                    }
                }
                else
                {
                    returnVal = "Password and confirm Password doesn't Match";
                }
                return returnVal;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public RegisterModel GetOTPModel(string UserName)
        {
            try
            {
                RegisterModel Model = new RegisterModel();
                string Username = UserName.ToLower();
                var result = (from S in Entity.Tbl_User where (S.UserName == Username && S.IsActive == true && S.IsSpamUser == false) select S).FirstOrDefault();
                if (result != null)
                {
                    Model.UserAlreadyExists = true;
                }
                else
                {
                    var res = (from S in Entity.Tbl_UserAuthentication where S.UserName == Username select S).ToList();
                    DateTime ChecktimeUTCTime = DateTime.UtcNow.AddMinutes(-20);
                    if (res.Any())
                    {
                        {
                            var ValidModel = res.Where(c => c.CreatedTime >= ChecktimeUTCTime).FirstOrDefault();
                            if (ValidModel != null)
                            {
                                Model.IsSecurityTokenGenerated = true;
                            }
                        }
                    }
                    else
                    {
                        Model.IsSecurityTokenGenerated = true;
                        Random generator = new Random();
                        Tbl_UserAuthentication AuthModel = new Tbl_UserAuthentication();
                        AuthModel.UserName = UserName.ToLower();
                        AuthModel.SecurityCode = generator.Next(100000, 1000000);
                        AuthModel.CreatedTime = DateTime.UtcNow;
                        Entity.Tbl_UserAuthentication.Add(AuthModel);
                        Entity.SaveChanges();
                    }
                }
                return Model;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
       public bool CreateCampaignDescription(CampaignMainViewModel viewModel)
        {
            try
            {
                int UserId = UserSession.UserId;
                if (viewModel.campaignDescription.Id == 0)
                {
                    Tbl_CampaignDescription NewStorydesc = new Tbl_CampaignDescription();
                    NewStorydesc.storyDescription = viewModel.campaignDescription.StoryDescription;
                    NewStorydesc.StoryId = viewModel.Id;
                    NewStorydesc.CreatedBy = UserSession.UserName;
                    NewStorydesc.CreatedOn = DateTime.UtcNow;
                    NewStorydesc.Status = true;

                    Entity.Tbl_CampaignDescription.Add(NewStorydesc);
                    Entity.SaveChanges();
                    var NewStoryId = viewModel.Id;
                    if (viewModel.Files.Any())
                    {
                        List<Tbl_StoriesAttachment> StoryAttachmentList = new List<Tbl_StoriesAttachment>();
                        foreach (var item in viewModel.Files)
                        {
                            Tbl_StoriesAttachment StoryAttachment = new Tbl_StoriesAttachment();
                            StoryAttachment.StoryId = NewStoryId;
                            StoryAttachment.MediaFile = item.File;
                            StoryAttachment.FileName = item.FileName;
                            StoryAttachment.CreatedBy = UserSession.UserName;
                            StoryAttachment.CreatedOn = DateTime.UtcNow;
                            StoryAttachment.ContentType = item.ContentType;
                            StoryAttachmentList.Add(StoryAttachment);
                        }
                        Entity.Tbl_StoriesAttachment.AddRange(StoryAttachmentList);
                        Entity.SaveChanges();
                    }
                    return true;
                }
                else
                {
                    var exisitingStorydesc = (from S in Entity.Tbl_CampaignDescription where S.Id == viewModel.campaignDescription.Id select S).FirstOrDefault();
                    if (exisitingStorydesc != null)
                    {
                        exisitingStorydesc.storyDescription = viewModel.campaignDescription.StoryDescription;
                        exisitingStorydesc.UpdatedBy = UserSession.UserName;
                        exisitingStorydesc.UpdatedOn = DateTime.UtcNow;
                        Entity.SaveChanges();
                        if (viewModel.Files.Any())
                        {
                            List<Tbl_StoriesAttachment> StoryAttachmentList = new List<Tbl_StoriesAttachment>();
                            foreach (var item in viewModel.Files)
                            {
                                Tbl_StoriesAttachment StoryAttachment = new Tbl_StoriesAttachment();
                                StoryAttachment.StoryId = viewModel.Id;
                                StoryAttachment.MediaFile = item.File;
                                StoryAttachment.FileName = item.FileName;
                                StoryAttachment.CreatedBy = UserSession.UserName;
                                StoryAttachment.CreatedOn = DateTime.UtcNow;
                                StoryAttachment.ContentType = item.ContentType;
                                StoryAttachmentList.Add(StoryAttachment);
                            }
                            Entity.Tbl_StoriesAttachment.AddRange(StoryAttachmentList);
                            Entity.SaveChanges();
                        }
                        return true;
                    }
                    else { return false; }
                }
            }
            catch (Exception ex) { throw ex; }
        }
        public bool AddPostUpdate(CampaignMainViewModel viewModel)
        {
            try
            {
                int UserId = UserSession.UserId;
                if (viewModel.campaignupdate.Id == 0)
                {
                    Tbl_CampaignDescriptionUpdates NewStorydesc = new Tbl_CampaignDescriptionUpdates();
                    NewStorydesc.storyDescription = viewModel.campaignupdate.StoryDescription;
                    NewStorydesc.StoryId = viewModel.Id;
                    NewStorydesc.CreatedBy = UserSession.UserName;
                    NewStorydesc.CreatedOn = DateTime.UtcNow;
                    NewStorydesc.Status = true;

                    Entity.Tbl_CampaignDescriptionUpdates.Add(NewStorydesc);
                    Entity.SaveChanges();
                    var NewStoryId = viewModel.Id;
                    if (viewModel.Files.Any())
                    {
                        List<Tbl_UpdatesAttachment> StoryAttachmentList = new List<Tbl_UpdatesAttachment>();
                        foreach (var item in viewModel.Files)
                        {
                            Tbl_UpdatesAttachment StoryAttachment = new Tbl_UpdatesAttachment();
                            StoryAttachment.StoryId = NewStoryId;
                            StoryAttachment.MediaFile = item.File;
                            StoryAttachment.FileName = item.FileName;
                            StoryAttachment.CreatedBy = UserSession.UserName;
                            StoryAttachment.CreatedOn = DateTime.UtcNow;
                            StoryAttachment.UpdateId = NewStorydesc.Id;
                            StoryAttachment.ContentType = item.ContentType;
                            StoryAttachmentList.Add(StoryAttachment);
                        }
                        Entity.Tbl_UpdatesAttachment.AddRange(StoryAttachmentList);
                        Entity.SaveChanges();
                    }
                    return true;
                }
                else
                {

                    return true;
                }


            }
            catch (Exception ex) { throw ex; }
        }

        public int CreateCampaign(CampaignMainViewModel viewModel)
        {
            try
            {
                int UserId = UserSession.UserId;
                if (viewModel.Id == 0)
                {
                    Tbl_Campaign NewCampaign = new Tbl_Campaign();

                    NewCampaign.UserId = UserSession.UserId;
                    NewCampaign.IsApprovedbyAdmin = false;
                    NewCampaign.Title = viewModel.CampaignTitle;
                    NewCampaign.Status = true;
                    NewCampaign.CreatedBy = UserSession.UserName;
                    NewCampaign.CreatedOn = DateTime.UtcNow;
                    NewCampaign.Category = Convert.ToInt32(viewModel.SCategoryType);
                    NewCampaign.Story_Expense = "";
                    NewCampaign.storyDescription = "";
                    NewCampaign.MoneyType = viewModel.CampaignTargetMoneyType;
                    NewCampaign.TargetAmount = viewModel.CampaignTargetMoney;
                    NewCampaign.BeneficiaryType = Convert.ToInt32(viewModel.SBeneficiaryType); //viewModel.BeneficiaryType != BeneficiaryType.Select ? viewModel.BeneficiaryType.GetHashCode() : -1;
                    NewCampaign.BName = viewModel.BName != null ? viewModel.BName : "";
                    NewCampaign.BGroupName = viewModel.BGroupName != null ? viewModel.BGroupName : "";
                    NewCampaign.NGOName = viewModel.NGOName != null ? viewModel.NGOName : "";
                    NewCampaign.IsApprovedbyAdmin = true;
                    Entity.Tbl_Campaign.Add(NewCampaign);
                    Entity.SaveChanges();
                    var NewCampaignId = NewCampaign.Id;
                    return NewCampaignId;
                }
                else
                {
                    var ExistingCampaign = (from S in Entity.Tbl_Campaign where S.Id == viewModel.Id select S).FirstOrDefault();
                    if (ExistingCampaign != null)
                    {
                        ExistingCampaign.Title = viewModel.CampaignTitle;
                        ExistingCampaign.storyDescription = "";
                        ExistingCampaign.MoneyType = viewModel.CampaignTargetMoneyType;
                        ExistingCampaign.TargetAmount = viewModel.CampaignTargetMoney;
                        ExistingCampaign.BName = viewModel.BName != null ? viewModel.BName : "";
                        ExistingCampaign.BGroupName = viewModel.BGroupName != null ? viewModel.BGroupName : "";
                        ExistingCampaign.NGOName = viewModel.NGOName != null ? viewModel.NGOName : "";
                        ExistingCampaign.BeneficiaryType = Convert.ToInt32(viewModel.SBeneficiaryType); //viewModel.BeneficiaryType != BeneficiaryType.Select ? viewModel.BeneficiaryType.GetHashCode() : -1;
                        ExistingCampaign.Category = Convert.ToInt32(viewModel.SCategoryType);
                        ExistingCampaign.UpdatedBy = UserSession.UserName;
                        ExistingCampaign.UpdatedOn = DateTime.UtcNow;
                        Entity.SaveChanges();

                        return ExistingCampaign.Id;
                    }
                    else { return 0; }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        //public int CreateCampaign(CampaignMainViewModel viewModel)
        //{
        //    try
        //    {
        //        int UserId = UserSession.UserId;
        //        if (viewModel.Id == 0)
        //        {
        //            Tbl_Campaign NewCampaign = new Tbl_Campaign();

        //            NewCampaign.UserId = UserSession.UserId;
        //            NewCampaign.IsApprovedbyAdmin = false;
        //            NewCampaign.Title = viewModel.CampaignTitle;
        //            NewCampaign.Status = true;
        //            NewCampaign.CreatedBy = UserSession.UserName;
        //            NewCampaign.CreatedOn = DateTime.UtcNow;
        //            NewCampaign.Category = (int)viewModel.StoryCategory;
        //            NewCampaign.Story_Expense = "";
        //            NewCampaign.storyDescription = "";
        //            NewCampaign.MoneyType = viewModel.CampaignTargetMoneyType;
        //            NewCampaign.TargetAmount = viewModel.CampaignTargetMoney;
        //            NewCampaign.BeneficiaryType = viewModel.BeneficiaryType != BeneficiaryType.Select ? viewModel.BeneficiaryType.GetHashCode() : -1;


        //            Entity.Tbl_Campaign.Add(NewCampaign);
        //            Entity.SaveChanges();
        //            var NewCampaignId = NewCampaign.Id;
        //            return NewCampaignId;
        //        }
        //        else
        //        {
        //            var ExistingCampaign = (from S in Entity.Tbl_Campaign where S.Id == viewModel.Id select S).FirstOrDefault();
        //            if (ExistingCampaign != null)
        //            {
        //                ExistingCampaign.Title = viewModel.CampaignTitle;
        //                ExistingCampaign.storyDescription = "";
        //                ExistingCampaign.MoneyType = viewModel.CampaignTargetMoneyType;
        //                ExistingCampaign.TargetAmount = viewModel.CampaignTargetMoney;
        //                ExistingCampaign.BeneficiaryType = viewModel.BeneficiaryType != BeneficiaryType.Select ? viewModel.BeneficiaryType.GetHashCode() : -1;
        //                ExistingCampaign.Category = (int)viewModel.StoryCategory;
        //                ExistingCampaign.UpdatedBy = UserSession.UserName;
        //                ExistingCampaign.UpdatedOn = DateTime.UtcNow;
        //                Entity.SaveChanges();

        //                return ExistingCampaign.Id;
        //            }
        //            else { return 0; }
        //        }
        //    }
        //    catch (Exception ex) { throw ex; }
        //}
      



    
        public bool ApproveCampaign(int Id, bool Status)
        {
            try
            {
                var res = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                res.IsApprovedbyAdmin = Status;
                Entity.SaveChanges();
                return true;

            }
            catch (Exception ex) { throw ex; }
        }

        public bool DeleteCampaign(int Id)
        {
            try
            {
                int UserId = UserSession.UserId;

                var res = (from S in Entity.Tbl_Campaign where S.Id == Id && S.UserId == UserId select S).FirstOrDefault();
                if (res != null)
                {
                    var resAttachment = (from S in Entity.Tbl_StoriesAttachment where S.StoryId == Id select S).ToList();
                    if (resAttachment.Any())
                    {
                        Entity.Tbl_StoriesAttachment.RemoveRange(resAttachment);
                        // Entity.SaveChanges();
                    }
                    var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails where S.StoryId == Id select S).ToList();
                    if (Beneficiary != null)
                    {
                        Entity.Tbl_BeneficiaryDetails.RemoveRange(Beneficiary);

                    }
                    var Target = (from S in Entity.Tbl_CampaignDescription where S.StoryId == Id select S).ToList();
                    if (Target != null)
                    {
                        Entity.Tbl_CampaignDescription.RemoveRange(Target);
                    }
                    var Cmnts = (from S in Entity.Tbl_ParentComment where S.StoryId == Id select S).ToList();
                    if (Cmnts != null)
                    {
                        Entity.Tbl_ParentComment.RemoveRange(Cmnts);
                    }
                    var Likes = (from S in Entity.Tbl_Like where S.StoryId == Id select S).ToList();
                    if (Likes != null)
                    {
                        Entity.Tbl_Like.RemoveRange(Likes);
                    }
                    var shares = (from S in Entity.Tbl_Shares where S.StoryId == Id select S).ToList();


                    if (shares != null)
                    {
                        Entity.Tbl_Shares.RemoveRange(shares);
                    }
                    var endorseresult = (from S in Entity.Tbl_Endorse
                                         where S.CampaignId == Id
                                         select S).ToList();
                    if (endorseresult != null)
                    {
                        Entity.Tbl_Endorse.RemoveRange(endorseresult);
                    }

                    Entity.Tbl_Campaign.Remove(res);
                    Entity.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public bool DeleteAttachment(int Id, int storyID)
        {
            try
            {


                var resAttachment = (from S in Entity.Tbl_StoriesAttachment where S.StoryId == storyID && S.Id == Id select S).ToList();
                if (resAttachment.Any())
                {
                    Entity.Tbl_StoriesAttachment.RemoveRange(resAttachment);
                    Entity.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public UserDetails GetUserDetails()
        {
            try
            {
                int UserId = UserSession.UserId;
                UserDetails ModelList = new UserDetails();
                var res = (from S in Entity.Tbl_User select S).ToList();
                //res = res.Any() && !UserSession.HasSession ? res.Where(s => s.Status).ToList() : res;
                if (res.Any() && UserSession.UserRole == RolesEnum.Admin)
                {
                    foreach (var item in res)
                    {
                        UserModel Model = new UserModel();
                        Model.Id = item.Id;
                        Model.UserName = item.UserName;
                        Model.IsAdmin = item.IsAdmin;
                        Model.IsAcLocked = item.IsAcLocked;
                        Model.IsSpamUser = item.IsSpamUser;
                        Model.IsActive = item.IsActive;

                        ModelList.Users.Add(Model);
                    }
                }
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //public StoryAccountsDetails GetStoryAccountsDetails(int Id = 0)
        //{
        //    try
        //    {
        //        StoryAccountsDetails Model = new StoryAccountsDetails();
        //        Model.StoryID = Id;
        //        var res = (from S in Entity.Tbl_Stories where S.Status && S.Id == Id select S.Story_Expense).FirstOrDefault();
        //        if (res != null)
        //        {
        //            HelperService ser = new HelperService();
        //            Model = ser.Deserialize<StoryAccountsDetails>(res);
        //            // xmlOutputData = ser.Serialize<Customer>(customer)
        //        }
        //        return Model;

        //    }
        //    catch (Exception ex) { throw ex; }

        //}

        //public bool AddStoryAccountsDetails(StoryAccountsDetails Model)
        //{
        //    try
        //    {

        //        var res = (from S in Entity.Tbl_Stories where S.Status && S.Id == Model.StoryID select S).FirstOrDefault();
        //        if (res != null)
        //        {
        //            HelperService ser = new HelperService();
        //            //StoryAccountsDetails customer = ser.Deserialize<StoryAccountsDetails>(res);
        //            res.Story_Expense = ser.Serialize<StoryAccountsDetails>(Model);
        //            Entity.SaveChanges();
        //        }
        //        return true;

        //    }
        //    catch (Exception ex) { throw ex; }

        //}

        public DataTable GetdtLatLong(string locatin)

        {
            try
            {
                string url = "http://maps.google.com/maps/api/geocode/xml?address=" + locatin + "&sensor=false";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        DataSet dsResult = new DataSet();
                        dsResult.ReadXml(reader);
                        DataTable dtCoordinates = new DataTable();
                        dtCoordinates.Columns.AddRange(new DataColumn[4] { new DataColumn("Id", typeof(int)),
                        new DataColumn("Address", typeof(string)),
                        new DataColumn("Latitude",typeof(string)),
                        new DataColumn("Longitude",typeof(string)) });
                        foreach (DataRow row in dsResult.Tables[0].Rows)
                        {
                            string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
                            DataRow location = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
                            dtCoordinates.Rows.Add(row["result_id"], row["formatted_address"], location["lat"], location["lng"]);
                        }
                        return dtCoordinates;

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int getidfromPolar(string Placename, string latitude, string longitude)
        {
            try
            {
                if (latitude == null)
                {
                    Locationmodel location = GetUserCountryByIpforlocation();
                    List<string> listloc = new List<string>();
                    latitude = location.Latitude;
                    longitude = location.Longitude;
                }

                Tbl_CityDetails placedetails = new Tbl_CityDetails();

                placedetails.CityName = Placename;
                placedetails.Latitude = latitude;
                placedetails.longitude = longitude;
                Entity.Tbl_CityDetails.Add(placedetails);
                Entity.SaveChanges();
                return placedetails.CityId;


            }
            catch (Exception ex)
            { throw ex; }
        }
        public int AddOrganizer(CampainOrganizerViewModel viewModel)
        {
            try
            {
                int UserId = UserSession.UserId;
                if (viewModel.Id == 0)
                {
                    Tbl_BeneficiaryDetails beneficiary = new Tbl_BeneficiaryDetails();
                    beneficiary.StoryId = viewModel.storyId;
                    beneficiary.Status = true;
                    beneficiary.BResidence = getidfromPolar(viewModel.placeNmae, viewModel.Latitude, viewModel.longitude);
                    beneficiary.DP = viewModel.BDisplayPic;
                    beneficiary.DPName = viewModel.BDisplayPicName != null ? viewModel.BDisplayPicName : "";
                    beneficiary.CreatedBy = UserSession.UserName;
                    beneficiary.CreatedOn = DateTime.UtcNow;
                    Entity.Tbl_BeneficiaryDetails.Add(beneficiary);
                    Entity.SaveChanges();
                    var NewBEneficiaryId = beneficiary.Id;
                    return NewBEneficiaryId;
                }
                else
                {
                    var beneficiary = (from S in Entity.Tbl_BeneficiaryDetails where S.Id == viewModel.Id select S).FirstOrDefault();

                    if (beneficiary != null)
                    {
                        beneficiary.BResidence = getidfromPolar(viewModel.placeNmae, viewModel.Latitude, viewModel.longitude);
                        //beneficiary.DP = viewModel.BDisplayPic;
                        //beneficiary.DPName = viewModel.BDisplayPicName != null ? viewModel.BDisplayPicName : "";
                        beneficiary.UpdatedBy = UserSession.UserName;
                        beneficiary.UpdatedOn = DateTime.UtcNow;
                        beneficiary.Status = true;
                        Entity.SaveChanges();
                        return beneficiary.Id;
                    }
                    else { return 0; }
                }
            }
            catch (Exception ex) { throw ex; }
        }

       

        public bool SendMail(string UserName, string DisplayName, string Password)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            string msg = string.Empty;
            try
            {
                MailAddress fromAddress = new MailAddress("contact@givingactually.com", "The GivingActually");
                message.From = fromAddress;
                message.To.Add(new MailAddress(UserName, DisplayName));
                message.Subject = "GivingActually- One Time Password for your Account";
                message.IsBodyHtml = true;
                message.Body = string.Format(@"Hi {0},<br /><br /><strong>Your password is {1}.</strong><br /><br />Thank You.", DisplayName, Password);

                smtpClient.Host = "relay-hosting.secureserver.net";   //-- Donot change.
                smtpClient.Port = 25; //--- Donot change
                smtpClient.EnableSsl = false;//--- Donot change
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new System.Net.NetworkCredential("contact@givingactually.com", "W@aWnfWTG7PM3AQ");

                smtpClient.Send(message);

                return true;
            }
            catch (Exception ex)
            { throw ex; }
        }
        public bool SendMail1(string UserName, string DisplayName, string Password)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();

                //  mailMsg.To.Add(new MailAddress("suji.arumugam@gmail.com", "The Recipient"));
                mailMsg.To.Add(new MailAddress(UserName, DisplayName));
                mailMsg.From = new MailAddress("info@givingactually.com", "The GivingActually");

                //mailMsg.Subject = "GivingActually- One Time Password for your Account";
                //msg.Body = 

                mailMsg.Subject = "GivingActually- One Time Password for your Account";
                string text = "Test Email with SendGrid using .NET's Built-in SMTP Library";
                string html = string.Format(@"Hi {0},<br /><br /><strong>Your password is {1}.</strong><br /><br />Thank You.", DisplayName, Password);

                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("apikey", "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");
                smtpClient.Credentials = credentials;
                //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
                smtpClient.Send(mailMsg);

                //SmtpClient smtp = new SmtpClient("smtp.zoho.com", 587); //465 - SSL | 587 - TLS

                return true;




                ////Create the msg object to be sent
                //MailMessage msg = new MailMessage();
                ////Add your email address to the recipients
                //msg.To.Add(UserName.ToLower());
                ////Configure the address we are sending the mail from
                //MailAddress address = new MailAddress("suji.arumugam@gmail.com");
                //msg.From = address;
                //msg.Subject = "One Time Password for your Account";
                //msg.Body = string.Format("Hi syhi {0},<br /><br />Your password is {1}{2}.<br /><br />Thank You.", UserName, Password, "");


                ////Configure an SmtpClient to send the mail.            
                //SmtpClient client = new SmtpClient();
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.EnableSsl = true;
                //client.Host = "smtp.gmail.com";
                //client.Port = 587;

                ////Setup credentials to login to our sender email address ("UserName", "Password")
                //client.UseDefaultCredentials = false;
                //NetworkCredential credentials = new NetworkCredential("suji.arumugam@gmail.com", "coolMadhu2571*");

                //client.Credentials = credentials;

                ////Send the msg
                //client.Send(msg);
                //return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public UserModel GetUserDetail()
        {
            try
            {
                int UserId = UserSession.UserId;
                UserModel ModelList = new UserModel();
                var res = (from S in Entity.Tbl_User where S.Id == UserId select S).FirstOrDefault();
                if (res != null)
                {
                    ModelList.Id = res.Id;
                    ModelList.UserName = res.UserName;
                    ModelList.DisplayName = res.Name;
                }

                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public UserModel GetUserDetailbyId(int id)
        {
            try
            {
                int UserId = id;
                UserModel ModelList = new UserModel();
                var res = (from S in Entity.Tbl_User where S.Id == UserId select S).FirstOrDefault();
                if (res != null)
                {
                    ModelList.Id = res.Id;
                    ModelList.UserName = res.UserName;
                    string initial = "";
                    initial = !(string.IsNullOrEmpty(res.FirstName)) ? res.FirstName : "";
                    initial = initial + (!(string.IsNullOrEmpty(res.LastName)) ? res.LastName : "");
                    ModelList.DisplayName = initial;
                    ModelList.FirstName = !(string.IsNullOrEmpty(res.FirstName)) ? res.FirstName : "";
                    ModelList.LastName = !(string.IsNullOrEmpty(res.LastName)) ? res.LastName : "";
                    ModelList.DPPAth = !(string.IsNullOrEmpty(res.DPPath)) ? res.DPPath : "";
                    initial = "";
                    initial = !(string.IsNullOrEmpty(res.FirstName)) ? res.FirstName.Substring(0, 1) : "";
                    initial = initial + (!(string.IsNullOrEmpty(res.LastName)) ? res.LastName.Substring(0, 1) : "");
                    ModelList.DispalyInitiial = initial;
                    ModelList.IsNGO = res.IsNGO != null ? res.IsNGO.Value : false;
                    ModelList.CanEndorse = res.CanEndorse != null ? res.CanEndorse.Value : false;
                    if (ModelList.IsNGO)
                    {
                        var NGO = (from S in Entity.tbl_UserNGOEndorsement where S.UserId == UserId select S).FirstOrDefault();
                        if (NGO != null)
                        {

                            ModelList.NGOType = NGO.NGOType.Value;
                            ModelList.NGOSector = NGO.NGOSector.Value;

                            ModelList.NGOAddress = "";
                            int state = NGO.StateId != null ? NGO.StateId.Value : 0;
                            int city = NGO.CityId != null ? NGO.CityId.Value : 0;
                            int country = NGO.CountryId != null ? NGO.CountryId.Value : 0;
                       
                           

                        }

                    }

                }

                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public UserModel GetUserDetailbyName(string UserName)
        {
            try
            {
                UserModel ModelList = new UserModel();
                var res = (from S in Entity.Tbl_User where S.UserName == UserName select S).FirstOrDefault();
                if (res != null)
                {
                    ModelList.Id = res.Id;
                    string initial = "";
                    initial = !(string.IsNullOrEmpty(res.FirstName)) ? res.FirstName : "";
                    initial = initial + (!(string.IsNullOrEmpty(res.LastName)) ? res.LastName : "");
                    ModelList.DisplayName = initial;
                    ModelList.FirstName = !(string.IsNullOrEmpty(res.FirstName)) ? res.FirstName : "";
                    ModelList.LastName = !(string.IsNullOrEmpty(res.LastName)) ? res.LastName : "";
                    ModelList.DPPAth = !(string.IsNullOrEmpty(res.DPPath)) ? res.DPPath : "";
                    initial = "";
                    initial = !(string.IsNullOrEmpty(res.FirstName)) ? res.FirstName.Substring(0, 1) : "";
                    initial = initial + (!(string.IsNullOrEmpty(res.LastName)) ? res.LastName.Substring(0, 1) : "");
                    ModelList.DispalyInitiial = initial;
                    ModelList.IsAdmin = res.IsAdmin;
                    ModelList.IsNGO = res.IsNGO != null ? res.IsNGO.Value : false;
                    ModelList.CanEndorse = res.CanEndorse != null ? res.CanEndorse.Value : false;
                    ModelList.LastLoginTime = res.LastLoginDate;
                }

                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public CampaignMainViewModel AddComments(PostCommentsVM comments)
        {
            try
            {
                CommentsVM cmt = new CommentsVM();
                cmt.CommentMsg = comments.CommentText;
                cmt.CommentedDate = DateTime.Now;
                cmt.campaignId = comments.Id;
                cmt.Users = GetUserDetail();
                cmt.SubComments = new List<SubCommentsVM>();
                Tbl_ParentComment cmnt = new Tbl_ParentComment();
                cmnt.UserId = UserSession.UserId;
                cmnt.StoryId = comments.Id;
                cmnt.CommentDate = cmt.CommentedDate;
                cmnt.CommentMessage = comments.CommentText;
                Entity.Tbl_ParentComment.Add(cmnt);

                Entity.SaveChanges();

                CampaignMainViewModel stry = new CampaignMainViewModel();
                stry = GetCamapign(comments.Id);

                return stry;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CampaignMainViewModel AddShares(Sharemodel share)
        {
            try
            {

                Tbl_Shares shr = new Tbl_Shares();



                shr.UserId = UserSession.UserId;
                shr.StoryId = share.Id;
                shr.updatedDate = DateTime.Now;
                shr.Media = share.media;
                Entity.Tbl_Shares.Add(shr);

                Entity.SaveChanges();

                CampaignMainViewModel stry = new CampaignMainViewModel();
                stry = GetCamapignShares(share.Id);

                return stry;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ENdorsementList AddEndorsementss(int Id)
        {
            try
            {
                int UserId = UserSession.UserId;
                Tbl_Endorse end = new Tbl_Endorse();

                end.NGOId = UserId;
                end.CampaignId = Id;
                end.updatedDate = DateTime.Now;
                Entity.Tbl_Endorse.Add(end);
                Entity.SaveChanges();
                var result = (from S in Entity.Tbl_Endorse
                              where S.CampaignId == Id
                              select S).ToList();
                ENdorsementList list = new ENdorsementList();
                List<Endorsement> individuallist = new List<Endorsement>();
                foreach (var val in result)
                {
                    var user = GetUserDetailbyId(val.NGOId);
                    Endorsement newval = new Endorsement();
                    newval.NGOId = val.NGOId;
                    newval.NGOName = user.DisplayName;
                    newval.endorsementId = val.EndorseID;
                    individuallist.Add(newval);
                }
                list.CampaignId = Id;
                list.TotalCount = result.Count();
                list.EndorseList = individuallist;
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public CampaignMainViewModel ToggleLikes(PostLikes Likeviewmodel)
        {
            try
            {
                if (Likeviewmodel.isLiked)
                {
                    LikesVM like = new LikesVM();
                    like.likedDate = DateTime.Now;
                    like.campaignId = Likeviewmodel.Id;
                    like.Users = GetUserDetail();
                    Tbl_Like lke = new Tbl_Like();

                    lke.UserId = UserSession.UserId;
                    lke.StoryId = Likeviewmodel.Id;
                    lke.updatedDate = like.likedDate;
                    Entity.Tbl_Like.Add(lke);
                }
                else
                {
                    int UserId = UserSession.UserId;
                    var Likes = (from S in Entity.Tbl_Like where S.UserId == UserId && S.StoryId == Likeviewmodel.Id select S).ToList();

                    if (Likes != null)
                    {
                        Entity.Tbl_Like.RemoveRange(Likes);
                    }

                }
                Entity.SaveChanges();

                CampaignMainViewModel stry = new CampaignMainViewModel();
                stry = GetCamapign(Likeviewmodel.Id);

                return stry;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        public List<string> GetCityNamesBYStatistics(BaseViewModel model)
        {
            try
            {
                Locationmodel location = new Locationmodel();
                if ((model.Latitude == "" || model.Longitude == "") || (model.Latitude == "0" || model.Longitude == "0") || (model.Latitude == null || model.Longitude == null))
                {
                    location = GetUserCountryByIpforlocation();
                }
                else
                {
                    location.Latitude = model.Latitude;
                    location.Longitude = model.Longitude;
                }
                List<string> listloc = new List<string>();



                var matches = Entity.GetnearestCities(location.Latitude, location.Longitude);

                var res = (from S in Entity.Tbl_GeoLocation select S).ToList();
                foreach (var item in res)
                {
                    listloc.Add(item.CityId.ToString());
                }
                return listloc;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public CampainOrganizerViewModel createViewModelOrganizer(Tbl_BeneficiaryDetails beneficiary)
        {
            try
            {
                CampainOrganizerViewModel Organizer = new CampainOrganizerViewModel();
                Organizer.storyId = beneficiary.StoryId;
                //Organizer.CountryId = Convert.ToInt32(beneficiary.BCountry);
                Organizer.Bresidence = Convert.ToInt32(beneficiary.BResidence);

                var res = (from S in Entity.Tbl_CityDetails where S.CityId == Organizer.Bresidence select S).FirstOrDefault();
                if (res != null)
                {
                    Organizer.placeNmae = res.CityName;
                    Organizer.Latitude = res.Latitude;
                    Organizer.longitude = res.longitude;
                }
                // Organizer.BPinCode = beneficiary.BPincode;//need to change to exact pincode of table
                Organizer.BDisplayPic = beneficiary.DP;
                Organizer.BDisplayPicName = beneficiary.DPName != null ? beneficiary.DPName : "";

                return Organizer;
            }
            catch (Exception ex)
            { throw ex; }
        }
        public string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            var Result = new string(array, 0, arrayIndex);
            Result = Result.Replace("&nbsp;", " ");
            Result = Result.Replace("&#39;", "'");
            return Result;
        }
        public CampaignMainViewModel GetCamapignForList(int Id)
        {
            try
            {
                int UserId = 0;
                CampaignMainViewModel Model = new CampaignMainViewModel();
                var res = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                if (res != null)
                {
                    Model.Id = res.Id;
                    Model.StoryCategory = (StoryCategory)res.Category;
                    Model.CategoryName = Model.StoryCategory.DisplayName();
                    Model.CampainOrganizer = new CampainOrganizerViewModel();
                    Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                    Model.CampaignTitle = res.Title != null ? res.Title : "";
                    Model.UserId = res.UserId;

                    Model.CampaignTargetMoney = res.TargetAmount.Value;
                    Model.CampaignTargetMoneyType = res.MoneyType;


                    var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails where S.StoryId == Id select S).FirstOrDefault();
                    if (Beneficiary != null)
                    {
                        CampainOrganizerViewModel ben = new CampainOrganizerViewModel();
                        ben = createViewModelOrganizer(Beneficiary);
                        ben.Id = Beneficiary.Id;
                        Model.CampainOrganizer = ben;
                        Model.Latitude = ben.Latitude;
                        Model.Longitude = ben.longitude;

                        String str = Model.CampainOrganizer.placeNmae != null ? Model.CampainOrganizer.placeNmae : "";
                        String[] spearator = { "," };
                        String[] strlist = str.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                        var result = strlist.Reverse().Take(2).Reverse().ToArray();
                        var val = string.Join(", ", result);
                        Model.CampainOrganizer.placeNmae = val;
                    }


                    var description = (from S in Entity.Tbl_CampaignDescription where S.StoryId == Id select S).FirstOrDefault();
                    if (description != null)
                    {
                        CampaignDescription desc = new CampaignDescription();
                        desc.StoryDescription = description.storyDescription;
                        desc.Id = description.Id;
                        desc.StripedDescription = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                        Model.campaignDescription = desc;
                    }
                    List<CampaignDonation> donationList = new List<CampaignDonation>();
                    var donations = (from S in Entity.Tbl_CampaignDonation where S.StoryId == Id && S.isPaid == true select S).ToList();
                    if (donations.Any())
                    {
                        CampaignDonation donationval = new CampaignDonation();
                        decimal RaisedAmt = 0;
                        long RaisedBy = 0;

                        foreach (var dntion in donations)
                        {
                            RaisedAmt = RaisedAmt + dntion.DonationAmnt;
                            RaisedBy++;
                            donationList.Add(new CampaignDonation() { DonatedBy = dntion.DonatedBy, isAnanymous = dntion.isAnanymous.Value, DonationAmnt = dntion.DonationAmnt, DonatedOn = dntion.DonatedOn.Value });
                        }

                        decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                        var raisedPerc = (RaisedAmt / Model.CampaignTargetMoney) * 100;
                        Model.CampaignDonationList.AddRange(donationList);
                        Model.RaisedAmount = RaisedAmt;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);
                    }

                }

                return Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public CampaignMainViewModel GetCamapign(int Id)
        {
            try
            {
                int UserId = UserSession.HasSession != false ? UserSession.UserId : 0;
                CampaignMainViewModel Model = new CampaignMainViewModel();
                var res = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                if (res != null)
                {
                    Model.Id = res.Id;
                    Model.StoryCategory = (StoryCategory)res.Category;
                    Model.SCategoryType = res.Category.ToString(); ;
                    Model.CategoryName = Model.StoryCategory.DisplayName();
                    Model.CampainOrganizer = new CampainOrganizerViewModel();
                    Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                    Model.CampaignTitle = res.Title != null ? res.Title : "";
                    Model.UserId = res.UserId;
                    if (Model.UserId == UserId)
                        Model.loggedinUser = true;
                    Model.Status = res.Status;
                    Model.CreatedBy = res.CreatedBy;
                    Model.CreatedOn = res.CreatedOn;
                    Model.UpdatedBy = res.UpdatedBy;
                    Model.UpdatedOn = res.UpdatedOn;
                    Model.BGroupName = res.BGroupName;
                    Model.NGOName = res.NGOName;
                    Model.BName = res.BName;
                    Model.BeneficiaryType = (BeneficiaryType)Enum.Parse(typeof(BeneficiaryType), Enum.GetName(typeof(BeneficiaryType), res.BeneficiaryType.Value), true);
                    Model.SBeneficiaryType = res.BeneficiaryType.Value.ToString();

                    Model.CampaignTargetMoney = res.TargetAmount.Value;
                    Model.CampaignTargetMoneyType = res.MoneyType;
                    int i = 0;
                    var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails where S.StoryId == Id select S).FirstOrDefault();
                    if (Beneficiary != null)
                    {
                        CampainOrganizerViewModel ben = new CampainOrganizerViewModel();
                        ben = createViewModelOrganizer(Beneficiary);
                        ben.Id = Beneficiary.Id;
                        Model.CampainOrganizer = ben;
                        Model.Latitude = ben.Latitude;
                        Model.Longitude = ben.longitude;
                        if (ben.BDisplayPic != null)
                        {
                            Files files = new Files();
                            files.File = ben.BDisplayPic;
                            files.FileName = ben.BDisplayPicName;
                            files.ContentType = "";
                            files.Index = 0;
                            Model.Files.Add(files);
                            i++;
                        }
                    }
                    var resFiles = (from F in Entity.Tbl_StoriesAttachment where F.StoryId == Id select F).ToList();
                    if (resFiles.Any())
                    {

                        foreach (var fItem in resFiles)
                        {
                            Files files = new Files();
                            files.File = fItem.MediaFile;
                            files.FileName = fItem.FileName;
                            files.ContentType = fItem.ContentType;
                            files.AttId = fItem.Id;
                            files.Index = i;
                            Model.Files.Add(files);
                            i++;
                        }
                    }




                    var description = (from S in Entity.Tbl_CampaignDescription where S.StoryId == Id select S).FirstOrDefault();
                    if (description != null)
                    {
                        CampaignDescription desc = new CampaignDescription();
                        desc.StoryDescription = description.storyDescription;
                        desc.Id = description.Id;
                        desc.StripedDescription = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                        Model.campaignDescription = desc;

                    }

                    var updates = (from S in Entity.Tbl_CampaignDescriptionUpdates where S.StoryId == Id select S).ToList();
                    if (updates.Any())
                    {
                        List<CampaignUpdates> updatesList = new List<CampaignUpdates>();
                        foreach (var update in updates)
                        {
                            if (update != null)
                            {
                                CampaignUpdates Updt = new CampaignUpdates();
                                CampaignDescription desc = new CampaignDescription();
                                desc.StoryDescription = update.storyDescription;
                                desc.Id = update.Id;

                                desc.StripedDescription = update.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                                Updt.UpdateDescription = desc;
                                Updt.updatedOn = update.CreatedOn.Value;

                                var uptFiles = (from F in Entity.Tbl_UpdatesAttachment where F.StoryId == Id && F.UpdateId == update.Id select F).ToList();
                                if (uptFiles.Any())
                                {

                                    List<Files> filesList = new List<Files>();
                                    foreach (var fItem in uptFiles)
                                    {
                                        Files files = new Files();
                                        files.File = fItem.MediaFile;
                                        files.FileName = fItem.FileName;
                                        files.ContentType = fItem.ContentType;
                                        files.AttId = fItem.Id;
                                        files.updtId = fItem.UpdateId;
                                        filesList.Add(files);
                                    }
                                    Updt.Files = filesList;
                                }
                                updatesList.Add(Updt);
                            }
                        }
                        Model.Updates = updatesList;
                    }

                    var Cmnts = (from S in Entity.Tbl_ParentComment where S.StoryId == Id select S).ToList();
                    if (Cmnts.Any())
                    {
                        List<CommentsVM> cmntModel = new List<CommentsVM>();
                        foreach (var Cmnt in Cmnts)
                        {
                            CommentsVM cmt = new CommentsVM();
                            cmt.CommentMsg = Cmnt.CommentMessage;
                            cmt.CommentedDate = Cmnt.CommentDate.Value;
                            cmt.campaignId = Cmnt.StoryId;
                            cmt.Users = GetUserDetailbyId(Cmnt.UserId);
                            cmt.SubComments = new List<SubCommentsVM>();
                            cmntModel.Add(cmt);
                        }
                        Model.Comments.AddRange(cmntModel);
                        Model.CommentCount = Cmnts.Count();
                    }




                    var endorseresult = (from S in Entity.Tbl_Endorse
                                         where S.CampaignId == Id
                                         select S).ToList();
                    ENdorsementList list = new ENdorsementList();
                    List<Endorsement> individuallist = new List<Endorsement>();
                    foreach (var val in endorseresult)
                    {
                        if (val.NGOId == UserSession.UserId)
                            Model.isCtNGOEndorsed = true;
                        var user = GetUserDetailbyId(val.NGOId);
                        Endorsement newval = new Endorsement();
                        newval.NGOId = val.NGOId;
                        newval.NGOName = user.DisplayName;
                        newval.endorsementId = val.EndorseID;
                        individuallist.Add(newval);
                    }
                    list.CampaignId = Id;
                    list.TotalCount = endorseresult.Count();
                    list.EndorseList = individuallist;
                    Model.EndorsementsList = list;

                    var shares = (from S in Entity.Tbl_Shares where S.StoryId == Id select S).ToList().Count();
                    Model.sharecount = shares;

                    var likes = (from S in Entity.Tbl_Like where S.StoryId == Id select S).ToList().Count();
                    Model.LikeCount = likes;
                    //  int UserId = UserSession.UserId;
                    var likebyuser = (from S in Entity.Tbl_Like
                                      where S.StoryId == Id && S.UserId == UserId
                                      select S).FirstOrDefault();
                    if (likebyuser != null)
                        Model.isLiked = true;
                    List<CampaignDonation> donationList = new List<CampaignDonation>();
                    var donations = (from S in Entity.Tbl_CampaignDonation where S.StoryId == Id && S.isPaid == true select S).ToList().OrderByDescending(a => a.DonationAmnt); ;
                    if (donations.Any())
                    {
                        CampaignDonation donationval = new CampaignDonation();
                        decimal RaisedAmt = 0;
                        long RaisedBy = 0;

                        foreach (var dntion in donations)
                        {
                            RaisedAmt = RaisedAmt + dntion.DonationAmnt;
                            RaisedBy++;
                            donationList.Add(new CampaignDonation() { DonatedBy = dntion.DonatedBy, isAnanymous = dntion.isAnanymous.Value, DonationAmnt = dntion.DonationAmnt, DonatedOn = dntion.DonatedOn.Value });
                        }

                        decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                        var raisedPerc = (RaisedAmt / Model.CampaignTargetMoney) * 100;
                        Model.CampaignDonationList.AddRange(donationList);
                        Model.RaisedAmount = RaisedAmt;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);
                    }

                }
                var cntry = GetUserCountryByIp();
                Model.CountryCode = cntry != null ? cntry.Name : "IN";
                Model.CurrencyCode = cntry != null ? cntry.CurrencySymbol : "";

                return Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public CampaignMainViewModel GetCamapignShares(int Id)
        {
            try
            {
                int UserId = UserSession.HasSession != false ? UserSession.UserId : 0;
                CampaignMainViewModel Model = new CampaignMainViewModel();

                var shares = (from S in Entity.Tbl_Shares where S.StoryId == Id select S).ToList().Count();
                Model.sharecount = shares;



                return Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public CampaignMainViewModel GetCamapignForView(int Id)
        {
            try
            {
                int UserId = UserSession.HasSession != false ? UserSession.UserId : 0;
                CampaignMainViewModel Model = new CampaignMainViewModel();
                var res = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                if (res != null)
                {
                    Model.Id = res.Id;
                    Model.StoryCategory = (StoryCategory)res.Category;
                    Model.CategoryName = Model.StoryCategory.DisplayName();
                    Model.CampainOrganizer = new CampainOrganizerViewModel();
                    Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                    Model.CampaignTitle = res.Title != null ? res.Title : "";
                    Model.UserId = res.UserId;
                    if (Model.UserId == UserId)
                        Model.loggedinUser = true;

                    var user = GetUserDetailbyId(UserId);
                    if (user.IsNGO && user.CanEndorse)
                        Model.isNGOUser = true;
                    Model.Status = res.Status;
                    Model.CreatedBy = res.CreatedBy;
                    Model.CreatedOn = res.CreatedOn;
                    Model.UpdatedBy = res.UpdatedBy;
                    Model.UpdatedOn = res.UpdatedOn;
                    Model.BGroupName = res.BGroupName;
                    Model.NGOName = res.NGOName;
                    Model.BName = res.BName;
                    Model.BeneficiaryType = (BeneficiaryType)Enum.Parse(typeof(BeneficiaryType), Enum.GetName(typeof(BeneficiaryType), res.BeneficiaryType.Value), true);

                    Model.CampaignTargetMoney = res.TargetAmount.Value;
                    Model.CampaignTargetMoneyType = res.MoneyType;
                    int i = 0;
                    var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails where S.StoryId == Id select S).FirstOrDefault();
                    if (Beneficiary != null)
                    {
                        CampainOrganizerViewModel ben = new CampainOrganizerViewModel();
                        ben = createViewModelOrganizer(Beneficiary);
                        ben.Id = Beneficiary.Id;
                        Model.CampainOrganizer = ben;
                        Model.Latitude = ben.Latitude;
                        Model.Longitude = ben.longitude;
                        if (ben.BDisplayPic != null)
                        {
                            Files files = new Files();
                            files.File = ben.BDisplayPic;
                            files.FileName = ben.BDisplayPicName;
                            files.ContentType = "";
                            files.Index = 0;
                            Model.Files.Add(files);
                            i++;
                        }
                    }
                    //var resFiles = (from F in Entity.Tbl_StoriesAttachment where F.StoryId == Id select F).ToList();
                    //if (resFiles.Any())
                    //{

                    //    foreach (var fItem in resFiles)
                    //    {
                    //        Files files = new Files();
                    //        files.File = fItem.MediaFile;
                    //        files.FileName = fItem.FileName;
                    //        files.ContentType = fItem.ContentType;
                    //        files.AttId = fItem.Id;
                    //        files.Index = i;
                    //        Model.Files.Add(files);
                    //        i++;
                    //    }
                    //}




                    var description = (from S in Entity.Tbl_CampaignDescription where S.StoryId == Id select S).FirstOrDefault();
                    if (description != null)
                    {
                        CampaignDescription desc = new CampaignDescription();
                        desc.StoryDescription = description.storyDescription;
                        desc.Id = description.Id;
                        desc.StripedDescription = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                        Model.campaignDescription = desc;

                    }

                    var updates = (from S in Entity.Tbl_CampaignDescriptionUpdates where S.StoryId == Id select S).ToList();
                    if (updates.Any())
                    {
                        List<CampaignUpdates> updatesList = new List<CampaignUpdates>();
                        foreach (var update in updates)
                        {
                            if (update != null)
                            {
                                CampaignUpdates Updt = new CampaignUpdates();
                                CampaignDescription desc = new CampaignDescription();
                                desc.StoryDescription = update.storyDescription;
                                desc.Id = update.Id;

                                desc.StripedDescription = update.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                                Updt.UpdateDescription = desc;
                                Updt.updatedOn = update.CreatedOn.Value;

                                var uptFiles = (from F in Entity.Tbl_UpdatesAttachment where F.StoryId == Id && F.UpdateId == update.Id select F).ToList();
                                if (uptFiles.Any())
                                {

                                    List<Files> filesList = new List<Files>();
                                    foreach (var fItem in uptFiles)
                                    {
                                        Files files = new Files();
                                        files.File = fItem.MediaFile;
                                        files.FileName = fItem.FileName;
                                        files.ContentType = fItem.ContentType;
                                        files.AttId = fItem.Id;
                                        files.updtId = fItem.UpdateId;
                                        filesList.Add(files);
                                    }
                                    Updt.Files = filesList;
                                }
                                updatesList.Add(Updt);
                            }
                        }
                        Model.Updates = updatesList;
                    }

                    var Cmnts = (from S in Entity.Tbl_ParentComment where S.StoryId == Id select S).OrderByDescending(a => a.CommentDate).ToList();
                    if (Cmnts.Any())
                    {
                        List<CommentsVM> cmntModel = new List<CommentsVM>();
                        foreach (var Cmnt in Cmnts)
                        {
                            CommentsVM cmt = new CommentsVM();
                            cmt.CommentMsg = Cmnt.CommentMessage;
                            cmt.CommentedDate = Cmnt.CommentDate.Value;
                            cmt.campaignId = Cmnt.StoryId;
                            cmt.Users = GetUserDetailbyId(Cmnt.UserId);
                            cmt.SubComments = new List<SubCommentsVM>();
                            cmntModel.Add(cmt);
                        }
                        Model.Comments.AddRange(cmntModel);
                        Model.CommentCount = Cmnts.Count();
                    }

                    var endorseresult = (from S in Entity.Tbl_Endorse
                                         where S.CampaignId == Id
                                         select S).ToList();
                    ENdorsementList list = new ENdorsementList();
                    List<Endorsement> individuallist = new List<Endorsement>();
                    foreach (var val in endorseresult)
                    {
                        if (val.NGOId == UserSession.UserId)
                            Model.isCtNGOEndorsed = true;

                        var userest = GetUserDetailbyId(val.NGOId);
                        Endorsement newval = new Endorsement();
                        newval.NGOId = val.NGOId;
                        newval.NGOName = userest.DisplayName;
                        newval.endorsementId = val.EndorseID;
                        newval.NGOAddress = userest.NGOAddress != null ? userest.NGOAddress : "";

                        var typeM = (StoryCategory)(userest.NGOSector);
                        var typeNmae = typeM.DisplayName();
                        newval.NGOSector = typeNmae;

                        var b = (NGOType)(userest.NGOSector);
                        var typeNm = b.DisplayName();
                        newval.NGOType = typeNm;
                        individuallist.Add(newval);
                    }
                    list.CampaignId = Id;
                    list.TotalCount = endorseresult.Count();
                    list.EndorseList = individuallist;
                    Model.EndorsementsList = list;
                    var shares = (from S in Entity.Tbl_Shares where S.StoryId == Id select S).ToList().Count();
                    Model.sharecount = shares;
                    var likes = (from S in Entity.Tbl_Like where S.StoryId == Id select S).ToList().Count();
                    Model.LikeCount = likes;
                    //  int UserId = UserSession.UserId;
                    var likebyuser = (from S in Entity.Tbl_Like
                                      where S.StoryId == Id && S.UserId == UserId
                                      select S).FirstOrDefault();
                    if (likebyuser != null)
                        Model.isLiked = true;
                    List<CampaignDonation> donationList = new List<CampaignDonation>();
                    var donations = (from S in Entity.Tbl_CampaignDonation where S.StoryId == Id && S.isPaid == true select S).ToList().OrderByDescending(a => a.DonationAmnt); ;
                    if (donations.Any())
                    {
                        CampaignDonation donationval = new CampaignDonation();
                        decimal RaisedAmt = 0;
                        long RaisedBy = 0;

                        foreach (var dntion in donations)
                        {
                            RaisedAmt = RaisedAmt + dntion.DonationAmnt;
                            RaisedBy++;
                            donationList.Add(new CampaignDonation() { DonatedBy = dntion.DonatedBy, isAnanymous = dntion.isAnanymous.Value, DonationAmnt = dntion.DonationAmnt, DonatedOn = dntion.DonatedOn.Value });
                        }

                        decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                        var raisedPerc = (RaisedAmt / Model.CampaignTargetMoney) * 100;
                        Model.CampaignDonationList.AddRange(donationList);
                        Model.RaisedAmount = RaisedAmt;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);
                    }

                }
                var cntry = GetUserCountryByIp();
                Model.CountryCode = cntry != null ? cntry.Name : "IN";
                Model.CurrencyCode = cntry != null ? cntry.CurrencySymbol : "";

                return Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<Files> GetCamapignImageForView(int Id)
        {
            try
            {
                int UserId = UserSession.HasSession != false ? UserSession.UserId : 0;
                List<Files> fileList = new List<Files>();
                int i = 0;
                var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails where S.StoryId == Id select S).FirstOrDefault();
                if (Beneficiary != null)
                {

                    if (Beneficiary.DP != null)
                    {
                        Files files = new Files();
                        files.File = Beneficiary.DP;
                        files.FileName = Beneficiary.DPName;
                        files.ContentType = "";
                        files.Index = 0;
                        fileList.Add(files);
                        i++;
                    }
                }
                var res = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                if (res != null)
                {


                    var resFiles = (from F in Entity.Tbl_StoriesAttachment where F.StoryId == Id select F).ToList();
                    if (resFiles.Any())
                    {

                        foreach (var fItem in resFiles)
                        {
                            Files files = new Files();
                            files.File = fItem.MediaFile;
                            files.FileName = fItem.FileName;
                            files.ContentType = fItem.ContentType;
                            files.AttId = fItem.Id;
                            files.Index = i;
                            fileList.Add(files);
                            i++;
                        }
                    }

                }
                return fileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public int AddCampaignDonation(CampaignDonation viewModel)
        {
            try
            {

                if (viewModel.id == 0)
                {
                    Tbl_CampaignDonation donation = new Tbl_CampaignDonation();
                    donation.StoryId = viewModel.StoryId;
                    donation.DonatedBy = viewModel.IdentityName;
                    donation.CountryId = viewModel.countryId;
                    donation.DonatedOn = DateTime.Now;
                    donation.DonationAmnt = viewModel.DonationAmnt;
                    donation.Email = viewModel.EMail;
                    donation.isAnanymous = viewModel.isAnanymous;
                    donation.PinCode = viewModel.pincode;

                    donation.Status = true;
                    donation.UpdatedBy = UserSession.UserName;
                    donation.UpdatedOn = DateTime.UtcNow;



                    Entity.Tbl_CampaignDonation.Add(donation);
                    Entity.SaveChanges();
                    var NewBEneficiaryId = donation.Id;

                    return NewBEneficiaryId;
                }
                else
                {
                    var donation = (from S in Entity.Tbl_CampaignDonation where S.Id == viewModel.id select S).FirstOrDefault();

                    if (donation != null)
                    {
                        donation.StoryId = viewModel.StoryId;
                        donation.DonatedBy = viewModel.IdentityName;
                        donation.CountryId = viewModel.countryId;
                        donation.DonatedOn = DateTime.Now;
                        donation.DonationAmnt = viewModel.DonationAmnt;
                        donation.Email = viewModel.EMail;
                        donation.isAnanymous = viewModel.isAnanymous;
                        donation.PinCode = viewModel.pincode;

                        donation.Status = true;
                        donation.UpdatedBy = UserSession.UserName;
                        donation.UpdatedOn = DateTime.UtcNow;
                        Entity.SaveChanges();
                        return donation.Id;
                    }
                    else { return 0; }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public int UpdateCampaignDonationorder(int id, string orderId, string payementID, string signature)
        {
            try
            {


                var donation = (from S in Entity.Tbl_CampaignDonation where S.Id == id select S).FirstOrDefault();

                if (donation != null)
                {
                    donation.razorpay_order_id = orderId;
                    donation.razorpay_payment_id = payementID;
                    donation.razorpay_signature = signature;
                    //if (string.IsNullOrEmpty(payementID))
                    //    donation.isPaid = true;
                    donation.Status = true;
                    donation.UpdatedBy = UserSession.UserName;
                    donation.UpdatedOn = DateTime.UtcNow;
                    Entity.SaveChanges();
                    return donation.Id;
                }
                else { return 0; }

            }
            catch (Exception ex) { throw ex; }

        }
        public CampaignMainViewModel UpdateCampaignDonationSuccess(string razorpay_order_id, string razorpay_payment_id, string razorpay_signature)
        {
            try
            {


                var donation = (from S in Entity.Tbl_CampaignDonation where S.razorpay_order_id == razorpay_order_id select S).FirstOrDefault();

                if (donation != null)
                {
                    donation.razorpay_order_id = razorpay_order_id;
                    donation.razorpay_payment_id = razorpay_payment_id;
                    donation.razorpay_signature = razorpay_signature;
                    if (!string.IsNullOrEmpty(razorpay_payment_id))
                        donation.isPaid = true;
                    donation.Status = true;
                    donation.UpdatedBy = UserSession.UserName;
                    donation.UpdatedOn = DateTime.UtcNow;
                    Entity.SaveChanges();
                    var m = GetCamapign(donation.StoryId);
                    return m;
                }
                else { return new CampaignMainViewModel(); }

            }
            catch (Exception ex) { throw ex; }

        }
        public RegisterModel CreatenSendOTP(string UserName, string DisplayName)
        {
            try
            {
                RegisterModel Model = new RegisterModel();
                string SecurityCode = "";
                string Username = UserName.ToLower();
                var result = (from S in Entity.Tbl_User where (S.UserName == Username && S.IsActive == true && S.IsSpamUser == false) select S).FirstOrDefault();
                if (result != null)
                {
                    Model.UserAlreadyExists = true;
                }
                else
                {
                    var res = (from S in Entity.Tbl_UserAuthentication where S.UserName == Username select S).ToList();
                    DateTime ChecktimeUTCTime = DateTime.UtcNow.AddMinutes(-20);
                    var ValidModel = res.Where(c => c.CreatedTime >= ChecktimeUTCTime).FirstOrDefault();

                    if (ValidModel != null)
                    {
                        Model.IsSecurityTokenGenerated = true;
                        SecurityCode = ValidModel.SecurityCode.ToString();
                    }
                    else
                    {
                        Model.IsSecurityTokenGenerated = true;
                        Random generator = new Random();
                        var resAuth = (from S in Entity.Tbl_UserAuthentication where S.UserName.ToLower() == UserName.ToLower() select S).ToList();
                        if (resAuth.Any())
                        {
                            Entity.Tbl_UserAuthentication.RemoveRange(resAuth);
                            Entity.SaveChanges();
                        }

                        Tbl_UserAuthentication AuthModel = new Tbl_UserAuthentication();
                        AuthModel.UserName = UserName.ToLower();
                        AuthModel.SecurityCode = generator.Next(100000, 1000000);
                        SecurityCode = AuthModel.SecurityCode.ToString();
                        AuthModel.CreatedTime = DateTime.UtcNow;
                        Entity.Tbl_UserAuthentication.Add(AuthModel);
                        Entity.SaveChanges();
                    }
                    SendMail(UserName, DisplayName, SecurityCode);
                }
                return Model;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public string RegisterUserWIthOTP(RegisterModel model)
        {
            try
            {
                string returnVal = "";

                if (model.SecurityToken != null && !string.IsNullOrEmpty(model.SecurityToken))
                {
                    var res = (from S in Entity.Tbl_UserAuthentication where S.UserName == model.UserName select S).ToList();
                    //  DateTime ChecktimeUTCTime = DateTime.UtcNow.AddMinutes(-20);
                    if (res.Any())
                    {
                        {
                            var ValidModel = res.FirstOrDefault();
                            if (ValidModel != null)
                            {
                                if (ValidModel.SecurityCode.ToString() == model.SecurityToken)
                                {
                                    var result = (from S in Entity.Tbl_User where (S.UserName == model.UserName && S.IsActive == false && S.IsSpamUser == true) select S).FirstOrDefault();
                                    var existingUser = (from S in Entity.Tbl_User
                                                        where S.Id == result.Id
                                                        select S).FirstOrDefault();


                                    existingUser.IsSpamUser = false;
                                    existingUser.IsActive = true;
                                    Entity.SaveChanges();
                                    returnVal = "Successfully Updated!";

                                }
                                else
                                {
                                    returnVal = "OTP Doesn't Match. Please try Again !";

                                }
                            }
                        }
                    }
                }
                else
                {
                    returnVal = "Please Input the valid OTP !";

                }
                return returnVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string CheckOTP(InputRegisterModel model)
        {
            try
            {
                string returnVal = "";

                if (model.SecurityToken != null && !string.IsNullOrEmpty(model.SecurityToken))
                {
                    var res = (from S in Entity.Tbl_UserAuthentication where S.UserName == model.UserName select S).ToList();
                    //  DateTime ChecktimeUTCTime = DateTime.UtcNow.AddMinutes(-20);
                    if (res.Any())
                    {
                        {
                            var ValidModel = res.FirstOrDefault();
                            if (ValidModel != null)
                            {
                                if (ValidModel.SecurityCode.ToString() == model.SecurityToken)
                                {
                                    var result = (from S in Entity.Tbl_User where (S.UserName == model.UserName && S.IsActive == false && S.IsSpamUser == true) select S).FirstOrDefault();
                                    var existingUser = (from S in Entity.Tbl_User
                                                        where S.Id == result.Id
                                                        select S).FirstOrDefault();


                                    existingUser.IsSpamUser = false;
                                    existingUser.IsActive = true;
                                    Entity.SaveChanges();
                                    returnVal = "Successfully Updated!";

                                }
                                else
                                {
                                    returnVal = "OTP Doesn't Match. Please try Again !";

                                }
                            }
                        }
                    }
                }
                else
                {
                    returnVal = "Please Input the valid OTP !";

                }
                return returnVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserModel ToSocialregisterModel(SocialRegisterModel reg)
        {
            UserModel um = new UserModel();
            var result = (from S in Entity.Tbl_User where S.UserName.ToLower() == reg.UserName.ToLower() && S.BasedOut.ToLower() == reg.provider.ToLower() select S).FirstOrDefault();
            if (result == null)
            {
                Tbl_User NewUser = new Tbl_User();
                NewUser.UserName = reg.UserName.ToLower();
                NewUser.Password = "";
                NewUser.IsAdmin = false;
                NewUser.IsSpamUser = false;
                NewUser.IsActive = true;
                NewUser.IsAcLocked = false;
                string initial = "";
                initial = !(string.IsNullOrEmpty(reg.FirstName)) ? reg.FirstName : "";
                initial = initial + " " + (!(string.IsNullOrEmpty(reg.LastName)) ? reg.LastName : "");
                NewUser.Name = initial;
                NewUser.FirstName = reg.FirstName;
                NewUser.LastName = reg.LastName;
                NewUser.DPPath = reg.DPPath;
                NewUser.BasedOut = reg.provider;
                NewUser.CurrentLoginDate = DateTime.Now;
                NewUser.LastLoginDate = DateTime.Now;
                Entity.Tbl_User.Add(NewUser);
                Entity.SaveChanges();
                um = GetUserDetail(NewUser.Id);
            }
            else
            {
                result.LastLoginDate = result.CurrentLoginDate;
                result.CurrentLoginDate = DateTime.UtcNow;

                Entity.SaveChanges();
                um = GetUserDetail(result.Id);
            }
            return um;
        }

        public UserModel GetUserDetail(int UserId)
        {
            try
            {
                UserModel ModelList = new UserModel();
                var res = (from S in Entity.Tbl_User where S.Id == UserId select S).FirstOrDefault();
                if (res != null)
                {
                    ModelList.Id = res.Id;
                    ModelList.UserName = res.UserName;
                    ModelList.DisplayName = res.Name;
                    ModelList.DPPAth = res.DPPath;
                    ModelList.FirstName = res.FirstName;
                    ModelList.LastName = res.LastName;
                    ModelList.IsAdmin = res.IsAdmin;
                    ModelList.IsNGO = res.IsNGO != null ? res.IsNGO.Value : false;
                    ModelList.CanEndorse = res.CanEndorse != null ? res.CanEndorse.Value : false;
                }

                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}