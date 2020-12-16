using firstWebAPI.DataLayer;
using GivingActuallyAPI.Models;
using GivingActuallyAPI.Models.HelperModels;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using static GivingActuallyAPI.Models.Helper;

namespace firstWebAPI.Services
{
    public class UserService
    {
        GivingActuallyEntities Entity = new GivingActuallyEntities();
        #region Get the campaigns according to the category And page
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
                    ModelList.IsActive = res.IsActive;
                    ModelList.IsAdmin = res.IsAdmin;
                    ModelList.IsAcLocked = res.IsAcLocked;
                    ModelList.CurrentLoginDate = res.CurrentLoginDate;
                    ModelList.LastLoginTime = res.LastLoginDate;
                    ModelList.IsSpamUser = res.IsSpamUser;
                    if (ModelList.IsNGO)
                    {
                        var NGO = (from S in Entity.tbl_UserNGOEndorsement where S.UserId == UserId select S).FirstOrDefault();
                        if (NGO != null)
                        {
                            if (ModelList.CanEndorse)
                            {

                                ModelList.NGOType = NGO.NGOType != null ? NGO.NGOType.Value : 0;
                                ModelList.NGOSector = NGO.NGOSector != null ? NGO.NGOSector.Value : 0;
                                if (NGO.NGOSector != null)
                                {
                                    var sector = (StoryCategory)NGO.NGOSector.Value;
                                    ModelList.NGOSectorName = sector.DisplayName();
                                }
                                if (NGO.NGOType != null)
                                {
                                    var Category = (NGOType)NGO.NGOType.Value;
                                    ModelList.NGOTypeName = Category.DisplayName();
                                }
                            }
                            ModelList.cityName = "";
                            ModelList.countryName = "";
                            ModelList.stateName = "";

                            ModelList.NGOAddress = "";
                            int state = NGO.StateId != null ? NGO.StateId.Value : 0;
                            int city = NGO.CityId != null ? NGO.CityId.Value : 0;
                            int country = NGO.CountryId != null ? NGO.CountryId.Value : 0;
                            var NGOAddress = "";
                            //var CountryName = (from S in Entity.Tbl_Countries where S.CountryId == country select S).FirstOrDefault();
                            //if (CountryName != null)
                            //{
                            //    ModelList.countryName = CountryName.CountryName;
                            //    NGOAddress = NGOAddress + CountryName.CountryName;
                            //}
                            //var stateName = (from S in Entity.Tbl_States where S.StateId == state select S).FirstOrDefault();
                            //if (stateName != null)
                            //{
                            //    NGOAddress = NGOAddress + stateName.StateName;
                            //    ModelList.stateName = stateName.StateName;
                            //}
                            //var CityName = (from S in Entity.Cities where S.CityId == city select S).FirstOrDefault();
                            //if (CityName != null)
                            //{
                            //    NGOAddress = NGOAddress + CityName.CityName;
                            //    ModelList.cityName = CityName.CityName;
                            //}
                            ModelList.countryName = NGO.CountryName;
                            NGOAddress = NGOAddress + NGO.CountryName;
                            NGOAddress = NGOAddress + NGO.StateName;
                            ModelList.stateName = NGO.StateName;
                            NGOAddress = NGOAddress + NGO.CityName;
                            ModelList.cityName = NGO.CityName;
                            ModelList.NGOAddress = NGOAddress;
                            ModelList.RegisterationNo = NGO.NGORegisterationNO;
                            ModelList.Registeredat = NGO.NGORegAt;

                        }

                    }

                }

                return ModelList;
            }
            catch (Exception ex)
            { throw ex; }


        }

        public UserModel GetUserDetailbyUserId(Tbl_User usr)
        {
            try
            {
        
                UserModel ModelList = new UserModel();
                var res = usr;
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
                    ModelList.IsActive = res.IsActive;
                    ModelList.IsAdmin = res.IsAdmin;
                    ModelList.IsAcLocked = res.IsAcLocked;
                    ModelList.CurrentLoginDate = res.CurrentLoginDate;
                    ModelList.LastLoginTime = res.LastLoginDate;
                    ModelList.IsSpamUser = res.IsSpamUser;
                    if (ModelList.IsNGO)
                    {
                        var NGO = (from S in Entity.tbl_UserNGOEndorsement where S.UserId == res.Id select S).FirstOrDefault();
                        if (NGO != null)
                        {
                            if (ModelList.CanEndorse)
                            {

                                ModelList.NGOType = NGO.NGOType != null ? NGO.NGOType.Value : 0;
                                ModelList.NGOSector = NGO.NGOSector != null ? NGO.NGOSector.Value : 0;
                                if (NGO.NGOSector != null)
                                {
                                    var sector = (StoryCategory)NGO.NGOSector.Value;
                                    ModelList.NGOSectorName = sector.DisplayName();
                                }
                                if (NGO.NGOType != null)
                                {
                                    var Category = (NGOType)NGO.NGOType.Value;
                                    ModelList.NGOTypeName = Category.DisplayName();
                                }
                            }
                            ModelList.cityName = "";
                            ModelList.countryName = "";
                            ModelList.stateName = "";

                            ModelList.NGOAddress = "";
                            int state = NGO.StateId != null ? NGO.StateId.Value : 0;
                            int city = NGO.CityId != null ? NGO.CityId.Value : 0;
                            int country = NGO.CountryId != null ? NGO.CountryId.Value : 0;
                            var NGOAddress = "";
                            ModelList.countryName = NGO.CountryName;
                            NGOAddress = NGOAddress + NGO.CountryName;
                            NGOAddress = NGOAddress + NGO.StateName;
                            ModelList.stateName = NGO.StateName;
                            NGOAddress = NGOAddress + NGO.CityName;
                            ModelList.cityName = NGO.CityName;
                            ModelList.NGOAddress = NGOAddress;
                            ModelList.RegisterationNo = NGO.NGORegisterationNO;
                            ModelList.Registeredat = NGO.NGORegAt;

                        }

                    }

                }

                return ModelList;
            }
            catch (Exception ex)
            { throw ex; }


        }

        public string uploadUserImage(HttpPostedFile file)
        {

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var filepath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/images/UserImages/ "));
                    if (!Directory.Exists(filepath))
                        System.IO.Directory.CreateDirectory(filepath);
                    string imageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                    if (!(file.ContentType == "video/mp4"))
                    {
                        imageName = imageName + ".png";
                    }



                    var TestUrl = System.IO.Path.Combine(filepath, imageName);
                    file.SaveAs(System.IO.Path.Combine(TestUrl));

                    string key = ConfigurationManager.AppSettings["BasicUrl"];
                    var Path = (key + "/images/UserImages/" + imageName);
                    return Path;
                }

                else
                { return ""; }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserList GetUserDetails(string Status = "All", int page = 1, int page_size = 50)
        {
            try
            {

                UserList ModelList = new UserList();
                var result = (from S in Entity.Tbl_User select S).ToList();
                var res = result.OrderByDescending(a => a.CreatedOn)
                       .Skip((page - 1) * page_size)
                       .Take(page_size).ToList();
                List<UserModel> list = new List<UserModel>();
                if (res.Any())
                {
                    foreach (var user in res)
                    {
                        UserModel usr = new UserModel();
                        usr = GetUserDetailbyUserId(user);
                        list.Add(usr);
                    }
                    ModelList.UserLists = new List<UserModel>();
                    ModelList.UserLists.AddRange(list);
                }
                ModelList.TotalUsers = result.Count();
                ModelList.pageindex = page;

                return ModelList;
            }
            catch (Exception ex)
            { throw ex; }

        }

        public int UpdateUser(InputRegisterModel inputModel)
        {
            try
            {
                int USerId = inputModel.UserId;
                var ExistingUser = (from S in Entity.Tbl_User where S.Id == inputModel.UserId select S).FirstOrDefault();
                if (ExistingUser != null)
                {

                    ExistingUser.IsNGO = inputModel.IsNGO;
                    ExistingUser.DPPath = !(string.IsNullOrEmpty(inputModel.DPPath)) ? inputModel.DPPath : "";
                    ExistingUser.FirstName = !(string.IsNullOrEmpty(inputModel.FirstName)) ? inputModel.FirstName : "";
                    ExistingUser.LastName = !(string.IsNullOrEmpty(inputModel.LastName)) ? inputModel.LastName : "";
                    string initial = "";
                    initial = !(string.IsNullOrEmpty(inputModel.FirstName)) ? inputModel.FirstName : "";
                    initial = initial + (!(string.IsNullOrEmpty(inputModel.LastName)) ? inputModel.LastName : "");

                    ExistingUser.Name = initial;
                    ExistingUser.CanEndorse = inputModel.canEndorse;
                    ExistingUser.UpdatedOn = DateTime.Now;
                    Entity.SaveChanges();
                    //ExistingUser.UpdatedBy=inputModel.
                    if (inputModel.IsNGO)
                        AddNGOModelUser(inputModel);
                    else
                    {
                        RemoveNGOUser(inputModel);
                    }

                }
                return USerId;
            }
            catch (Exception ex)
            { throw ex; }
        }

        public bool RemoveNGOUser(InputRegisterModel reg)
        {
            try
            {
                UserModel um = new UserModel();
                var result = (from S in Entity.tbl_UserNGOEndorsement where S.UserId == reg.UserId select S).FirstOrDefault();
                if (result != null)
                {
                    Entity.tbl_UserNGOEndorsement.Remove(result);
                    Entity.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            { throw ex; }
        }
        public bool AddNGOModelUser(InputRegisterModel reg)
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
        #endregion
    }
}