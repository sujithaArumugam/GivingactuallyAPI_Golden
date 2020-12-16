using firstWebAPI.DataLayer;
using GivingActuallyAPI.Models;
using GivingActuallyAPI.Models.HelperModels;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using static GivingActuallyAPI.Models.Helper;

namespace GivingActuallyAPI.Services
{
    public class CampaignService
    {
        GivingActuallyEntities Entity = new GivingActuallyEntities();
        #region Get the campaigns according to the category And page


        public async Task<UserCampaignModelsList> GetUserCampaigns(int CategoryId = -1, int page = 1, int page_size = 12, int userId = 0)
        {
            try
            {
                int maxRows = page_size;
                UserCampaignModelsList ModelList = new UserCampaignModelsList();


                var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                            where ((CategoryId != -1 ? item.Category == CategoryId : true) && item.UserId == userId)
                            orderby item.CreatedOn descending
                            select item.Id;

                var CampaignResult = await query.ToListAsync();


                int total = CampaignResult.Count();
                var CampaignRes = CampaignResult.Skip((page - 1) * maxRows).Take(maxRows).ToList();

                List<int> CampaignIdList = new List<int>();
                CampaignIdList.AddRange(CampaignRes.Select(a => a));


                if (CampaignIdList.Any())
                {

                    foreach (var item in CampaignIdList)
                    {
                        UserCampaignModel Model = new UserCampaignModel();
                        Model = GetCamapignsForUsersListView(item);
                        ModelList.CampaignLists.Add(Model);
                    }
                }
                double pageCount = (double)((decimal)total / Convert.ToDecimal(maxRows));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);
                ModelList.TotalCampaigns = total;
                ModelList.CurrentPageIndex = page;
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<UserStatisticsModel> GetUserStatistics(int userId = 0)
        {
            try
            {

                UserStatisticsModel ModelList = new UserStatisticsModel();
                var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                            where ((item.Status == true) && (item.IsApprovedbyAdmin == true) && item.UserId == userId)
                            select new { item.Id, item.TargetAmount };

                var CampaignResult = await query.ToListAsync();


                int totalCampaigns = CampaignResult.Count();
                var totalAmount = CampaignResult.Sum(a => a.TargetAmount.Value);

                ModelList.TotalCampaigns = totalCampaigns;
                ModelList.TotalGoals = totalAmount;

                List<int> CampaignIdList = new List<int>();
                CampaignIdList = CampaignResult.Select(a => a.Id).ToList();
                List<CampaignDonation> donationList = new List<CampaignDonation>();
                var donations = (from S in Entity.Tbl_CampaignDonation.AsNoTracking() where CampaignIdList.Contains(S.Id) && S.isPaid == true select S).ToList();
                if (donations.Any())
                {

                    decimal RaisedAmt = 0;
                    List<string> donrList = new List<string>();
                    foreach (var dntion in donations)
                    {
                        RaisedAmt = RaisedAmt + dntion.DonationAmnt;
                        donrList.Add(dntion.Email);
                    }
                    var TotalRaisedAmount = RaisedAmt;
                    var TotalRaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;
                    ModelList.TotalRecievedMoney = RaisedAmt;
                    ModelList.TotalDonors = TotalRaisedBy;
                }
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<MyDonationsList> GetUserDonations(string userName = "")
        {
            try
            {
                MyDonationsList ModelList = new MyDonationsList();



                ModelList.MyDonationList = new List<MyDonations>();
                List<int> CampaignIdList = new List<int>();

                var donations = await (from S in Entity.Tbl_CampaignDonation.AsNoTracking()
                                       where S.Email.ToLower() == userName.ToLower() && S.isPaid == true
                                       select new
                                       {
                                           S.StoryId
,
                                           S.DonatedBy,
                                           S.Email,
                                           S.DonatedOn,
                                           S.DonationAmnt,
                                           S.isAnanymous,
                                           S.PhoneNumber,
                                           S.PlaceName
                                       }).ToListAsync()
                                 ;
                if (donations.Any())
                {

                    decimal RaisedAmt = 0;
                    List<string> donrList = new List<string>();
                    foreach (var dntion in donations)
                    {
                        MyDonations Model = new MyDonations();
                        RaisedAmt = RaisedAmt + dntion.DonationAmnt;

                        ModelList.MyDonationList.Add(new MyDonations()
                        {
                            CampaignId = dntion.StoryId,
                            EMail = dntion.Email,
                            PhNo = dntion.PhoneNumber,
                            PlaceName = dntion.PlaceName,
                            DonatedBy = dntion.DonatedBy,
                            isAnanymous = dntion.isAnanymous.Value,
                            DonationAmnt = dntion.DonationAmnt,
                            DonatedOn = dntion.DonatedOn.Value
                        });

                        donrList.Add(dntion.StoryId.ToString());
                    }
                    var TotalRaisedAmount = RaisedAmt;
                    var TotalRaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;
                    ModelList.TotalDonationAmount = RaisedAmt;
                    ModelList.TotalDonations = TotalRaisedBy;
                }
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<CampaignModelsList> GetTopCampaigns(int CategoryId = -1, int page = 1, int page_size = 6)
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();


                var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                            where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                            orderby item.CountUsage descending
                            select item.Id;

                var CampaignRes = await query.Skip((page - 1) * maxRows).Take(maxRows).ToListAsync();
                List<int> CampaignIdList = new List<int>();
                CampaignIdList.AddRange(CampaignRes.Select(a => a));
                if (CampaignIdList.Any())
                {
                    var re = await GetCampaignsForList_Sp(CampaignIdList);
                    ModelList = re;
                }

                ModelList.CurrentPageIndex = page;
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public bool UpdateTopCampaigns()
        {
            try
            {
                var re = Entity.GetTopCampaigns();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<CampaignModelsList> GetsearchCampaigns(string searchText = "", int page = 1, int page_size = 6)
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();
                var matches = Entity.GetSearchCampaigns(searchText);

                var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                            where item.IsApprovedbyAdmin && item.Status
                            orderby item.Category
                            select item.Id;

                var CampaignRes = await query.Skip((page - 1) * maxRows).Take(maxRows).ToListAsync();
                List<int> CampaignIdList = new List<int>();
                CampaignIdList.AddRange(CampaignRes.Select(a => a));
                if (CampaignIdList.Any())
                {
                    var re = await GetCampaignsForListtest2(CampaignIdList);
                    ModelList = re;
                }

                ModelList.CurrentPageIndex = page;
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool DeActivateCampaigns()
        {

            try
            {
                var query = Entity.DeactivateCampaigns();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> FirstNOtifyDeActCampaigns()
        {

            try
            {
                var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                            where item.IsApprovedbyAdmin && item.Status && DbFunctions.DiffDays(DateTime.Now, item.TargetDate.Value) == 2
                            orderby item.CreatedOn descending
                            select new { item.Id, item.TargetDate, item.Title, item.BName, item.CreatedBy, item.CreatedUserName };
                var CampaignRes = await query.ToListAsync();
                foreach (var result in CampaignRes)
                {
                    // var text = "'https://givingactually.com/fundraiser/" + result.Id.ToString();

                    var body = string.Format(@"Dear {0},<br /><br />The fundraiser titled <strong>{1}</strong> is about to expire in two days. please do the extention of target date or withdrawal as per the current situation inorder to avoid the deactivation.<a href='https://givingactually.com/fundraiser/{2}'> Click here to view fundraiser</a> <br /><br />Thank You.<br> <br/>please ignore this , incase if you have already started the appropriate action.<br> <br/>please reach out to customer support by writing to 'info@givingactually.com'", result.CreatedUserName, result.Title, result.Id);//.DisplayName, DisplayName);

                    SendMailBase(result.CreatedBy, result.CreatedUserName, "Fundraiser Expiry Notification", body);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> SecondNOtifyDeActCampaigns()
        {

            try
            {
                var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                            where item.IsApprovedbyAdmin && item.Status && DbFunctions.DiffDays(DateTime.Now, item.TargetDate.Value) == 1
                            orderby item.CreatedOn descending
                            select new { item.Id, item.TargetDate, item.Title, item.BName, item.CreatedBy, item.CreatedUserName };
                var CampaignRes = await query.ToListAsync();
                foreach (var result in CampaignRes)
                {


                    var body = string.Format(@"Dear {0},<br /><br />The fundraiser titled <strong>{1}</strong> is about to expire in a day. please do the extention of target date or withdrawal as per the current situation inorder to avoid the deactivation.<a href='https://givingactually.com/fundraiser/{2}'> Click here to view fundraiser</a> <br /><br />Thank You.<br> <br/>please ignore this , incase if you have already started the appropriate action.<br> <br/>please reach out to customer support by writing to 'info@givingactually.com'", result.CreatedUserName, result.Title, result.Id);//.DisplayName, DisplayName);

                    SendMailBase(result.CreatedBy, result.CreatedUserName, "Fundraiser Expiry Notification", body);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<CampaignModelsList> GetCampaignsAsync(int CategoryId = -1, int page = 1, int page_size = 12,
          string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0")
        {
            try
            {
                int maxRows = page_size;
                var skip = (page - 1) * maxRows;
                CampaignModelsList ModelList = new CampaignModelsList();

                List<int> CampaignIdList = new List<int>();
                if (order == "Desc")
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();




                        //  var CampaignRes = await query.Skip(() => skip).Take(() => maxRows).ToListAsync();

                        //var CampaignRes1 = (Entity.Tbl_Campaign.Where(s => (CategoryId != -1 ? s.Category == CategoryId : true) && s.IsApprovedbyAdmin && s.Status).
                        //   OrderByDescending(a => a.CreatedOn).Skip(() => skip).Take(() => maxRows).Select(a => new { a.Id })).ToList();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName descending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) descending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {



                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);
                        var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                    where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                    orderby a.DISTANCE descending
                                    select a.CampaignId;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                    }

                }
                else
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName ascending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) ascending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);
                        var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                    where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                    orderby a.DISTANCE ascending
                                    select a.CampaignId;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                    }

                }

                ModelList = await getcampainCommon(CampaignIdList, page, maxRows);
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<CampaignModelsList> GetCampaigns1(int CategoryId = -1, int page = 1, int page_size = 12,
       string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0")
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();

                List<int> CampaignIdList = new List<int>();
                if (order == "Desc")
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();




                        //  var CampaignRes = await query.Skip(() => skip).Take(() => maxRows).ToListAsync();

                        //var CampaignRes1 = (Entity.Tbl_Campaign.Where(s => (CategoryId != -1 ? s.Category == CategoryId : true) && s.IsApprovedbyAdmin && s.Status).
                        //   OrderByDescending(a => a.CreatedOn).Skip(() => skip).Take(() => maxRows).Select(a => new { a.Id })).ToList();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName descending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) descending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {



                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);
                        var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                    where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                    orderby a.DISTANCE descending
                                    select a.CampaignId;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                    }

                }
                else
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName ascending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) ascending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);
                        var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                    where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                    orderby a.DISTANCE ascending
                                    select a.CampaignId;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                    }

                }

                ModelList = await getcampainCommonTest1(CampaignIdList, page, maxRows);
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<CampaignModelsList> GetCampaigns2(int CategoryId = -1, int page = 1, int page_size = 12,
       string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0", int Distance = 0)
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();

                List<int> CampaignIdList = new List<int>();
                if (order == "Desc")
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();



                        //  var CampaignRes = await query.Skip(() => skip).Take(() => maxRows).ToListAsync();

                        //var CampaignRes1 = (Entity.Tbl_Campaign.Where(s => (CategoryId != -1 ? s.Category == CategoryId : true) && s.IsApprovedbyAdmin && s.Status).
                        //   OrderByDescending(a => a.CreatedOn).Skip(() => skip).Take(() => maxRows).Select(a => new { a.Id })).ToList();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "DaysLeft")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetDate ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "Totalsupporters")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CountUsage descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));//Totalsupporters
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName descending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) descending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {



                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);

                        if (Distance == 0)
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                        orderby a.DISTANCE descending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));

                        }
                        else
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null && a.DISTANCE.Value <= Distance
                                        orderby a.DISTANCE descending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                        }

                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }


                }
                else
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "DaysLeft")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetDate descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "Totalsupporters")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CountUsage ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));//Totalsupporters
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName ascending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) ascending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);
                        if (Distance == 0)
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                        orderby a.DISTANCE ascending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));

                        }
                        else
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null && a.DISTANCE.Value <= Distance
                                        orderby a.DISTANCE ascending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                        }
                    }

                }

                ModelList = await getcampainCommonTest2(CampaignIdList, page, maxRows);
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<CampaignModelsList> GetCampaigns_sp(int CategoryId = -1, int page = 1, int page_size = 12,
       string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0", int Distance = 0)
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();

                List<int> CampaignIdList = new List<int>();
                if (order == "Desc")
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();



                        //  var CampaignRes = await query.Skip(() => skip).Take(() => maxRows).ToListAsync();

                        //var CampaignRes1 = (Entity.Tbl_Campaign.Where(s => (CategoryId != -1 ? s.Category == CategoryId : true) && s.IsApprovedbyAdmin && s.Status).
                        //   OrderByDescending(a => a.CreatedOn).Skip(() => skip).Take(() => maxRows).Select(a => new { a.Id })).ToList();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "DaysLeft")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetDate ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "Totalsupporters")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CountUsage descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));//Totalsupporters
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName descending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) descending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {



                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);

                        if (Distance == 0)
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                        orderby a.DISTANCE descending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));

                        }
                        else
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null && a.DISTANCE.Value <= Distance
                                        orderby a.DISTANCE descending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                        }

                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }


                }
                else
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "DaysLeft")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetDate descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "Totalsupporters")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CountUsage ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));//Totalsupporters
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName ascending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) ascending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);
                        if (Distance == 0)
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                        orderby a.DISTANCE ascending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));

                        }
                        else
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null && a.DISTANCE.Value <= Distance
                                        orderby a.DISTANCE ascending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                        }
                    }

                }

                ModelList = await getcampainCommonTest_sp(CampaignIdList, page, maxRows);
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<CampaignModelsList> GetAdminCampaigns_sp(int CategoryId = -1, int page = 1, int page_size = 500,
      string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0", int Distance = 0)
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();

                List<int> CampaignIdList = new List<int>();

                if (order == "Desc")
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "DaysLeft")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetDate ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "Totalsupporters")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CountUsage descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));//Totalsupporters
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName descending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) descending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {



                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);

                        if (Distance == 0)
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                        orderby a.DISTANCE descending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));

                        }
                        else
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null && a.DISTANCE.Value <= Distance
                                        orderby a.DISTANCE descending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                        }

                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }


                }
                else
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "DaysLeft")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetDate descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "Totalsupporters")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CountUsage ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));//Totalsupporters
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName ascending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) ascending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);
                        if (Distance == 0)
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                        orderby a.DISTANCE ascending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));

                        }
                        else
                        {
                            var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                        where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null && a.DISTANCE.Value <= Distance
                                        orderby a.DISTANCE ascending
                                        select a.CampaignId;
                            var CampaignRes = await query.ToListAsync();

                            CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                        }
                    }

                }



                ModelList = await getcampainCommonTest_sp(CampaignIdList, page, maxRows);
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<CampaignModelsList> GetAdminnewCampaigns(DateTime LastLoginDate, int page = 1, int page_size = 500)
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();

                List<int> CampaignIdList = new List<int>();

                LastLoginDate = LastLoginDate != null ? LastLoginDate : DateTime.Now;

                var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                            where (item.CreatedOn > LastLoginDate)
                            orderby item.CreatedOn ascending
                            select item.Id;

                var CampaignRes = await query.ToListAsync();
                CampaignIdList.AddRange(CampaignRes.Select(a => a));
                ModelList = await getcampainCommonTest_sp(CampaignIdList, page, maxRows);
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<CampaignModelsList> GetAdminUnApprovalCampaigns(int page = 1, int page_size = 500)
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();

                List<int> CampaignIdList = new List<int>();

                var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                            where (item.IsApprovedbyAdmin == false)
                            orderby item.CreatedOn ascending
                            select item.Id;

                var CampaignRes = await query.ToListAsync();
                CampaignIdList.AddRange(CampaignRes.Select(a => a));
                ModelList = await getcampainCommonTest_sp(CampaignIdList, page, maxRows);
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<CampaignModelsList> GetFraudulenceCampaigns(int page = 1, int page_size = 500)
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();

                List<int> CampaignIdList = new List<int>();

                var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                            where (item.IsApprovedbyAdmin == true)
                            orderby item.CreatedOn ascending
                            select item.Id;

                var CampaignRes = await query.ToListAsync();
                CampaignIdList.AddRange(CampaignRes.Select(a => a));
                ModelList = await getcampainCommonTest_sp(CampaignIdList, page, maxRows);
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool ApproveCampaign(int Id, bool Status)
        {
            try
            {
                var res = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                res.IsApprovedbyAdmin = Status;
                res.Status = Status;
                Entity.SaveChanges();
                return true;
            }
            catch (Exception ex) { throw ex; }
        }



        public async Task<CampaignModelsList> getcampainCommonTest_sp(List<int> result, int page, int maxRows)
        {
            try
            {

                CampaignModelsList ModelList = new CampaignModelsList();
                var res = result
                           .Skip((page - 1) * maxRows)
                           .Take(maxRows).ToList();

                var re = await GetCampaignsForList_Sp(res);
                ModelList = re;
                double pageCount = (double)((decimal)result.Count() / Convert.ToDecimal(maxRows));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);
                ModelList.TotalCampaigns = result.Count();
                ModelList.CurrentPageIndex = page;
                return ModelList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<CampaignModelsList> GetCampaigns3(int CategoryId = -1, int page = 1, int page_size = 12,
       string SortBy = "CreatedOn", string order = "Desc", string Lat = "0", string Lon = "0")
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();

                List<int> CampaignIdList = new List<int>();
                if (order == "Desc")
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();


                        var query1 = from item in Entity.Tbl_Campaign.AsNoTracking()
                                     where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                     orderby item.CreatedOn descending
                                     select item.Id;
                        var CampaignRes1 = await query.ToListAsync();

                        //  var CampaignRes = await query.Skip(() => skip).Take(() => maxRows).ToListAsync();

                        //var CampaignRes1 = (Entity.Tbl_Campaign.Where(s => (CategoryId != -1 ? s.Category == CategoryId : true) && s.IsApprovedbyAdmin && s.Status).
                        //   OrderByDescending(a => a.CreatedOn).Skip(() => skip).Take(() => maxRows).Select(a => new { a.Id })).ToList();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName descending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) descending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {



                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderByDescending(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);
                        var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                    where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                    orderby a.DISTANCE descending
                                    select a.CampaignId;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                    }

                }
                else
                {
                    if (SortBy == "CreatedOn")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.Status && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking()
                                     join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                     orderby Cit.CityName ascending
                                     select ben.StoryId);
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "RaisedAmount")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     orderby g.Sum(x => x.DonationAmnt) ascending
                                     select g.Key);

                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Distinct().Select(a => a));
                    }
                    else if (SortBy == "Donors")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_CampaignDonation.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.Where(Don => Don.isPaid == true).DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.Id > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Likes")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Like.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.LikeID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Comments")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_ParentComment.AsNoTracking() on Cam.Id equals Don.StoryId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.CommentID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Endorsements")
                    {
                        var query = (from Cam in Entity.Tbl_Campaign.AsNoTracking()
                                     join Don in Entity.Tbl_Endorse.AsNoTracking() on Cam.Id equals Don.CampaignId into leftjoinResult
                                     from Don in leftjoinResult.DefaultIfEmpty()
                                     group Don by Cam.Id into g
                                     select new
                                     {
                                         Id = g.Key,
                                         Count = g.Sum(s => s.EndorseID > 0 ? 1 : 0)
                                     }).OrderBy(y => y.Count);
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Id));
                    }
                    else if (SortBy == "Distance")
                    {
                        var results = Entity.GetCampaignsByDistance(Lat, Lon);
                        var query = from a in Entity.tbl_DistanceCount.AsNoTracking()
                                    where a.InputLat == Lat && a.InputLon == Lon && a.CampaignId != null
                                    orderby a.DISTANCE ascending
                                    select a.CampaignId;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a.Value));
                    }

                }

                ModelList = await getcampainCommonTest3(CampaignIdList, page, maxRows);
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<CampaignModelsList> getcampainCommon(List<int> result, int page, int maxRows)
        {
            try
            {

                CampaignModelsList ModelList = new CampaignModelsList();

                int skip = (page - 1) * maxRows;
                var res = result
                             .Skip(skip).Take(maxRows).ToList();
                if (res.Any())
                {
                    foreach (var item in res)
                    {
                        CampaignModel Model = new CampaignModel();
                        Model = await GetCamapignsForListView(item);
                        ModelList.CampaignLists.Add(Model);
                    }
                }
                double pageCount = (double)((decimal)result.Count() / Convert.ToDecimal(maxRows));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);
                ModelList.TotalCampaigns = result.Count();
                ModelList.CurrentPageIndex = page;
                return ModelList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CampaignModelsList> getcampainCommonTest1(List<int> result, int page, int maxRows)
        {
            try
            {

                CampaignModelsList ModelList = new CampaignModelsList();
                var res = result
                           .Skip((page - 1) * maxRows)
                           .Take(maxRows).ToList();
                if (res.Any())
                {
                    foreach (var item in res)
                    {
                        CampaignModel Model = new CampaignModel();
                        returnModel retmodel = new returnModel();
                        retmodel = await GetCampaignsForListtest(item);
                        if (retmodel.IsActive)
                        {
                            ModelList.CampaignLists.Add(retmodel.CampaignModel);
                        }
                    }
                }
                double pageCount = (double)((decimal)result.Count() / Convert.ToDecimal(maxRows));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);
                ModelList.TotalCampaigns = result.Count();
                ModelList.CurrentPageIndex = page;
                return ModelList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<CampaignModelsList> getcampainCommonTest2(List<int> result, int page, int maxRows)
        {
            try
            {

                CampaignModelsList ModelList = new CampaignModelsList();
                var res = result
                           .Skip((page - 1) * maxRows)
                           .Take(maxRows).ToList();


                //if (res.Any())
                //{
                //    foreach (var item in res)
                //    {
                //        CampaignModel Model = new CampaignModel();
                //        Model = GetCampaignsForListtest(item);
                //        ModelList.CampaignLists.Add(Model);
                //    }
                //}

                var re = await GetCampaignsForListtest2(res);
                ModelList = re;
                double pageCount = (double)((decimal)result.Count() / Convert.ToDecimal(maxRows));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);
                ModelList.TotalCampaigns = result.Count();
                ModelList.CurrentPageIndex = page;
                return ModelList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CampaignModelsList> getcampainCommonTest3(List<int> result, int page, int maxRows)
        {
            try
            {

                CampaignModelsList ModelList = new CampaignModelsList();
                var res = result
                           .Skip((page - 1) * maxRows)
                           .Take(maxRows).ToList();
                //if (res.Any())
                //{
                //    foreach (var item in res)
                //    {
                //        CampaignModel Model = new CampaignModel();
                //        Model = GetCampaignsForListtest(item);
                //        ModelList.CampaignLists.Add(Model);
                //    }
                //}
                ModelList.CampaignLists = new List<CampaignModel>();
                var re = await GetCamapignsForListView_Tuned(res);
                ModelList.CampaignLists.AddRange(re);


                double pageCount = (double)((decimal)result.Count() / Convert.ToDecimal(maxRows));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);
                ModelList.TotalCampaigns = result.Count();
                ModelList.CurrentPageIndex = page;
                return ModelList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<CampaignModelsList> GetCampaignsold(int CategoryId = -1, int page = 1, int page_size = 12,
            string SortBy = "Created On", string order = "Desc")
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();
                var result = (from S in Entity.Tbl_Campaign select S).Where(s => CategoryId != -1 ? s.Category == CategoryId : true).ToList()
                     ;
                result = result.Where(S => S.IsApprovedbyAdmin && S.Status).ToList();

                if (order == "Desc")
                {
                    if (SortBy == "Created On")
                    {
                        result = result.OrderByDescending(a => a.CreatedOn).ToList();
                    }
                    if (SortBy == "goal")
                    {
                        result = result.OrderByDescending(a => a.TargetAmount).ToList();
                    }

                }
                else
                {
                    if (SortBy == "Created On")
                    {
                        result = result.OrderBy(a => a.CreatedOn).ToList();
                    }
                    if (SortBy == "goal")
                    {
                        result = result.OrderBy(a => a.TargetAmount).ToList();
                    }
                }
                var res = result
                        .Skip((page - 1) * maxRows)
                        .Take(maxRows).ToList();
                if (res.Any())
                {
                    foreach (var item in res)
                    {
                        CampaignModel Model = new CampaignModel();
                        Model = await GetCamapignsForListView(item.Id);
                        ModelList.CampaignLists.Add(Model);
                    }
                }
                double pageCount = (double)((decimal)result.Count() / Convert.ToDecimal(maxRows));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);
                ModelList.TotalCampaigns = result.Count();
                ModelList.CurrentPageIndex = page;
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<CampaignModelsList> GetCampaignsForListtest2(List<int> CampaignIds)
        {
            try
            {
                var CampaignDetails = await (from cm in Entity.Tbl_Campaign.AsNoTracking()
                                             join Dc in Entity.Tbl_CampaignDescription.AsNoTracking() on cm.Id equals Dc.StoryId
                                             join ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking() on cm.Id equals ben.StoryId
                                             join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                             join Dn in Entity.Tbl_CampaignDonation.AsNoTracking() on cm.Id equals Dn.StoryId into outerJoinDon
                                             from Dn in outerJoinDon.DefaultIfEmpty()
                                             where CampaignIds.Contains(cm.Id)
                                             select new
                                             {
                                                 CampaignId = cm.Id,
                                                 cm.Category,
                                                 cm.IsApprovedbyAdmin,
                                                 cm.Title,
                                                 cm.UserId,
                                                 cm.CreatedBy,
                                                 cm.CreatedOn,
                                                 cm.MoneyType,
                                                 cm.Status,
                                                 cm.TargetAmount,
                                                 cm.TargetDate,
                                                 cm.CountUsage,
                                                 cm.CreatedUserName,
                                                 beneficiaryId = ben != null ? ben.Id : 0,
                                                 DPPath = ben != null ? ben.DPPath : "",
                                                 BResidence = ben != null ? ben.BResidence : 0,
                                                 CityName = Cit != null ? Cit.CityName : "",
                                                 Latitude = Cit != null ? Cit.Latitude : "",
                                                 longitude = Cit != null ? Cit.longitude : "",
                                                 DPName = ben != null ? ben.DPName : "",
                                                 DescId = Dc != null ? Dc.Id : 0,
                                                 storyDescription = Dc != null ? Dc.storyDescription : "",
                                                 DonationId = Dn != null ? Dn.Id : 0,
                                                 DonatedBy = Dn != null ? Dn.DonatedBy : "",
                                                 DonatedOn = Dn != null ? Dn.DonatedOn : DateTime.Now,
                                                 isAnanymous = Dn != null ? Dn.isAnanymous : true,
                                                 DonationAmnt = Dn != null ? Dn.DonationAmnt : 0,
                                                 isPaid = Dn != null ? Dn.isPaid : false
                                             }).ToListAsync();
                CampaignModelsList ModelList = new CampaignModelsList();
                foreach (int CamapignId in CampaignIds)
                {
                    CampaignModel Model = new CampaignModel();
                    var CurrentDetail = CampaignDetails.Where(a => a.CampaignId == CamapignId).ToList();
                    if (CurrentDetail.Any())
                    {
                        var DonList = CurrentDetail.Where(x => x.isPaid == true).GroupBy(x => x.CampaignId)
                    .Select(g => new
                    {
                        DonatedBy = g.Key,
                        Total = g.Sum(x => x.DonationAmnt)
                    });

                        var DonorList = CurrentDetail.Where(x => x.isPaid == true)
                             .GroupBy(x => x.DonatedBy)
                             .Select(g => new
                             {
                                 DonatedBy = g.Key,
                                 Total = g.Sum(x => x.DonationAmnt)
                             });

                        var CampaignDetail = CurrentDetail.FirstOrDefault();


                        Model.Id = CampaignDetail.CampaignId;
                        var StoryCategory = (StoryCategory)CampaignDetail.Category;
                        Model.CategoryName = StoryCategory.DisplayName();
                        Model.CampaignStatus = "Draft";
                        Model.IsApprovedbyAdmin = CampaignDetail.IsApprovedbyAdmin;
                        Model.CampaignTitle = CampaignDetail.Title != null ? CampaignDetail.Title : "";
                        Model.UserId = CampaignDetail.UserId;
                        Model.CreatedBy = CampaignDetail.CreatedBy;
                        Model.CreatedOn = CampaignDetail.CreatedOn.Value;
                        Model.CampaignTargetMoney = CampaignDetail.TargetAmount.Value;
                        Model.CampaignTargetDate = CampaignDetail.TargetDate != null ? CampaignDetail.TargetDate.Value : DateTime.Now.AddDays(31);
                        var MoneyType = (MoneyType)(string.IsNullOrEmpty(CampaignDetail.MoneyType) ? 0 : Convert.ToInt32(CampaignDetail.MoneyType));
                        Model.CampaignTargetMoneyType = MoneyType.DisplayName();

                        int diff2 = (CampaignDetail.TargetDate != null) ? ((CampaignDetail.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                        if (CampaignDetail.TargetDate != null && CampaignDetail.TargetDate.Value.Date == DateTime.Now.Date)
                        {
                            Model.DaysLeft = diff2;
                        }
                        else
                        {
                            Model.DaysLeft = diff2 + 1;
                        }
                        Model.OrganizerName = CampaignDetail.CreatedUserName;

                        String[] spearator1 = { " " };
                        String[] strlist1 = CampaignDetail.CreatedUserName.Split(spearator1, StringSplitOptions.RemoveEmptyEntries);
                        List<string> initialList = new List<string>();
                        foreach (var i in strlist1)
                        {
                            initialList.Add(i.Substring(0, 1));
                        }
                        var result1 = initialList.ToArray();
                        var val1 = string.Join("", result1);

                        Model.DisplayInitial = val1;

                        var userdetail = await (from cm in Entity.Tbl_User.AsNoTracking()
                                                where cm.Id == CampaignDetail.UserId
                                                select new
                                                {
                                                    UserDPImage = cm.DPPath
                                                }).FirstOrDefaultAsync();

                        Model.UserDPImage = userdetail.UserDPImage;
                        Model.OrganizerPictureUrl = userdetail.UserDPImage;
                        Model.Totalsupporters = CampaignDetail.CountUsage;


                        if (CampaignDetail.beneficiaryId > 0)
                        {
                            String str = CampaignDetail.CityName != null ? CampaignDetail.CityName : "";
                            String[] spearator = { "," };
                            String[] strlist = str.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                            var result = strlist.Reverse().Skip(2).Take(1).Reverse().ToArray();
                            var val = string.Join(", ", result);
                            Model.placeName = string.IsNullOrWhiteSpace(val) ? str : val;
                            Model.Latitude = CampaignDetail.Latitude;
                            Model.Longitude = CampaignDetail.longitude;
                            Model.FullplaceName = CampaignDetail.CityName != null ? CampaignDetail.CityName : "";
                            if (!string.IsNullOrEmpty(CampaignDetail.DPPath))
                            {
                                string key = ConfigurationManager.AppSettings["BasicUrl"];
                                Model.BDisplayPicPath = (key + CampaignDetail.DPPath);
                            }
                        }
                        if (CampaignDetail.DescId > 0)
                        {
                            Model.CampaignDescriptionDtl = CampaignDetail.storyDescription;
                            var stripeddesc = CampaignDetail.storyDescription != null ? StripTagsCharArray(CampaignDetail.storyDescription) : "";
                            Model.CampaignDescription = stripeddesc;
                            if (CampaignDetail.IsApprovedbyAdmin)
                                Model.CampaignStatus = "Active";
                            else
                                Model.CampaignStatus = "Pending Approval";

                            if (!CampaignDetail.Status)
                                Model.CampaignStatus = "InActive";
                        }

                        var RaisedAmt = DonList.Any() ? DonList.ToList().FirstOrDefault().Total : 0;
                        decimal difference = CampaignDetail.TargetAmount.Value - RaisedAmt;
                        var raisedPerc = CampaignDetail.TargetAmount != null ? (CampaignDetail.TargetAmount.Value != 0 ? ((RaisedAmt / CampaignDetail.TargetAmount.Value) * 100) : 0) : 0;
                        var RaisedBy = DonorList.Any() ? DonorList.Distinct().ToList().Count() : 0;
                        Model.RaisedAmount = RaisedAmt;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);

                        ModelList.CampaignLists.Add(Model);
                    }
                }





                return ModelList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CampaignModelsList> GetCampaignsForList_Sp(List<int> CampaignIds)
        {
            try
            {

                var CampaignDetails = new List<spres_getsummary>();
                using (var dbContext = new GivingActuallyEntities())
                {

                    //  var Tresults = await dbContext.Database.SqlQuery<spres_get1cam>("EXEC GetCampaigns1Test {0}, {1}", p1, p2)
                    //.ToArrayAsync();
                    CampaignIds.Add(0);
                    var Tresults = await dbContext.Database.SqlQuery<spres_getsummary>("EXEC GetCampaignSummary {0}", String.Join(",", CampaignIds))
                   .ToListAsync();
                    //Get second result set
                    CampaignDetails.AddRange(Tresults);
                }
                CampaignModelsList ModelList = new CampaignModelsList();
                foreach (int CamapignId in CampaignIds)
                {
                    CampaignModel Model = new CampaignModel();
                    var CurrentDetail = CampaignDetails.Where(a => a.Id == CamapignId).ToList();
                    if (CurrentDetail.Any())
                    {
                        var DonList = CurrentDetail.Where(x => x.isPaid == true).GroupBy(x => x.Id)
                    .Select(g => new
                    {
                        DonatedBy = g.Key,
                        Total = g.Sum(x => x.DonationAmnt)
                    });

                        var DonorList = CurrentDetail.Where(x => x.isPaid == true)
                             .GroupBy(x => x.DonatedBy)
                             .Select(g => new
                             {
                                 DonatedBy = g.Key,
                                 Total = g.Sum(x => x.DonationAmnt)
                             });

                        var CampaignDetail = CurrentDetail.FirstOrDefault();


                        Model.Id = CampaignDetail.Id;
                        var StoryCategory = (StoryCategory)CampaignDetail.Category;
                        Model.CategoryName = StoryCategory.DisplayName();
                        Model.CampaignStatus = "Draft";
                        Model.IsApprovedbyAdmin = CampaignDetail.IsApprovedbyAdmin;
                        Model.CampaignTitle = CampaignDetail.Title != null ? CampaignDetail.Title : "";
                        Model.UserId = CampaignDetail.UserId;
                        Model.CreatedBy = CampaignDetail.CreatedBy;
                        Model.CreatedOn = CampaignDetail.CreatedOn.Value;
                        Model.CampaignTargetMoney = CampaignDetail.TargetAmount.Value;
                        Model.CampaignTargetDate = CampaignDetail.TargetDate != null ? CampaignDetail.TargetDate.Value : DateTime.Now.AddDays(31);
                        var MoneyType = (MoneyType)(string.IsNullOrEmpty(CampaignDetail.MoneyType) ? 0 : Convert.ToInt32(CampaignDetail.MoneyType));
                        Model.CampaignTargetMoneyType = MoneyType.DisplayName();

                        int diff2 = (CampaignDetail.TargetDate != null) ? ((CampaignDetail.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                        if (CampaignDetail.TargetDate != null && CampaignDetail.TargetDate.Value.Date == DateTime.Now.Date)
                        {
                            Model.DaysLeft = diff2;
                        }
                        else
                        {
                            Model.DaysLeft = diff2 + 1;
                        }
                        Model.OrganizerName = CampaignDetail.CreatedUserName;

                        String[] spearator1 = { " " };
                        String[] strlist1 = CampaignDetail.CreatedUserName.Split(spearator1, StringSplitOptions.RemoveEmptyEntries);
                        List<string> initialList = new List<string>();
                        foreach (var i in strlist1)
                        {
                            initialList.Add(i.Substring(0, 1));
                        }
                        var result1 = initialList.ToArray();
                        var val1 = string.Join("", result1);

                        Model.DisplayInitial = val1;


                        Model.UserDPImage = CampaignDetail.UserDPImage;
                        Model.OrganizerPictureUrl = CampaignDetail.UserDPImage;
                        Model.Totalsupporters = CampaignDetail.CountUsage;


                        if (CampaignDetail.beneficiaryId > 0)
                        {
                            String str = CampaignDetail.CityName != null ? CampaignDetail.CityName : "";
                            String[] spearator = { "," };
                            String[] strlist = str.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                            var result = strlist.Reverse().Skip(2).Take(1).Reverse().ToArray();
                            var val = string.Join(", ", result);
                            Model.placeName = string.IsNullOrWhiteSpace(val) ? str : val;
                            Model.Latitude = CampaignDetail.Latitude;
                            Model.Longitude = CampaignDetail.longitude;
                            Model.FullplaceName = CampaignDetail.CityName != null ? CampaignDetail.CityName : "";
                            if (!string.IsNullOrEmpty(CampaignDetail.DPPath))
                            {
                                string key = ConfigurationManager.AppSettings["BasicUrl"];
                                Model.BDisplayPicPath = (key + CampaignDetail.DPPath);
                            }
                        }
                        if (CampaignDetail.DescId > 0)
                        {
                            Model.CampaignDescriptionDtl = CampaignDetail.storyDescription;
                            var stripeddesc = CampaignDetail.storyDescription != null ? StripTagsCharArray(CampaignDetail.storyDescription) : "";
                            Model.CampaignDescription = stripeddesc;
                            if (CampaignDetail.IsApprovedbyAdmin)
                                Model.CampaignStatus = "Active";
                            else
                                Model.CampaignStatus = "Pending Approval";

                            if (!CampaignDetail.Status)
                                Model.CampaignStatus = "InActive";
                        }

                        var RaisedAmt = DonList.Any() ? DonList.ToList().FirstOrDefault().Total : 0;
                        decimal difference = CampaignDetail.TargetAmount.Value - RaisedAmt.Value;
                        var raisedPerc = CampaignDetail.TargetAmount != null ? (CampaignDetail.TargetAmount.Value != 0 ? ((RaisedAmt / CampaignDetail.TargetAmount.Value) * 100) : 0) : 0;
                        var RaisedBy = DonorList.Any() ? DonorList.Distinct().ToList().Count() : 0;
                        Model.RaisedAmount = RaisedAmt.Value;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);

                        ModelList.CampaignLists.Add(Model);
                    }
                }





                return ModelList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<CampaignModel> GetCampaignSummary(int CampaignId)
        {
            try
            {
                var CampaignDetails = await (from cm in Entity.Tbl_Campaign.AsNoTracking()
                                             join Dc in Entity.Tbl_CampaignDescription.AsNoTracking() on cm.Id equals Dc.StoryId
                                             join ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking() on cm.Id equals ben.StoryId
                                             join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                                             join Dn in Entity.Tbl_CampaignDonation.AsNoTracking() on cm.Id equals Dn.StoryId into outerJoinDon
                                             from Dn in outerJoinDon.DefaultIfEmpty()
                                             where cm.Id == CampaignId
                                             select new
                                             {
                                                 CampaignId = cm.Id,
                                                 cm.Category,
                                                 cm.IsApprovedbyAdmin,
                                                 cm.Title,
                                                 cm.UserId,
                                                 cm.CreatedBy,
                                                 cm.CreatedOn,
                                                 cm.MoneyType,
                                                 cm.Status,
                                                 cm.TargetAmount,
                                                 cm.TargetDate,
                                                 cm.CountUsage,
                                                 cm.CreatedUserName,
                                                 beneficiaryId = ben != null ? ben.Id : 0,
                                                 DPPath = ben != null ? ben.DPPath : "",
                                                 BResidence = ben != null ? ben.BResidence : 0,
                                                 CityName = Cit != null ? Cit.CityName : "",
                                                 Latitude = Cit != null ? Cit.Latitude : "",
                                                 longitude = Cit != null ? Cit.longitude : "",
                                                 DPName = ben != null ? ben.DPName : "",
                                                 DescId = Dc != null ? Dc.Id : 0,
                                                 storyDescription = Dc != null ? Dc.storyDescription : "",
                                                 DonationId = Dn != null ? Dn.Id : 0,
                                                 Email = Dn != null ? Dn.Email : "",
                                                 DonatedBy = Dn != null ? Dn.DonatedBy : "",
                                                 DonatedOn = Dn != null ? Dn.DonatedOn : DateTime.Now,
                                                 isAnanymous = Dn != null ? Dn.isAnanymous : true,
                                                 DonationAmnt = Dn != null ? Dn.DonationAmnt : 0,
                                                 isPaid = Dn != null ? Dn.isPaid : false
                                             }).ToListAsync();
                CampaignModelsList ModelList = new CampaignModelsList();

                CampaignModel Model = new CampaignModel();
                var CurrentDetail = CampaignDetails.ToList();
                if (CurrentDetail.Any())
                {
                    var DonList = CurrentDetail.Where(x => x.isPaid == true).GroupBy(x => x.CampaignId)
                .Select(g => new
                {
                    DonatedBy = g.Key,
                    Total = g.Sum(x => x.DonationAmnt)
                });

                    var DonorList = CurrentDetail.Where(x => x.isPaid == true)
                         .GroupBy(x => x.Email)
                         .Select(g => new
                         {
                             Email = g.Key,
                             DonatedBy = g.FirstOrDefault().DonatedBy,
                             Total = g.Sum(x => x.DonationAmnt)
                         });

                    var CampaignDetail = CurrentDetail.FirstOrDefault();


                    Model.Id = CampaignDetail.CampaignId;
                    var StoryCategory = (StoryCategory)CampaignDetail.Category;
                    Model.CategoryName = StoryCategory.DisplayName();
                    Model.CampaignStatus = "Draft";
                    Model.IsApprovedbyAdmin = CampaignDetail.IsApprovedbyAdmin;
                    Model.CampaignTitle = CampaignDetail.Title != null ? CampaignDetail.Title : "";
                    Model.UserId = CampaignDetail.UserId;
                    Model.CreatedBy = CampaignDetail.CreatedBy;
                    Model.CreatedOn = CampaignDetail.CreatedOn.Value;
                    Model.CampaignTargetMoney = CampaignDetail.TargetAmount.Value;
                    Model.CampaignTargetDate = CampaignDetail.TargetDate != null ? CampaignDetail.TargetDate.Value : DateTime.Now.AddDays(31);
                    var MoneyType = (MoneyType)(string.IsNullOrEmpty(CampaignDetail.MoneyType) ? 0 : Convert.ToInt32(CampaignDetail.MoneyType));
                    Model.CampaignTargetMoneyType = MoneyType.DisplayName();

                    int diff2 = (CampaignDetail.TargetDate != null) ? ((CampaignDetail.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                    if (CampaignDetail.TargetDate != null && CampaignDetail.TargetDate.Value.Date == DateTime.Now.Date)
                    {
                        Model.DaysLeft = diff2;
                    }
                    else
                    {
                        Model.DaysLeft = diff2 + 1;
                    }
                    Model.OrganizerName = CampaignDetail.CreatedUserName;

                    String[] spearator1 = { " " };
                    String[] strlist1 = CampaignDetail.CreatedUserName.Split(spearator1, StringSplitOptions.RemoveEmptyEntries);
                    List<string> initialList = new List<string>();
                    foreach (var i in strlist1)
                    {
                        initialList.Add(i.Substring(0, 1));
                    }
                    var result1 = initialList.ToArray();
                    var val1 = string.Join("", result1);

                    Model.DisplayInitial = val1;


                    var userdetail = await (from cm in Entity.Tbl_User.AsNoTracking()
                                            where cm.Id == CampaignDetail.UserId
                                            select new
                                            {
                                                UserDPImage = cm.DPPath
                                            }).FirstOrDefaultAsync();

                    Model.UserDPImage = userdetail.UserDPImage;
                    Model.OrganizerPictureUrl = userdetail.UserDPImage;
                    Model.Totalsupporters = CampaignDetail.CountUsage;


                    if (CampaignDetail.beneficiaryId > 0)
                    {
                        String str = CampaignDetail.CityName != null ? CampaignDetail.CityName : "";
                        String[] spearator = { "," };
                        String[] strlist = str.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                        var result = strlist.Reverse().Skip(2).Take(1).Reverse().ToArray();
                        var val = string.Join(", ", result);
                        Model.placeName = string.IsNullOrWhiteSpace(val) ? str : val;
                        Model.Latitude = CampaignDetail.Latitude;
                        Model.Longitude = CampaignDetail.longitude;
                        Model.FullplaceName = CampaignDetail.CityName != null ? CampaignDetail.CityName : "";
                        if (!string.IsNullOrEmpty(CampaignDetail.DPPath))
                        {
                            string key = ConfigurationManager.AppSettings["BasicUrl"];
                            Model.BDisplayPicPath = (key + CampaignDetail.DPPath);
                        }
                    }
                    if (CampaignDetail.DescId > 0)
                    {
                        Model.CampaignDescriptionDtl = CampaignDetail.storyDescription;
                        var stripeddesc = CampaignDetail.storyDescription != null ? StripTagsCharArray(CampaignDetail.storyDescription) : "";
                        Model.CampaignDescription = stripeddesc;
                        if (CampaignDetail.IsApprovedbyAdmin)
                            Model.CampaignStatus = "Active";
                        else
                            Model.CampaignStatus = "Pending Approval";

                        if (!CampaignDetail.Status)
                            Model.CampaignStatus = "InActive";
                    }

                    var RaisedAmt = DonList.Any() ? DonList.ToList().FirstOrDefault().Total : 0;
                    decimal difference = CampaignDetail.TargetAmount.Value - RaisedAmt;
                    var raisedPerc = CampaignDetail.TargetAmount != null ? (CampaignDetail.TargetAmount.Value != 0 ? ((RaisedAmt / CampaignDetail.TargetAmount.Value) * 100) : 0) : 0;
                    var RaisedBy = DonorList.Any() ? DonorList.Distinct().ToList().Count() : 0;
                    Model.RaisedAmount = RaisedAmt;
                    Model.RaisedBy = RaisedBy;
                    Model.RaisedPercentage = Convert.ToInt32(raisedPerc);

                    ModelList.CampaignLists.Add(Model);
                }






                return Model;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<returnModel> GetCampaignsForListtest(int Id)
        {
            try
            {
                returnModel RetModel = new returnModel();
                CampaignModel Model = new CampaignModel();



                //var CampaignDetails = (from cm in Entity.Tbl_Campaign
                //                       join Dc in Entity.Tbl_CampaignDescription on cm.Id equals Dc.StoryId into outerJoinDc
                //                       from Dc in outerJoinDc.DefaultIfEmpty()
                //                       join ben in Entity.Tbl_BeneficiaryDetails on cm.Id equals ben.StoryId into outerJoinben
                //                       from ben in outerJoinben.DefaultIfEmpty()
                //                       join Cit in Entity.Tbl_CityDetails on ben.BResidence equals Cit.CityId into outerJoinCit
                //                       from Cit in outerJoinCit.DefaultIfEmpty()
                //                       join Dn in Entity.Tbl_CampaignDonation on cm.Id equals Dn.StoryId into outerJoinDon
                //                       from Dn in outerJoinDon.DefaultIfEmpty()
                //                       where cm.Id == Id //&& (Dn.isPaid == true)
                //                       select new
                //                       {
                //                           CampaignId = cm.Id,
                //                           cm.Category,
                //                           cm.IsApprovedbyAdmin,
                //                           cm.Title,
                //                           cm.UserId,
                //                           cm.CreatedBy,
                //                           cm.CreatedOn,
                //                           cm.MoneyType,
                //                           cm.Status,
                //                           cm.TargetAmount,
                //                           beneficiaryId = ben != null ? ben.Id : 0,
                //                           DPPath = ben != null ? ben.DPPath : "",
                //                           BResidence = ben != null ? ben.BResidence : 0,
                //                           CityName = Cit != null ? Cit.CityName : "",
                //                           Latitude = Cit != null ? Cit.Latitude : "",
                //                           longitude = Cit != null ? Cit.longitude : "",
                //                           DPName = ben != null ? ben.DPName : "",
                //                           DescId = Dc != null ? Dc.Id : 0,
                //                           storyDescription = Dc != null ? Dc.storyDescription : "",
                //                           DonationId = Dn != null ? Dn.Id : 0,
                //                           DonatedBy = Dn != null ? Dn.DonatedBy : "",
                //                           DonatedOn = Dn != null ? Dn.DonatedOn : DateTime.Now,
                //                           isAnanymous = Dn != null ? Dn.isAnanymous : true,
                //                           DonationAmnt = Dn != null ? Dn.DonationAmnt : 0,
                //                           isPaid = Dn != null ? Dn.isPaid : false
                //                       }).ToList();

                var query = (from cm in Entity.Tbl_Campaign.AsNoTracking()
                             join Dc in Entity.Tbl_CampaignDescription.AsNoTracking() on cm.Id equals Dc.StoryId
                             join ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking() on cm.Id equals ben.StoryId
                             join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId
                             join Dn in Entity.Tbl_CampaignDonation.AsNoTracking() on cm.Id equals Dn.StoryId into outerJoinDon
                             from Dn in outerJoinDon.DefaultIfEmpty()
                             where cm.Id == Id
                             select new
                             {
                                 CampaignId = cm.Id,
                                 cm.Category,
                                 cm.IsApprovedbyAdmin,
                                 cm.Title,
                                 cm.UserId,
                                 cm.CreatedBy,
                                 cm.CreatedOn,
                                 cm.MoneyType,
                                 cm.Status,
                                 cm.TargetAmount,
                                 cm.TargetDate,
                                 cm.CountUsage,
                                 cm.CreatedUserName,
                                 beneficiaryId = ben != null ? ben.Id : 0,
                                 DPPath = ben != null ? ben.DPPath : "",
                                 BResidence = ben != null ? ben.BResidence : 0,
                                 CityName = Cit != null ? Cit.CityName : "",
                                 Latitude = Cit != null ? Cit.Latitude : "",
                                 longitude = Cit != null ? Cit.longitude : "",
                                 DPName = ben != null ? ben.DPName : "",
                                 DescId = Dc != null ? Dc.Id : 0,
                                 storyDescription = Dc != null ? Dc.storyDescription : "",
                                 DonationId = Dn != null ? Dn.Id : 0,
                                 DonatedBy = Dn != null ? Dn.DonatedBy : "",
                                 DonatedOn = Dn != null ? Dn.DonatedOn : DateTime.Now,
                                 isAnanymous = Dn != null ? Dn.isAnanymous : true,
                                 DonationAmnt = Dn != null ? Dn.DonationAmnt : 0,
                                 isPaid = Dn != null ? Dn.isPaid : false
                             });

                var CampaignDetails = await query.ToListAsync();
                bool IsActive = false;
                if (CampaignDetails.Any())
                {
                    IsActive = true;
                    var DonList = CampaignDetails.Where(x => x.isPaid == true).GroupBy(x => x.CampaignId)
                     .Select(g => new
                     {
                         DonatedBy = g.Key,
                         Total = g.Sum(x => x.DonationAmnt)
                     });

                    var DonorList = CampaignDetails.Where(x => x.isPaid == true)
                         .GroupBy(x => x.DonatedBy)
                         .Select(g => new
                         {
                             DonatedBy = g.Key,
                             Total = g.Sum(x => x.DonationAmnt)
                         });

                    var CampaignDetail = CampaignDetails.FirstOrDefault();


                    Model.Id = CampaignDetail.CampaignId;
                    var StoryCategory = (StoryCategory)CampaignDetail.Category;
                    Model.CategoryName = StoryCategory.DisplayName();
                    Model.CampaignStatus = "Draft";
                    Model.IsApprovedbyAdmin = CampaignDetail.IsApprovedbyAdmin;
                    Model.CampaignTitle = CampaignDetail.Title != null ? CampaignDetail.Title : "";
                    Model.UserId = CampaignDetail.UserId;
                    Model.CreatedBy = CampaignDetail.CreatedBy;
                    Model.CreatedOn = CampaignDetail.CreatedOn.Value;
                    Model.CampaignTargetMoney = CampaignDetail.TargetAmount.Value;

                    Model.CampaignTargetDate = CampaignDetail.TargetDate != null ? CampaignDetail.TargetDate.Value : DateTime.Now.AddDays(31);
                    var MoneyType = (MoneyType)(string.IsNullOrEmpty(CampaignDetail.MoneyType) ? 0 : Convert.ToInt32(CampaignDetail.MoneyType));
                    Model.CampaignTargetMoneyType = MoneyType.DisplayName();
                    int diff2 = (CampaignDetail.TargetDate != null) ? ((CampaignDetail.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                    if (CampaignDetail.TargetDate != null && CampaignDetail.TargetDate.Value.Date == DateTime.Now.Date)
                    {
                        Model.DaysLeft = diff2;
                    }
                    else
                    {
                        Model.DaysLeft = diff2 + 1;
                    }
                    Model.OrganizerName = CampaignDetail.CreatedUserName;
                    Model.OrganizerPictureUrl = "";
                    Model.Totalsupporters = CampaignDetail.CountUsage;
                    if (CampaignDetail.beneficiaryId > 0)
                    {
                        String str = CampaignDetail.CityName != null ? CampaignDetail.CityName : "";
                        String[] spearator = { "," };
                        String[] strlist = str.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                        //  var result = strlist.Reverse().Take(2).Reverse().ToArray();
                        var result = strlist.Reverse().Skip(2).Take(1).Reverse().ToArray();
                        var val = string.Join(", ", result);

                        Model.placeName = string.IsNullOrWhiteSpace(val) ? str : val;
                        Model.Latitude = CampaignDetail.Latitude;
                        Model.Longitude = CampaignDetail.longitude;
                        Model.FullplaceName = CampaignDetail.CityName != null ? CampaignDetail.CityName : "";
                        if (!string.IsNullOrEmpty(CampaignDetail.DPPath))
                        {
                            string key = ConfigurationManager.AppSettings["BasicUrl"];
                            Model.BDisplayPicPath = (key + CampaignDetail.DPPath);
                        }

                    }
                    if (CampaignDetail.DescId > 0)
                    {
                        Model.CampaignDescriptionDtl = CampaignDetail.storyDescription;
                        var stripeddesc = CampaignDetail.storyDescription != null ? StripTagsCharArray(CampaignDetail.storyDescription) : "";
                        Model.CampaignDescription = stripeddesc;
                        if (CampaignDetail.IsApprovedbyAdmin)
                            Model.CampaignStatus = "Active";
                        else
                            Model.CampaignStatus = "Pending Approval";

                        if (!CampaignDetail.Status)
                            Model.CampaignStatus = "InActive";
                    }

                    var RaisedAmt = DonList.Any() ? DonList.ToList().FirstOrDefault().Total : 0;
                    decimal difference = CampaignDetail.TargetAmount.Value - RaisedAmt;
                    var raisedPerc = CampaignDetail.TargetAmount != null ? (CampaignDetail.TargetAmount.Value != 0 ? ((RaisedAmt / CampaignDetail.TargetAmount.Value) * 100) : 0) : 0;
                    var RaisedBy = DonorList.Any() ? DonorList.Distinct().ToList().Count() : 0;
                    Model.RaisedAmount = RaisedAmt;
                    Model.RaisedBy = RaisedBy;
                    Model.RaisedPercentage = Convert.ToInt32(raisedPerc);
                }

                RetModel.CampaignModel = new CampaignModel();
                RetModel.CampaignModel = Model;
                RetModel.IsActive = IsActive;
                return RetModel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<CampaignModel> GetCamapignsForListView(int Id)
        {
            try
            {
                CampaignModel Model = new CampaignModel();

                var query = from item in Entity.Tbl_Campaign.AsNoTracking()
                            where item.Id == Id
                            select item;
                var res = await query.FirstOrDefaultAsync();
                if (res != null)
                {
                    Model.Id = res.Id;
                    var StoryCategory = (StoryCategory)res.Category;
                    Model.CategoryName = StoryCategory.DisplayName();
                    Model.CampaignStatus = "Draft";
                    Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                    Model.CampaignTitle = res.Title != null ? res.Title : "";
                    Model.UserId = res.UserId;
                    Model.CreatedBy = res.CreatedBy;
                    Model.CreatedOn = res.CreatedOn.Value;
                    int diff2 = (res.TargetDate != null) ? ((res.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                    if (res.TargetDate != null && res.TargetDate.Value.Date == DateTime.Now.Date)
                    {
                        Model.DaysLeft = diff2;
                    }
                    else
                    {
                        Model.DaysLeft = diff2 + 1;
                    }
                    Model.OrganizerName = res.CreatedUserName;
                    Model.OrganizerPictureUrl = "";
                    Model.Totalsupporters = res.CountUsage;
                    Model.CampaignTargetDate = res.TargetDate != null ? res.TargetDate.Value : DateTime.Now.AddDays(31);
                    Model.CampaignTargetMoney = res.TargetAmount.Value;

                    var MoneyType = (MoneyType)(string.IsNullOrEmpty(res.MoneyType) ? 0 : Convert.ToInt32(res.MoneyType));

                    Model.CampaignTargetMoneyType = MoneyType.DisplayName();


                    var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails.AsNoTracking() where S.StoryId == Id select S).FirstOrDefault();
                    if (Beneficiary != null)
                    {
                        CampainOrganizerViewModel ben = new CampainOrganizerViewModel();
                        ben = createViewModelOrganizer(Beneficiary);
                        ben.Id = Beneficiary.Id;
                        String str = ben.placeNmae != null ? ben.placeNmae : "";
                        String[] spearator = { "," };
                        String[] strlist = str.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                        var result = strlist.Reverse().Skip(2).Take(1).Reverse().ToArray();
                        var val = string.Join(", ", result);

                        Model.placeName = string.IsNullOrWhiteSpace(val) ? str : val;
                        Model.Latitude = ben.Latitude;
                        Model.Longitude = ben.longitude;
                        Model.FullplaceName = ben.placeNmae != null ? ben.placeNmae : "";
                        //Model.BDisplayPic = ben.BDisplayPic;
                        // string key = ConfigurationManager.AppSettings["BasicUrl"];
                        Model.BDisplayPicPath = ben.BDisplayPicPath;
                        string key = ConfigurationManager.AppSettings["BasicUrl"];
                        Model.BDisplayPicPath = (key + ben.BDisplayPicPath);
                        ben.BDisplayPicPath = (key + ben.BDisplayPicPath);
                        //var context = HttpContext.Current;
                        //string filePath = context.Server.MapPath("~/Uploads/Images/" + "ed2.jpg");
                        //Model.BDisplayPicPath = "https://givingactuallyblob.blob.core.windows.net/campaign1/5f20ebf4-76ec-4b56-91d4-3cb3420b7716.jpg";
                    }


                    var description = (from S in Entity.Tbl_CampaignDescription.AsNoTracking() where S.StoryId == Id select S).FirstOrDefault();
                    if (description != null)
                    {
                        CampaignDescription desc = new CampaignDescription();
                        Model.CampaignDescriptionDtl = description.storyDescription;
                        var stripeddesc = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                        Model.CampaignDescription = stripeddesc;
                        if (res.IsApprovedbyAdmin)
                            Model.CampaignStatus = "Active";
                        else
                            Model.CampaignStatus = "Pending Approval";

                        if (!res.Status)
                            Model.CampaignStatus = "InActive";
                    }
                    List<CampaignDonation> donationList = new List<CampaignDonation>();
                    var donations = (from S in Entity.Tbl_CampaignDonation.AsNoTracking() where S.StoryId == Id && S.isPaid == true select S).ToList();
                    if (donations.Any())
                    {
                        CampaignDonation donationval = new CampaignDonation();
                        decimal RaisedAmt = 0;
                        long RaisedBy = 0;
                        List<string> donrList = new List<string>();
                        foreach (var dntion in donations)
                        {
                            RaisedAmt = RaisedAmt + dntion.DonationAmnt;
                            RaisedBy++;
                            donationList.Add(new CampaignDonation() { DonatedBy = dntion.DonatedBy, isAnanymous = dntion.isAnanymous.Value, DonationAmnt = dntion.DonationAmnt, DonatedOn = dntion.DonatedOn.Value });
                            donrList.Add(dntion.Email);
                        }

                        decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                        var raisedPerc = Model.CampaignTargetMoney != 0 ? (RaisedAmt / Model.CampaignTargetMoney) * 100 : 0;
                        RaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;
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
        public async Task<List<CampaignModel>> GetCamapignsForListView_Tuned(List<int> CampaignIds)
        {
            try
            {
                List<CampaignModel> ModelList = new List<CampaignModel>();

                var query = from item in Entity.Tbl_Campaign.Include(a => a.Tbl_BeneficiaryDetails).AsNoTracking()
                           .Include(a => a.Tbl_CampaignDescription).AsNoTracking()
                           .Include(a => a.Tbl_CampaignDonation).AsNoTracking()
                           .Include(a => a.Tbl_User).AsNoTracking()
                            select item;
                var resultcampaigns = await query.ToListAsync();

                foreach (int CamapignId in CampaignIds)
                {
                    CampaignModel Model = new CampaignModel();
                    var res = resultcampaigns.Where(a => a.Id == CamapignId).FirstOrDefault();
                    Model.Id = res.Id;
                    var StoryCategory = (StoryCategory)res.Category;
                    Model.CategoryName = Enum.GetName(typeof(StoryCategory), res.Category);//StoryCategory.DisplayName();
                    Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                    Model.CampaignTitle = res.Title != null ? res.Title : "";
                    Model.CampaignStatus = "Draft";
                    Model.UserId = res.UserId;
                    Model.CreatedBy = res.CreatedBy;



                    Model.CampaignTargetMoney = res.TargetAmount.Value;
                    //var MoneyType = (MoneyType)Enum.Parse(typeof(MoneyType), Enum.GetName(typeof(MoneyType), Convert.ToInt32(res.MoneyType)), true);
                    int diff2 = (res.TargetDate != null) ? ((res.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                    if (res.TargetDate != null && res.TargetDate.Value.Date == DateTime.Now.Date)
                    {
                        Model.DaysLeft = diff2;
                    }
                    else
                    {
                        Model.DaysLeft = diff2 + 1;
                    }
                    Model.OrganizerName = res.CreatedUserName;
                    Model.OrganizerPictureUrl = "";
                    Model.Totalsupporters = res.CountUsage;
                    Model.CampaignTargetDate = res.TargetDate != null ? res.TargetDate.Value : DateTime.Now.AddDays(31);
                    Model.CampaignTargetMoneyType = res.MoneyType;// MoneyType.DisplayName();


                    var Beneficiary = res.Tbl_BeneficiaryDetails.FirstOrDefault();
                    if (Beneficiary != null)
                    {
                        CampainOrganizerViewModel ben = new CampainOrganizerViewModel();
                        ben = createViewModelOrganizer(Beneficiary);
                        ben.Id = Beneficiary.Id;
                        string key = ConfigurationManager.AppSettings["BasicUrl"];

                        Model.BDisplayPicPath = (key + ben.BDisplayPicPath);
                        ben.BDisplayPicPath = (key + ben.BDisplayPicPath);
                    }
                    // var resFiles = (from F in Entity.Tbl_StoriesAttachment where F.StoryId == Id select F).ToList();

                    var description = res.Tbl_CampaignDescription.FirstOrDefault();
                    //  (from S in Entity.Tbl_CampaignDescription where S.StoryId == Id select S).FirstOrDefault();
                    if (description != null)
                    {
                        CampaignDescription desc = new CampaignDescription();
                        Model.CampaignDescriptionDtl = description.storyDescription;
                        var stripeddesc = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                        Model.CampaignDescription = stripeddesc;
                        if (res.IsApprovedbyAdmin)
                            Model.CampaignStatus = "Active";
                        else
                            Model.CampaignStatus = "Pending Approval";

                        if (!res.Status)
                            Model.CampaignStatus = "InActive";
                    }



                    List<CampaignDonation> donationList = new List<CampaignDonation>();

                    var donResult = res.Tbl_CampaignDonation.Where(a => a.isPaid == true).GroupBy(x => new { x.Email, x.isAnanymous }).Select(g => new QueryResult()
                    {
                        SumDonation = g.Sum(x => x.DonationAmnt),
                        isAnanymous = g.Key.isAnanymous.Value,
                        Email = g.Key.Email,
                        DonatedBy = g.FirstOrDefault().DonatedBy,
                        DonatedOn = g.FirstOrDefault().DonatedOn.Value
                    });
                    if (donResult.Any())
                    {
                        CampaignDonation donationval = new CampaignDonation();
                        decimal RaisedAmt = 0;
                        long RaisedBy = 0;
                        List<string> donrList = new List<string>();
                        foreach (var dntion in donResult)
                        {
                            RaisedAmt = RaisedAmt + dntion.SumDonation;
                            RaisedBy++;
                            donrList.Add(dntion.Email);
                            donationList.Add(new CampaignDonation() { DonatedBy = (dntion.isAnanymous) ? "Anonymous" : dntion.DonatedBy, isAnanymous = dntion.isAnanymous, DonationAmnt = dntion.SumDonation, DonatedOn = dntion.DonatedOn });
                        }

                        decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                        var raisedPerc = (RaisedAmt / Model.CampaignTargetMoney) * 100;
                        Model.RaisedAmount = RaisedAmt;
                        RaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);



                    }

                    ModelList.Add(Model);


                }
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public UserCampaignModel GetCamapignsForUsersListView(int Id)
        {
            try
            {
                UserCampaignModel Model = new UserCampaignModel();

                var res = (from S in Entity.Tbl_Campaign.AsNoTracking() where S.Id == Id select S).FirstOrDefault();
                if (res != null)
                {
                    Model.Id = res.Id;
                    var StoryCategory = (StoryCategory)res.Category;
                    Model.CategoryName = StoryCategory.DisplayName();
                    Model.CampaignStatus = "Draft";
                    Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                    Model.CampaignTitle = res.Title != null ? res.Title : "";
                    Model.UserId = res.UserId;
                    Model.CreatedBy = res.CreatedBy;
                    Model.CreatedOn = res.CreatedOn.Value;

                    Model.CampaignTargetMoney = res.TargetAmount.Value;
                    var MoneyType = (MoneyType)(string.IsNullOrEmpty(res.MoneyType) ? 0 : Convert.ToInt32(res.MoneyType));
                    int diff2 = (res.TargetDate != null) ? ((res.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                    if (res.TargetDate != null && res.TargetDate.Value.Date == DateTime.Now.Date)
                    {
                        Model.DaysLeft = diff2;
                    }
                    else
                    {
                        Model.DaysLeft = diff2 + 1;
                    }
                    Model.OrganizerName = res.CreatedUserName;
                    var userdetail = (from cm in Entity.Tbl_User.AsNoTracking()
                                      where cm.Id == res.UserId
                                      select new
                                      {
                                          UserDPImage = cm.DPPath
                                      }).FirstOrDefault();

                    Model.UserDPImage = userdetail.UserDPImage;
                    Model.OrganizerPictureUrl = userdetail.UserDPImage;
                    Model.Totalsupporters = res.CountUsage;
                    Model.CampaignTargetDate = res.TargetDate != null ? res.TargetDate.Value : DateTime.Now.AddDays(31);
                    Model.CampaignTargetMoneyType = MoneyType.DisplayName();
                    // Enum.GetName(typeof(MoneyType), MoneyType);

                    var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails.AsNoTracking() where S.StoryId == Id select S).FirstOrDefault();
                    if (Beneficiary != null)
                    {
                        CampainOrganizerViewModel ben = new CampainOrganizerViewModel();
                        ben = createViewModelOrganizer(Beneficiary);
                        ben.Id = Beneficiary.Id;
                        String str = ben.placeNmae != null ? ben.placeNmae : "";
                        String[] spearator = { "," };
                        String[] strlist = str.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                        var result = strlist.Reverse().Skip(2).Take(1).Reverse().ToArray();
                        var val = string.Join(", ", result);

                        Model.placeName = string.IsNullOrWhiteSpace(val) ? str : val;
                        Model.FullplaceName = ben.placeNmae != null ? ben.placeNmae : "";
                        Model.Latitude = ben.Latitude;
                        Model.Longitude = ben.longitude;
                        Model.BDisplayPicPath = ben.BDisplayPicPath;
                        string key = ConfigurationManager.AppSettings["BasicUrl"];
                        Model.BDisplayPicPath = (key + ben.BDisplayPicPath);
                        ben.BDisplayPicPath = (key + ben.BDisplayPicPath);
                    }


                    var description = (from S in Entity.Tbl_CampaignDescription.AsNoTracking() where S.StoryId == Id select S).FirstOrDefault();
                    if (description != null)
                    {
                        CampaignDescription desc = new CampaignDescription();
                        Model.CampaignDescriptionDtl = description.storyDescription;
                        var stripeddesc = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                        Model.CampaignDescription = stripeddesc;
                        if (res.IsApprovedbyAdmin)
                            Model.CampaignStatus = "Active";
                        else
                            Model.CampaignStatus = "Pending Approval";

                        if (!res.Status)
                            Model.CampaignStatus = "InActive";
                    }


                    var Cmnts = (from S in Entity.Tbl_ParentComment.AsNoTracking() where S.StoryId == Id select S).ToList();
                    if (Cmnts.Any())
                    {
                        Model.CommentTotalCount = Cmnts.Count();
                    }

                    var endorseresult = (from S in Entity.Tbl_Endorse.AsNoTracking()
                                         where S.CampaignId == Id
                                         select S).ToList();
                    if (endorseresult.Any())
                    {
                        Model.EndorseTotalCount = endorseresult.Count();
                    }

                    var shares = (from S in Entity.Tbl_Shares.AsNoTracking() where S.StoryId == Id select S).ToList().Count();
                    Model.ShareTotalCount = shares;

                    var bank = (from S in Entity.tbl_CmpnBenBankDetails.AsNoTracking() where S.campaignId == Id select new { S.id,S.IsApprovedByAdmin, S.IFSC }).FirstOrDefault();
                    Model.isBankAdded = bank != null ? true : false;
                    Model.isBankVerified = bank != null ? (bank.IsApprovedByAdmin == 1 ? true : false) : false;

                    var withdraw = (from S in Entity.Tbl_CampaignWithdrawRequest.AsNoTracking() where S.CampaignId == Id group S by S.CampaignId into g select new { g.Key, total = g.Sum(x => x.withdrawalAmount) }).FirstOrDefault();
                    Model.iswithdrawalavailable = withdraw != null ? true : false;
                    Model.WithDrawnAmount = withdraw != null ? withdraw.total : 0;

                    var likes = (from S in Entity.Tbl_Like where S.StoryId == Id select S).ToList().Count();
                    Model.LikesTotalCount = likes;
                    List<CampaignDonation> donationList = new List<CampaignDonation>();
                    var donations = (from S in Entity.Tbl_CampaignDonation.AsNoTracking() where S.StoryId == Id && S.isPaid == true select S).ToList();
                    if (donations.Any())
                    {
                        CampaignDonation donationval = new CampaignDonation();
                        decimal RaisedAmt = 0;
                        long RaisedBy = 0;
                        List<string> donrList = new List<string>();
                        foreach (var dntion in donations)
                        {
                            RaisedAmt = RaisedAmt + dntion.DonationAmnt;
                            RaisedBy++;
                            donationList.Add(new CampaignDonation()
                            {
                                DonatedBy = (dntion.isAnanymous.Value) ? "Anonymous" : dntion.DonatedBy,
                                isAnanymous = dntion.isAnanymous.Value,
                                DonationAmnt = dntion.DonationAmnt,
                                DonatedOn = dntion.DonatedOn.Value,
                                TotolActualdonationAmnt = dntion.PaidDOnationAmt.Value,
                                TotolPaymentGSTFee = dntion.PayGSTAmnt.Value,
                                TotolPaymentProcFee = dntion.PayGSTAmnt.Value
                            });
                            donrList.Add(dntion.Email);
                        }
                        var received = donationList.Sum(a => a.TotolActualdonationAmnt);
                        Model.ELigibleAmtForWithdw = withdraw != null ? (received - withdraw.total) : received;

                        decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                        var raisedPerc = Model.CampaignTargetMoney != 0 ? (RaisedAmt / Model.CampaignTargetMoney) * 100 : 0;
                        Model.RaisedAmount = RaisedAmt;
                        Model.RaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;

                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);

                        Model.DonorsTotalCount = donations.GroupBy(a => a.DonatedBy).ToList().Count();
                    }



                }

                return Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<UserCampaignModelsList> GetCamapignsForUsersListView_tuned(List<int> CampaignIds)
        {
            try
            {
                UserCampaignModel Model = new UserCampaignModel();
                //join Pc in Entity.Tbl_ParentComment.AsNoTracking() on cm.Id equals Pc.StoryId into outerJoinPc
                //from Pc in outerJoinPc.DefaultIfEmpty()
                //join Sh in Entity.Tbl_Shares.AsNoTracking() on cm.Id equals Sh.StoryId into outerJoinSh
                //from Sh in outerJoinSh.DefaultIfEmpty()
                //join Lk in Entity.Tbl_Like.AsNoTracking() on cm.Id equals Lk.StoryId into outerJoinLk
                //from Lk in outerJoinLk.DefaultIfEmpty()
                //join en in Entity.Tbl_Endorse.AsNoTracking() on cm.Id equals en.CampaignId into outerJoinEn
                //from en in outerJoinEn.DefaultIfEmpty()
                var CampaignDetails = await (from cm in Entity.Tbl_Campaign.AsNoTracking()
                                             join Dc in Entity.Tbl_CampaignDescription.AsNoTracking() on cm.Id equals Dc.StoryId
                                             join ben in Entity.Tbl_BeneficiaryDetails.AsNoTracking() on cm.Id equals ben.StoryId
                                             join Cit in Entity.Tbl_CityDetails.AsNoTracking() on ben.BResidence equals Cit.CityId

                                             join Dn in Entity.Tbl_CampaignDonation.AsNoTracking() on cm.Id equals Dn.StoryId into outerJoinDon
                                             from Dn in outerJoinDon.DefaultIfEmpty()
                                             where CampaignIds.Contains(cm.Id)
                                             select new
                                             {
                                                 CampaignId = cm.Id,
                                                 cm.Category,
                                                 cm.IsApprovedbyAdmin,
                                                 cm.Title,
                                                 cm.UserId,
                                                 cm.CreatedBy,
                                                 cm.CreatedOn,
                                                 cm.MoneyType,
                                                 cm.Status,
                                                 cm.TargetAmount,
                                                 cm.TargetDate,
                                                 cm.CountUsage,
                                                 cm.CreatedUserName,
                                                 beneficiaryId = ben != null ? ben.Id : 0,
                                                 DPPath = ben != null ? ben.DPPath : "",
                                                 BResidence = ben != null ? ben.BResidence : 0,
                                                 CityName = Cit != null ? Cit.CityName : "",
                                                 Latitude = Cit != null ? Cit.Latitude : "",
                                                 longitude = Cit != null ? Cit.longitude : "",
                                                 DPName = ben != null ? ben.DPName : "",
                                                 DescId = Dc != null ? Dc.Id : 0,
                                                 storyDescription = Dc != null ? Dc.storyDescription : "",
                                                 DonationId = Dn != null ? Dn.Id : 0,
                                                 DonatedBy = Dn != null ? Dn.DonatedBy : "",
                                                 DonatedOn = Dn != null ? Dn.DonatedOn : DateTime.Now,
                                                 isAnanymous = Dn != null ? Dn.isAnanymous : true,
                                                 DonationAmnt = Dn != null ? Dn.DonationAmnt : 0,
                                                 isPaid = Dn != null ? Dn.isPaid : false
                                             }).ToListAsync();
                UserCampaignModelsList camList = new UserCampaignModelsList();
                foreach (int id in CampaignIds)
                {
                    // var res = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                    var rescam = CampaignDetails.Where(a => a.CampaignId == id).ToList();
                    if (rescam.Any())
                    {
                        var res = rescam.FirstOrDefault();
                        Model.Id = res.CampaignId;
                        var StoryCategory = (StoryCategory)res.Category;
                        Model.CategoryName = StoryCategory.DisplayName();
                        Model.CampaignStatus = "Draft";
                        Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                        Model.CampaignTitle = res.Title != null ? res.Title : "";
                        Model.UserId = res.UserId;
                        Model.CreatedBy = res.CreatedBy;
                        Model.CreatedOn = res.CreatedOn.Value;

                        Model.CampaignTargetMoney = res.TargetAmount.Value;
                        int diff2 = (res.TargetDate != null) ? ((res.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                        if (res.TargetDate != null && res.TargetDate.Value.Date == DateTime.Now.Date)
                        {
                            Model.DaysLeft = diff2;
                        }
                        else
                        {
                            Model.DaysLeft = diff2 + 1;
                        }
                        Model.OrganizerName = res.CreatedUserName;
                        Model.OrganizerPictureUrl = "";
                        Model.Totalsupporters = res.CountUsage;
                        Model.CampaignTargetDate = res.TargetDate != null ? res.TargetDate.Value : DateTime.Now.AddDays(31);
                        var MoneyType = (MoneyType)(string.IsNullOrEmpty(res.MoneyType) ? 0 : Convert.ToInt32(res.MoneyType));

                        Model.CampaignTargetMoneyType = MoneyType.DisplayName();
                        // Enum.GetName(typeof(MoneyType), MoneyType);


                        if (res.beneficiaryId > 0)
                        {
                            String str = res.CityName != null ? res.CityName : "";
                            String[] spearator = { "," };
                            String[] strlist = str.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                            var result = strlist.Reverse().Skip(2).Take(1).Reverse().ToArray();
                            var val = string.Join(", ", result);

                            Model.placeName = string.IsNullOrWhiteSpace(val) ? str : val;
                            Model.FullplaceName = res.CityName != null ? res.CityName : "";
                            Model.Latitude = res.Latitude;
                            Model.Longitude = res.longitude;
                            if (!string.IsNullOrEmpty(res.DPPath))
                            {
                                string key = ConfigurationManager.AppSettings["BasicUrl"];
                                Model.BDisplayPicPath = (key + res.DPPath);
                            }
                        }
                        if (res.DescId > 0)
                        {
                            Model.CampaignDescriptionDtl = res.storyDescription;
                            var stripeddesc = res.storyDescription != null ? StripTagsCharArray(res.storyDescription) : "";
                            Model.CampaignDescription = stripeddesc;
                            if (res.IsApprovedbyAdmin)
                                Model.CampaignStatus = "Active";
                            else
                                Model.CampaignStatus = "Pending Approval";

                            if (!res.Status)
                                Model.CampaignStatus = "InActive";
                        }

                        var DonList = rescam.Where(x => x.isPaid == true).GroupBy(x => x.CampaignId)
                    .Select(g => new
                    {
                        DonatedBy = g.Key,
                        Total = g.Sum(x => x.DonationAmnt)
                    });

                        var DonorList = rescam.Where(x => x.isPaid == true)
                             .GroupBy(x => x.DonatedBy)
                             .Select(g => new
                             {
                                 DonatedBy = g.Key,
                                 Total = g.Sum(x => x.DonationAmnt)
                             });
                        var fewres = await (from item in Entity.Tbl_Campaign.Include(a => a.Tbl_Endorse).AsNoTracking()
                           .Include(a => a.Tbl_Like).AsNoTracking()
                           .Include(a => a.Tbl_ParentComment).AsNoTracking()
                           .Include(a => a.Tbl_Shares).AsNoTracking()
                                            where item.Id == id
                                            select item).FirstOrDefaultAsync();

                        if (fewres.Tbl_ParentComment != null)
                        {
                            Model.CommentTotalCount = fewres.Tbl_ParentComment.Count();
                        }
                        if (fewres.Tbl_Endorse != null)
                        {
                            Model.EndorseTotalCount = fewres.Tbl_Endorse.Count();
                        }
                        if (fewres.Tbl_Shares != null)
                        {
                            Model.ShareTotalCount = fewres.Tbl_Shares.Count();
                        }
                        if (fewres.Tbl_Like != null)
                        {
                            Model.LikesTotalCount = fewres.Tbl_Like.Count();
                        }


                        var RaisedAmt = DonList.Any() ? DonList.ToList().FirstOrDefault().Total : 0;
                        decimal difference = res.TargetAmount.Value - RaisedAmt;
                        var raisedPerc = res.TargetAmount != null ? (res.TargetAmount.Value != 0 ? ((RaisedAmt / res.TargetAmount.Value) * 100) : 0) : 0;
                        var RaisedBy = DonorList.Any() ? DonorList.Distinct().ToList().Count() : 0;
                        Model.RaisedAmount = RaisedAmt;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);

                        camList.CampaignLists.Add(Model);



                    }
                }
                return camList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
        #region search Campaigns
        public SerachModel GetCamapignsForSearchView(int Id, string searchtext)
        {
            try
            {
                CampaignModel Model = new CampaignModel();
                searchtext = searchtext.ToLower();

                var serachResult = (from C in Entity.Tbl_Campaign.AsNoTracking()
                                    where C.Id == Id
                                    select C).ToList();
                var modelresult = serachResult.Where(R =>
               ((R.NGOName != null ? R.NGOName.ToLower().Contains(searchtext) : false) ||
        (R.BGroupName != null ? R.BGroupName.ToLower().Contains(searchtext) : false) ||
        (R.BName != null ? R.BName.ToLower().Contains(searchtext) : false) ||
        (R.Title != null ? R.Title.ToLower().Contains(searchtext) : false) ||
        (R.CreatedBy != null ? R.CreatedBy.ToLower().Contains(searchtext) : false))).FirstOrDefault();
                var res = serachResult.FirstOrDefault();
                var StoryCy = (StoryCategory)res.Category;
                var CategoryName = StoryCy.DisplayName();

                var descriptionval = (from S in Entity.Tbl_CampaignDescription.AsNoTracking()
                                      where S.StoryId == Id
        && (S.storyDescription != null ? S.storyDescription.ToLower().Contains(searchtext) : false)
                                      select S).FirstOrDefault();
                var searchResultFound = modelresult != null || descriptionval != null || CategoryName.ToLower().Contains(searchtext);
                if (searchResultFound)
                {
                    if (res != null)
                    {
                        Model.Id = res.Id;
                        var StoryCategory = (StoryCategory)res.Category;
                        Model.CategoryName = StoryCategory.DisplayName();
                        Model.CampaignStatus = "Draft";
                        Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                        Model.CampaignTitle = res.Title != null ? res.Title : "";
                        Model.UserId = res.UserId;
                        Model.CreatedBy = res.CreatedBy;
                        Model.CreatedOn = res.CreatedOn.Value;
                        Model.CampaignTargetMoney = res.TargetAmount.Value;
                        Model.CampaignTargetDate = res.TargetDate != null ? res.TargetDate.Value : DateTime.Now.AddDays(31);
                        var MoneyType = (MoneyType)(string.IsNullOrEmpty(res.MoneyType) ? 0 : Convert.ToInt32(res.MoneyType));
                        int diff2 = (res.TargetDate != null) ? ((res.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                        if (res.TargetDate != null && res.TargetDate.Value.Date == DateTime.Now.Date)
                        {
                            Model.DaysLeft = diff2;
                        }
                        else
                        {
                            Model.DaysLeft = diff2 + 1;
                        }
                        Model.OrganizerName = res.CreatedUserName;
                        Model.OrganizerPictureUrl = "";
                        Model.Totalsupporters = res.CountUsage;
                        Model.CampaignTargetMoneyType = MoneyType.DisplayName();
                        var description = (from S in Entity.Tbl_CampaignDescription.AsNoTracking() where S.StoryId == Id select S).FirstOrDefault();
                        if (description != null)
                        {
                            CampaignDescription desc = new CampaignDescription();
                            Model.CampaignDescriptionDtl = description.storyDescription;
                            var stripeddesc = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                            Model.CampaignDescription = stripeddesc;
                            if (res.IsApprovedbyAdmin)
                                Model.CampaignStatus = "Active";
                            else
                                Model.CampaignStatus = "Pending Approval";

                            if (!res.Status)
                                Model.CampaignStatus = "InActive";
                        }
                        List<CampaignDonation> donationList = new List<CampaignDonation>();
                        var donations = (from S in Entity.Tbl_CampaignDonation.AsNoTracking() where S.StoryId == Id && S.isPaid == true select S).ToList();
                        if (donations.Any())
                        {
                            CampaignDonation donationval = new CampaignDonation();
                            decimal RaisedAmt = 0;
                            long RaisedBy = 0;
                            List<string> donrList = new List<string>();
                            foreach (var dntion in donations)
                            {
                                RaisedAmt = RaisedAmt + dntion.DonationAmnt;
                                RaisedBy++;
                                donationList.Add(new CampaignDonation() { DonatedBy = dntion.DonatedBy, isAnanymous = dntion.isAnanymous.Value, DonationAmnt = dntion.DonationAmnt, DonatedOn = dntion.DonatedOn.Value });
                                donrList.Add(dntion.Email);
                            }
                            RaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;
                            decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                            var raisedPerc = Model.CampaignTargetMoney != 0 ? (RaisedAmt / Model.CampaignTargetMoney) * 100 : 0;

                            Model.RaisedAmount = RaisedAmt;
                            Model.RaisedBy = RaisedBy;
                            Model.RaisedPercentage = Convert.ToInt32(raisedPerc);
                        }

                    }
                    SerachModel returnResult = new SerachModel();
                    returnResult.Model = Model;
                    returnResult.isEMptyModel = false;
                    return returnResult;
                }
                SerachModel returnResult1 = new SerachModel();
                returnResult1.Model = Model;
                returnResult1.isEMptyModel = true;
                return returnResult1;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<CampaignModelsList> SearchCampaigns(int page, int page_size, string searchtext)
        {
            try
            {
                CampaignModelsList ModelList = new CampaignModelsList();

                var matches = Entity.GetSearchCampaigns(searchtext);
                List<int> CampaignIdList = new List<int>();
                int maxRows = page_size;

                var query = from S in Entity.tbl_SearchCampaignResult.AsNoTracking().Where(s => s.SearchText == searchtext)
                            join item in Entity.Tbl_Campaign.AsNoTracking().Where(a => a.IsApprovedbyAdmin) on S.CampaignId equals item.Id
                            select S.CampaignId.Value;

                CampaignIdList = await query.ToListAsync();

                if (CampaignIdList.Any())
                {
                    var distinctItems = CampaignIdList.Distinct();

                    var CampaignListnew = distinctItems.Skip((page - 1) * maxRows)
                  .Take(maxRows).ToList();
                    //foreach (var campaignId in CampaignListnew)
                    //{
                    //    CampaignModel Model = new CampaignModel(); Model = await GetCamapignsForListView(campaignId);
                    //    ModelList.CampaignLists.Add(Model);
                    //}
                    if (CampaignListnew.Any())
                    {
                        var re = await GetCampaignsForList_Sp(CampaignListnew.OrderBy(s => s).ToList());
                        ModelList = re;
                    }
                    double pageCount = (double)((decimal)distinctItems.Count() / Convert.ToDecimal(page_size));
                    ModelList.PageCount = (int)Math.Ceiling(pageCount);
                    ModelList.TotalCampaigns = distinctItems.Count();
                    ModelList.CurrentPageIndex = page;
                }
                return ModelList;
            }
            catch (Exception ex)
            { throw ex; }
        }

        public async Task<CampaignModelsList> SearchCampaigns_sp(int page, int page_size, string searchtext)
        {
            try
            {
                int maxRows = page_size;
                int skip = (page - 1) * maxRows;
                int take = maxRows;
                List<int> CampaignIds = new List<int>();
                int Countval = 0;
                List<spres_getsearchRes> CampaignDetails = new List<spres_getsearchRes>();
                using (var dbContext = new GivingActuallyEntities())
                {
                    CampaignDetails = await dbContext.Database.SqlQuery<spres_getsearchRes>("EXEC GetSearchCampaignsResults {0},{1},{2}", searchtext, skip, take)
                 .ToListAsync();
                    CampaignIds = CampaignDetails.Select(a => a.Id).Distinct().OrderBy(a => a).ToList();
                    Countval = dbContext.GetSearchResultsCount(searchtext).FirstOrDefault().Value;
                }
                CampaignModelsList ModelList = new CampaignModelsList();


                foreach (int CamapignId in CampaignIds)
                {
                    CampaignModel Model = new CampaignModel();
                    var CurrentDetail = CampaignDetails.Where(a => a.Id == CamapignId).ToList();
                    if (CurrentDetail.Any())
                    {
                        var DonList = CurrentDetail.Where(x => x.isPaid == true).GroupBy(x => x.Id)
                    .Select(g => new
                    {
                        DonatedBy = g.Key,
                        Total = g.Sum(x => x.DonationAmnt)
                    });

                        var DonorList = CurrentDetail.Where(x => x.isPaid == true)
                             .GroupBy(x => x.DonatedBy)
                             .Select(g => new
                             {
                                 DonatedBy = g.Key,
                                 Total = g.Sum(x => x.DonationAmnt)
                             });

                        var CampaignDetail = CurrentDetail.FirstOrDefault();


                        Model.Id = CampaignDetail.Id;
                        var StoryCategory = (StoryCategory)CampaignDetail.Category;
                        Model.CategoryName = StoryCategory.DisplayName();
                        Model.CampaignStatus = "Draft";
                        Model.IsApprovedbyAdmin = CampaignDetail.IsApprovedbyAdmin;
                        Model.CampaignTitle = CampaignDetail.Title != null ? CampaignDetail.Title : "";
                        Model.UserId = CampaignDetail.UserId;
                        Model.CreatedBy = CampaignDetail.CreatedBy;
                        Model.CreatedOn = CampaignDetail.CreatedOn.Value;
                        Model.CampaignTargetMoney = CampaignDetail.TargetAmount.Value;
                        Model.CampaignTargetDate = CampaignDetail.TargetDate != null ? CampaignDetail.TargetDate.Value : DateTime.Now.AddDays(31);
                        var MoneyType = (MoneyType)(string.IsNullOrEmpty(CampaignDetail.MoneyType) ? 0 : Convert.ToInt32(CampaignDetail.MoneyType));
                        Model.CampaignTargetMoneyType = MoneyType.DisplayName();

                        int diff2 = (CampaignDetail.TargetDate != null) ? ((CampaignDetail.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                        if (CampaignDetail.TargetDate != null && CampaignDetail.TargetDate.Value.Date == DateTime.Now.Date)
                        {
                            Model.DaysLeft = diff2;
                        }
                        else
                        {
                            Model.DaysLeft = diff2 + 1;
                        }
                        Model.OrganizerName = CampaignDetail.CreatedUserName;

                        String[] spearator1 = { " " };
                        String[] strlist1 = CampaignDetail.CreatedUserName.Split(spearator1, StringSplitOptions.RemoveEmptyEntries);
                        List<string> initialList = new List<string>();
                        foreach (var i in strlist1)
                        {
                            initialList.Add(i.Substring(0, 1));
                        }
                        var result1 = initialList.ToArray();
                        var val1 = string.Join("", result1);

                        Model.DisplayInitial = val1;


                        Model.UserDPImage = CampaignDetail.UserDPImage;
                        Model.OrganizerPictureUrl = CampaignDetail.UserDPImage;
                        Model.Totalsupporters = CampaignDetail.CountUsage;


                        if (CampaignDetail.beneficiaryId > 0)
                        {
                            String str = CampaignDetail.CityName != null ? CampaignDetail.CityName : "";
                            String[] spearator = { "," };
                            String[] strlist = str.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                            var result = strlist.Reverse().Skip(2).Take(1).Reverse().ToArray();
                            var val = string.Join(", ", result);
                            Model.placeName = string.IsNullOrWhiteSpace(val) ? str : val;
                            Model.Latitude = CampaignDetail.Latitude;
                            Model.Longitude = CampaignDetail.longitude;
                            Model.FullplaceName = CampaignDetail.CityName != null ? CampaignDetail.CityName : "";
                            if (!string.IsNullOrEmpty(CampaignDetail.DPPath))
                            {
                                string key = ConfigurationManager.AppSettings["BasicUrl"];
                                Model.BDisplayPicPath = (key + CampaignDetail.DPPath);
                            }
                        }
                        if (CampaignDetail.DescId > 0)
                        {
                            Model.CampaignDescriptionDtl = CampaignDetail.storyDescription;
                            var stripeddesc = CampaignDetail.storyDescription != null ? StripTagsCharArray(CampaignDetail.storyDescription) : "";
                            Model.CampaignDescription = stripeddesc;
                            if (CampaignDetail.IsApprovedbyAdmin)
                                Model.CampaignStatus = "Active";
                            else
                                Model.CampaignStatus = "Pending Approval";

                            if (!CampaignDetail.Status)
                                Model.CampaignStatus = "InActive";
                        }

                        var RaisedAmt = DonList.Any() ? DonList.ToList().FirstOrDefault().Total : 0;
                        decimal difference = CampaignDetail.TargetAmount.Value - RaisedAmt.Value;
                        var raisedPerc = CampaignDetail.TargetAmount != null ? (CampaignDetail.TargetAmount.Value != 0 ? ((RaisedAmt / CampaignDetail.TargetAmount.Value) * 100) : 0) : 0;
                        var RaisedBy = DonorList.Any() ? DonorList.Distinct().ToList().Count() : 0;
                        Model.RaisedAmount = RaisedAmt.Value;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);

                        ModelList.CampaignLists.Add(Model);
                    }
                }


                double pageCount = (double)((decimal)Countval / Convert.ToDecimal(page_size));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);
                ModelList.TotalCampaigns = Countval;
                ModelList.CurrentPageIndex = page;


                return ModelList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CampaignModelsList GetSearchUserCampaigns(int CategoryId = -1, int page = 1, int page_size = 12, string searchtext = "")
        {
            try
            {
                int maxRows = page_size;
                CampaignModelsList ModelList = new CampaignModelsList();


                var result = (from S in Entity.Tbl_Campaign.AsNoTracking() select S).Where(s => CategoryId != -1 ? s.Category == CategoryId : true).ToList()
                     ;
                result = result.Where(S => S.Status && S.IsApprovedbyAdmin).ToList();
                int j = 0;
                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        CampaignModel Model = new CampaignModel();
                        var Search = GetCamapignsForSearchView(item.Id, searchtext);
                        if (!Search.isEMptyModel)
                        {
                            ModelList.CampaignLists.Add(Search.Model);
                            j++;
                        }
                        if (j == page_size)
                        { break; }
                    }
                }


                double pageCount = (double)((decimal)result.Count() / Convert.ToDecimal(maxRows));
                ModelList.PageCount = (int)Math.Ceiling(pageCount);
                ModelList.TotalCampaigns = result.Count();
                ModelList.CurrentPageIndex = page;
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Location near you
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

                var res = (from S in Entity.Tbl_GeoLocation.AsNoTracking() select S).Where(a => a.InputLon == location.Longitude && a.InputLat == location.Latitude).ToList();
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
        public async Task<CampaignModelsList> GetCampaignbyLoc(BaseViewModel model)
        {
            try
            {
                int maxRows = 12;
                CampaignModelsList ModelList = new CampaignModelsList();
                List<string> allowedcities = GetCityNamesBYStatistics(model);
                var CampaignsNearbyLocation = (from S in Entity.Tbl_BeneficiaryDetails.AsNoTracking() where allowedcities.Contains(S.BResidence.ToString()) select S.StoryId).ToList();
                int CategoryId = !string.IsNullOrEmpty(model.Category) ? Convert.ToInt16(model.Category) : -1;

                var res = (from S in Entity.Tbl_Campaign.AsNoTracking()
                           where CampaignsNearbyLocation.Contains(S.Id)
                           select S)
                           .Where(s => model.Category != "-1" ? s.Category == CategoryId : true).ToList();
                double pageCount = (double)((decimal)res.Count() / Convert.ToDecimal(maxRows));

                res = res.OrderByDescending(a => a.CreatedOn).Skip((model.page - 1) * maxRows)
                .Take(maxRows).ToList();

                foreach (var item in res)
                {
                    CampaignModel Model = new CampaignModel();
                    Model = await GetCamapignsForListView(item.Id);
                    Model.Id = item.Id;

                    ModelList.CampaignLists.Add(Model);
                }

                ModelList.PageCount = (int)Math.Ceiling(pageCount);

                ModelList.CurrentPageIndex = model.page;
                return ModelList;



            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        #endregion
        #region Get the campaign's detial based on Id
        public async Task<CampaignDetailModel> GetCamapignDetail(int Id, int UserId = 0)
        {
            try
            {
                // int UserId = UserSession.HasSession != false ? UserSession.UserId : 0;

                CampaignDetailModel Model = new CampaignDetailModel();

                var res = await (from S in Entity.Tbl_Campaign.AsNoTracking() where S.Id == Id select S).FirstOrDefaultAsync();
                if (res != null)
                {
                    //  var valu = res.Tbl_CampaignDonation.Where(s => s.isPaid == true);
                    Model.Id = res.Id;
                    var StoryCategory = (StoryCategory)res.Category;
                    Model.CategoryName = Enum.GetName(typeof(StoryCategory), res.Category);//StoryCategory.DisplayName();
                    Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                    Model.CampaignTitle = res.Title != null ? res.Title : "";
                    Model.CampaignStatus = "Draft";
                    Model.UserId = res.UserId;

                    Model.CampainOrganizer = new CampainOrganizerViewModel();


                    if (Model.UserId == UserId)
                        Model.loggedinUser = true;
                    Model.Status = res.Status;
                    Model.CreatedBy = res.CreatedBy;
                    Model.CreatedOn = res.CreatedOn;
                    Model.BGroupName = res.BGroupName;
                    Model.NGOName = res.NGOName;
                    Model.BenificiaryName = res.BGroupName;

                    var BeneficiaryType = (BeneficiaryType)Enum.Parse(typeof(BeneficiaryType), Enum.GetName(typeof(BeneficiaryType), res.BeneficiaryType.Value), true);
                    Model.BeneficiaryType = Enum.GetName(typeof(BeneficiaryType), res.BeneficiaryType.Value);//BeneficiaryType.DisplayName();
                    if (Model.BeneficiaryType == "Myself")
                    {

                        Model.BenificiaryName = res.CreatedUserName;
                        Model.BGroupName = res.CreatedUserName;
                    }

                    Model.CampaignTargetMoney = res.TargetAmount.Value;
                    //var MoneyType = (MoneyType)Enum.Parse(typeof(MoneyType), Enum.GetName(typeof(MoneyType), Convert.ToInt32(res.MoneyType)), true);

                    Model.CampaignTargetDate = res.TargetDate != null ? res.TargetDate.Value : DateTime.Now.AddDays(31);
                    Model.CampaignTargetMoneyType = res.MoneyType;// MoneyType.DisplayName();
                    int diff2 = (res.TargetDate != null && res.TargetDate != null) ? ((res.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                    if (res.TargetDate.Value.Date == DateTime.Now.Date)
                    {
                        Model.DaysLeft = diff2;
                    }
                    else
                    {
                        Model.DaysLeft = diff2 + 1;
                    }
                    Model.OrganizerName = res.CreatedUserName;
                    Model.OrganizerPictureUrl = "";
                    Model.Totalsupporters = res.CountUsage;
                    int i = 0;
                    var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails.AsNoTracking() where S.StoryId == Id select S).FirstOrDefault();
                    if (Beneficiary != null)
                    {
                        CampainOrganizerViewModel ben = new CampainOrganizerViewModel();
                        ben = createViewModelOrganizer(Beneficiary);
                        ben.Id = Beneficiary.Id;
                        Model.CampainOrganizer = ben;
                        Model.Latitude = ben.Latitude;
                        Model.Longitude = ben.longitude;
                        string key = ConfigurationManager.AppSettings["BasicUrl"];

                        Model.BDisplayPicPath = (key + ben.BDisplayPicPath);
                        ben.BDisplayPicPath = (key + ben.BDisplayPicPath);
                        //if (ben.BDisplayPic != null)
                        //{
                        //    Files files = new Files();
                        //    files.File = ben.BDisplayPic;
                        //    files.FileName = ben.BDisplayPicName;
                        //    files.ContentType = "";
                        //    files.Index = 0;
                        //    Model.UploadedImages.Add(files);
                        // //   Model.BDisplayPic= ben.BDisplayPic;
                        //    var context = HttpContext.Current;
                        //    string filePath = context.Server.MapPath("~/Uploads/Images/" + "ed2.jpg");
                        //    Model.BDisplayPicPath = "https://givingactuallyblob.blob.core.windows.net/campaign1/5f20ebf4-76ec-4b56-91d4-3cb3420b7716.jpg";
                        //    i++;
                        //}
                    }
                    var resFiles = (from F in Entity.Tbl_StoriesAttachment.AsNoTracking() where F.StoryId == Id select F).ToList();
                    if (resFiles.Any())
                    {

                        foreach (var fItem in resFiles)
                        {
                            Files files = new Files();
                            //   files.File = fItem.MediaFile;
                            files.FileName = fItem.FileName;
                            files.ContentType = fItem.ContentType;
                            files.AttId = fItem.Id;


                            string key = ConfigurationManager.AppSettings["BasicUrl"];
                            files.FilePath = (key + fItem.DPPath);


                            //                       files.FilePath = fItem.DPPath != null ? fItem.DPPath : "";

                            //                       if (fItem.ContentType == "video/mp4")
                            //                       {
                            //                           var storageAccount = CloudStorageAccount.Parse(
                            //ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);
                            //                           var container = storageAccount.CreateCloudBlobClient().GetContainerReference("videos");
                            //                           var blob = container.GetBlockBlobReference("Test.mp4");
                            //                           var sharedAccessSig = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
                            //                           {
                            //                               Permissions = SharedAccessBlobPermissions.Read,
                            //                               SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24)
                            //                           });

                            //                           var url = string.Format("{0}{1}", blob.Uri, sharedAccessSig);
                            //                           files.FilePath = url;
                            //                       }
                            files.Index = i;
                            Model.UploadedImages.Add(files);
                            i++;
                        }
                    }




                    var description = (from S in Entity.Tbl_CampaignDescription.AsNoTracking() where S.StoryId == Id select S).FirstOrDefault();
                    if (description != null)
                    {
                        CampaignDescription desc = new CampaignDescription();
                        desc.StoryDescription = description.storyDescription;
                        desc.Id = description.Id;
                        desc.StripedDescription = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                        Model.campaignDescription = desc;
                        if (res.IsApprovedbyAdmin)
                            Model.CampaignStatus = "Active";
                        else
                            Model.CampaignStatus = "Pending Approval";

                        if (!res.Status)
                            Model.CampaignStatus = "InActive";
                    }

                    var updates = (from S in Entity.Tbl_CampaignDescriptionUpdates.AsNoTracking() where S.StoryId == Id select S).ToList();
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

                                var uptFiles = (from F in Entity.Tbl_UpdatesAttachment.AsNoTracking() where F.StoryId == Id && F.UpdateId == update.Id select F).ToList();
                                if (uptFiles.Any())
                                {

                                    List<Files> filesList = new List<Files>();
                                    foreach (var fItem in uptFiles)
                                    {
                                        Files files = new Files();
                                        // files.File = fItem.MediaFile;

                                        string key = ConfigurationManager.AppSettings["BasicUrl"];
                                        files.FilePath = (key + fItem.DPPath);

                                        //  files.FilePath = fItem.DPPath != null ? fItem.DPPath : "";
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

                    var Cmnts = (from S in Entity.Tbl_ParentComment.AsNoTracking() where S.StoryId == Id select S).ToList();
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
                    var endorseresult = (from S in Entity.Tbl_Endorse.AsNoTracking()
                                         where S.CampaignId == Id
                                         select S).ToList();
                    ENdorsementList list = new ENdorsementList();
                    List<Endorsement> individuallist = new List<Endorsement>();
                    foreach (var val in endorseresult)
                    {
                        if (val.NGOId == UserId)
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

                    var isendorsed = (from S in Entity.Tbl_Endorse.AsNoTracking()
                                      where S.CampaignId == Id && S.NGOId == UserId
                                      select S).ToList();
                    Model.IsEndorsedByUser = false;
                    if (isendorsed.Any())
                    {
                        Model.IsEndorsedByUser = true;
                    }
                    var shares = (from S in Entity.Tbl_Shares.AsNoTracking() where S.StoryId == Id select S).Count();
                    Model.sharecount = shares;

                    var likes = (from S in Entity.Tbl_Like.AsNoTracking() where S.StoryId == Id select S).Count();
                    Model.LikeCount = likes;
                    //  int UserId = UserSession.UserId;
                    var likebyuser = (from S in Entity.Tbl_Like.AsNoTracking()
                                      where S.StoryId == Id && S.UserId == UserId
                                      select S).FirstOrDefault();
                    if (likebyuser != null)
                        Model.IsSupportedByCtUser = true;
                    List<CampaignDonation> donationList = new List<CampaignDonation>();
                    //  var donations = (from S in Entity.Tbl_CampaignDonation where S.StoryId == Id && S.isPaid == true select S).ToList().OrderByDescending(a => a.DonationAmnt); ;


                    var query = from record in Entity.Tbl_CampaignDonation.AsNoTracking()
                                where record.StoryId == Id && record.isPaid == true
                                group record by new
                                {
                                    record.Email,
                                    record.isAnanymous
                                } into g

                                select new QueryResult
                                {
                                    SumDonation = g.Sum(x => x.DonationAmnt),
                                    SumReceivedDonation = g.Sum(x => x.PaidDOnationAmt.Value),
                                    SumGST = g.Sum(x => x.PayGSTAmnt.Value),
                                    SumProcessingFee = g.Sum(x => x.PayProcessingFeeAmnt.Value),
                                    isAnanymous = g.Key.isAnanymous.Value,
                                    DonatedBy = g.FirstOrDefault().DonatedBy,
                                    DonatedOn = g.FirstOrDefault().DonatedOn.Value,
                                    Email = g.Key.Email
                                }
                               ;
                    var withdraw = (from S in Entity.Tbl_CampaignWithdrawRequest.AsNoTracking() where S.CampaignId == Id group S by S.CampaignId into g select new { g.Key, total = g.Sum(x => x.withdrawalAmount) }).FirstOrDefault();
                    Model.iswithdrawalavailable = withdraw != null ? true : false;
                    Model.WithDrawnAmount = withdraw != null ? withdraw.total : 0;
                    // Model.ELigibleAmtForWithdw = withdraw != null ? (Model.CampaignTargetMoney - withdraw.total) : Model.CampaignTargetMoney;


                    query = query.AsNoTracking().OrderByDescending(a => a.SumDonation);
                    if (query.Any())
                    {
                        CampaignDonation donationval = new CampaignDonation();
                        decimal RaisedAmt = 0;
                        long RaisedBy = 0;
                        List<string> donrList = new List<string>();
                        foreach (var dntion in query)
                        {
                            RaisedAmt = RaisedAmt + dntion.SumDonation;
                            RaisedBy++;
                            donrList.Add(dntion.Email);
                            var donordetail = GetUserDetailbyName(dntion.Email);
                            donationList.Add(new CampaignDonation()
                            {
                                DonatedBy = (dntion.isAnanymous) ? "Anonymous" : dntion.DonatedBy,
                                donorDpPath = donordetail != null ? donordetail.DPPAth : "",
                                isAnanymous = dntion.isAnanymous,
                                DonationAmnt = dntion.SumDonation,
                                DonatedOn = dntion.DonatedOn,
                                TotolActualdonationAmnt = dntion.SumReceivedDonation,
                                TotolPaymentGSTFee = dntion.SumGST,
                                TotolPaymentProcFee = dntion.SumProcessingFee
                            });


                        }
                        var received = donationList.Sum(a => a.TotolActualdonationAmnt);
                        Model.ELigibleAmtForWithdw = withdraw != null ? (received - withdraw.total) : received;


                        decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                        var raisedPerc = (RaisedAmt / Model.CampaignTargetMoney) * 100;
                        Model.CampaignDonationList.AddRange(donationList);
                        Model.RaisedAmount = RaisedAmt;
                        RaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);
                        Model.CanEnableDonate = (RaisedAmt >= Model.CampaignTargetMoney) ? false : true;
                    }

                }
                // var cntry = GetUserCountryByIp();
                //  Model.CountryCode = cntry != null ? cntry.Name : "IN";
                //  Model.CurrencyCode = cntry != null ? cntry.CurrencySymbol : "";

                return Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<CampaignDetailModel> GetCamapignDetail_Sp(int Id, int UserId = 0)
        {
            try
            {

                CampaignDetailModel Model = new CampaignDetailModel();

                //var query = (from cm in Entity.Tbl_Campaign
                //                       join Dc in Entity.Tbl_CampaignDescription on cm.Id equals Dc.StoryId into outerJoinDc
                //                       from Dc in outerJoinDc.DefaultIfEmpty()
                //                       join ben in Entity.Tbl_BeneficiaryDetails on cm.Id equals ben.StoryId into outerJoinben
                //                       from ben in outerJoinben.DefaultIfEmpty()
                //                       join Cit in Entity.Tbl_CityDetails on ben.BResidence equals Cit.CityId into outerJoinCit
                //                       from Cit in outerJoinCit.DefaultIfEmpty()
                //                       join Dn in Entity.Tbl_CampaignDonation on cm.Id equals Dn.StoryId into outerJoinDon
                //                       from Dn in outerJoinDon.DefaultIfEmpty()

                //                       join Du in Entity.Tbl_CampaignDescriptionUpdates on cm.Id equals Du.StoryId into outerJoinDu
                //                       from Du in outerJoinDu.DefaultIfEmpty()
                //                       join En in Entity.Tbl_Endorse on cm.Id equals En.CampaignId into outerJoinEn
                //                       from En in outerJoinEn.DefaultIfEmpty()
                //                       join li in Entity.Tbl_Like on cm.Id equals li.StoryId into outerJoinli
                //                       from li in outerJoinli.DefaultIfEmpty()
                //                       join Pa in Entity.Tbl_ParentComment on cm.Id equals Pa.StoryId into outerJoinPa
                //                       from Pa in outerJoinPa.DefaultIfEmpty()
                //                       join Sh in Entity.Tbl_Shares on cm.Id equals Sh.StoryId into outerJoinSh
                //                       from Sh in outerJoinSh.DefaultIfEmpty()
                //                       join At in Entity.Tbl_StoriesAttachment on cm.Id equals At.StoryId into outerJoinAt
                //                       from At in outerJoinAt.DefaultIfEmpty()
                //                       join Up in Entity.Tbl_UpdatesAttachment on cm.Id equals Up.StoryId into outerJoinUp
                //                       from Up in outerJoinUp.DefaultIfEmpty()
                //                       join Us in Entity.Tbl_User on cm.UserId equals Us.Id into outerJoinUs
                //                       from Us in outerJoinUs.DefaultIfEmpty()
                //                       where cm.Id == Id //&& (Dn.isPaid == true)
                //                       select new
                //                       {
                //                           CampaignId = cm.Id,
                //                           cm.Category,
                //                           cm.IsApprovedbyAdmin,
                //                           cm.Title,
                //                           cm.UserId,
                //                           cm.CreatedBy,
                //                           cm.CreatedOn,
                //                           cm.MoneyType,
                //                           cm.Status,
                //                           cm.TargetAmount,
                //                           beneficiaryId = ben != null ? ben.Id : 0,
                //                           DPPath = ben != null ? ben.DPPath : "",
                //                           BResidence = ben != null ? ben.BResidence : 0,
                //                           CityName = Cit != null ? Cit.CityName : "",
                //                           Latitude = Cit != null ? Cit.Latitude : "",
                //                           longitude = Cit != null ? Cit.longitude : "",
                //                           DPName = ben != null ? ben.DPName : "",
                //                           DescId = Dc != null ? Dc.Id : 0,
                //                           storyDescription = Dc != null ? Dc.storyDescription : "",
                //                           DonationId = Dn != null ? Dn.Id : 0,
                //                           DonatedBy = Dn != null ? Dn.DonatedBy : "",
                //                           DonatedOn = Dn != null ? Dn.DonatedOn : DateTime.Now,
                //                           isAnanymous = Dn != null ? Dn.isAnanymous : true,
                //                           DonationAmnt = Dn != null ? Dn.DonationAmnt : 0,
                //                           isPaid = Dn != null ? Dn.isPaid : false
                //                       });


                var query = from item in Entity.Tbl_Campaign.Include(a => a.Tbl_BeneficiaryDetails).AsNoTracking()
                            .Include(a => a.Tbl_CampaignDescription).AsNoTracking()
                            .Include(a => a.Tbl_CampaignDescriptionUpdates).AsNoTracking()
                            .Include(a => a.Tbl_CampaignDonation).AsNoTracking()
                            .Include(a => a.Tbl_Endorse).AsNoTracking()
                            .Include(a => a.Tbl_Like).AsNoTracking()
                            .Include(a => a.Tbl_ParentComment).AsNoTracking()
                            .Include(a => a.Tbl_Shares).AsNoTracking()
                            .Include(a => a.Tbl_StoriesAttachment).AsNoTracking()
                            .Include(a => a.Tbl_UpdatesAttachment).AsNoTracking()
                            .Include(a => a.Tbl_User).AsNoTracking()
                            where item.Id == Id
                            select item;


                var res = await query.FirstOrDefaultAsync();

                if (res != null)
                {
                    Model.Id = res.Id;
                    var StoryCategory = (StoryCategory)res.Category;
                    Model.CategoryName = Enum.GetName(typeof(StoryCategory), res.Category);//StoryCategory.DisplayName();
                    Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                    Model.CampaignTitle = res.Title != null ? res.Title : "";
                    Model.CampaignStatus = "Draft";
                    Model.UserId = res.UserId;

                    Model.CampainOrganizer = new CampainOrganizerViewModel();


                    if (Model.UserId == UserId)
                        Model.loggedinUser = true;
                    Model.Status = res.Status;
                    Model.CreatedBy = res.CreatedBy;
                    Model.CreatedOn = res.CreatedOn;
                    Model.BGroupName = res.BGroupName;
                    Model.NGOName = res.NGOName;
                    Model.BenificiaryName = res.BGroupName;

                    var BeneficiaryType = (BeneficiaryType)Enum.Parse(typeof(BeneficiaryType), Enum.GetName(typeof(BeneficiaryType), res.BeneficiaryType.Value), true);
                    Model.BeneficiaryType = Enum.GetName(typeof(BeneficiaryType), res.BeneficiaryType.Value);//BeneficiaryType.DisplayName();
                    if (Model.BeneficiaryType == "Myself")
                    {
                        //var userName = GetUserDetailbyName(res.CreatedBy);
                        //Model.BenificiaryName = userName.DisplayName;
                        //Model.BGroupName = userName.DisplayName;
                        var userName = res.Tbl_User;
                        Model.BenificiaryName = userName != null ? userName.Name : "";
                        Model.BGroupName = userName != null ? userName.Name : "";
                    }

                    Model.CampaignTargetMoney = res.TargetAmount.Value;
                    //var MoneyType = (MoneyType)Enum.Parse(typeof(MoneyType), Enum.GetName(typeof(MoneyType), Convert.ToInt32(res.MoneyType)), true);

                    Model.CampaignTargetDate = res.TargetDate != null ? res.TargetDate.Value : DateTime.Now.AddDays(31);
                    Model.CampaignTargetMoneyType = res.MoneyType;// MoneyType.DisplayName(); 
                    int diff2 = (res.TargetDate != null) ? ((res.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                    if (res.TargetDate != null && res.TargetDate.Value.Date == DateTime.Now.Date)
                    {
                        Model.DaysLeft = diff2;
                    }
                    else
                    {
                        Model.DaysLeft = diff2 + 1;
                    }
                    Model.OrganizerName = res.CreatedUserName;
                    Model.OrganizerPictureUrl = "";
                    Model.Totalsupporters = res.CountUsage;
                    int i = 0;
                    //var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails where S.StoryId == Id select S).FirstOrDefault();
                    var Beneficiary = res.Tbl_BeneficiaryDetails.FirstOrDefault();
                    if (Beneficiary != null)
                    {
                        CampainOrganizerViewModel ben = new CampainOrganizerViewModel();
                        ben = createViewModelOrganizer(Beneficiary);
                        ben.Id = Beneficiary.Id;
                        Model.CampainOrganizer = ben;
                        Model.Latitude = ben.Latitude;
                        Model.Longitude = ben.longitude;
                        string key = ConfigurationManager.AppSettings["BasicUrl"];

                        Model.BDisplayPicPath = (key + ben.BDisplayPicPath);
                        ben.BDisplayPicPath = (key + ben.BDisplayPicPath);
                    }
                    // var resFiles = (from F in Entity.Tbl_StoriesAttachment where F.StoryId == Id select F).ToList();
                    var resFiles = res.Tbl_StoriesAttachment;
                    if (resFiles.Any())
                    {

                        foreach (var fItem in resFiles)
                        {
                            Files files = new Files();
                            //   files.File = fItem.MediaFile;
                            files.FileName = fItem.FileName;
                            files.ContentType = fItem.ContentType;
                            files.AttId = fItem.Id;


                            string key = ConfigurationManager.AppSettings["BasicUrl"];
                            files.FilePath = (key + fItem.DPPath);
                            files.Index = i;
                            Model.UploadedImages.Add(files);
                            i++;
                        }
                    }
                    var description = res.Tbl_CampaignDescription.FirstOrDefault();
                    //  (from S in Entity.Tbl_CampaignDescription where S.StoryId == Id select S).FirstOrDefault();
                    if (description != null)
                    {
                        CampaignDescription desc = new CampaignDescription();
                        desc.StoryDescription = description.storyDescription;
                        desc.Id = description.Id;
                        desc.StripedDescription = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                        Model.campaignDescription = desc;
                        if (res.IsApprovedbyAdmin)
                            Model.CampaignStatus = "Active";
                        else
                            Model.CampaignStatus = "Pending Approval";

                        if (!res.Status)
                            Model.CampaignStatus = "InActive";
                    }

                    //   var updates = (from S in Entity.Tbl_CampaignDescriptionUpdates where S.StoryId == Id select S).ToList();
                    var updates = res.Tbl_CampaignDescriptionUpdates.ToList();
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

                                var uptFiles = res.Tbl_UpdatesAttachment.Where(a => a.UpdateId == update.Id).ToList();
                                if (uptFiles.Any())
                                {

                                    List<Files> filesList = new List<Files>();
                                    foreach (var fItem in uptFiles)
                                    {
                                        Files files = new Files();
                                        // files.File = fItem.MediaFile;

                                        string key = ConfigurationManager.AppSettings["BasicUrl"];
                                        files.FilePath = (key + fItem.DPPath);

                                        //  files.FilePath = fItem.DPPath != null ? fItem.DPPath : "";
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

                    var Cmnts = res.Tbl_ParentComment.ToList();
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




                    //var endorseresult = (from S in Entity.Tbl_Endorse
                    //                     where S.CampaignId == Id
                    //                     select S).ToList();
                    var endorseresult = res.Tbl_Endorse.ToList();
                    ENdorsementList list = new ENdorsementList();
                    List<Endorsement> individuallist = new List<Endorsement>();
                    foreach (var val in endorseresult)
                    {
                        if (val.NGOId == UserId)
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

                    var isendorsed = res.Tbl_Endorse.Where(S => S.NGOId == UserId).FirstOrDefault();
                    Model.IsEndorsedByUser = false;
                    if (isendorsed != null)
                    {
                        Model.IsEndorsedByUser = true;
                    }
                    var shares = res.Tbl_Shares.Count();
                    Model.sharecount = shares;

                    var likes = res.Tbl_Like.Count();
                    Model.LikeCount = likes;
                    //  int UserId = UserSession.UserId;
                    var likebyuser = res.Tbl_Like.Where(S => S.UserId == UserId).FirstOrDefault();

                    if (likebyuser != null)
                        Model.IsSupportedByCtUser = true;
                    List<CampaignDonation> donationList = new List<CampaignDonation>();

                    var donResult = res.Tbl_CampaignDonation.Where(a => a.isPaid == true).GroupBy(x => new { x.Email, x.isAnanymous }).Select(g => new QueryResult()
                    {
                        SumDonation = g.Sum(x => x.DonationAmnt),
                        isAnanymous = g.Key.isAnanymous.Value,
                        Email = g.Key.Email,
                        DonatedBy = g.FirstOrDefault().DonatedBy,
                        DonatedOn = g.FirstOrDefault().DonatedOn.Value
                    });
                    if (donResult.Any())
                    {
                        CampaignDonation donationval = new CampaignDonation();
                        decimal RaisedAmt = 0;
                        long RaisedBy = 0;
                        List<string> donrList = new List<string>();
                        foreach (var dntion in donResult)
                        {
                            RaisedAmt = RaisedAmt + dntion.SumDonation;
                            RaisedBy++;
                            donrList.Add(dntion.Email);
                            donationList.Add(new CampaignDonation() { DonatedBy = (dntion.isAnanymous) ? "Anonymous" : dntion.DonatedBy, isAnanymous = dntion.isAnanymous, DonationAmnt = dntion.SumDonation, DonatedOn = dntion.DonatedOn });
                        }

                        decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                        var raisedPerc = (RaisedAmt / Model.CampaignTargetMoney) * 100;
                        Model.CampaignDonationList.AddRange(donationList);
                        Model.RaisedAmount = RaisedAmt;
                        RaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);
                        Model.CanEnableDonate = (RaisedAmt >= Model.CampaignTargetMoney) ? false : true;
                    }

                }


                return Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<CampaignDetailModel> GetCamapignDetail_Tuned(int Id, int UserId = 0)
        {
            try
            {

                CampaignDetailModel Model = new CampaignDetailModel();
                //  var res = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                var query = from item in Entity.Tbl_Campaign.Include(a => a.Tbl_BeneficiaryDetails).AsNoTracking()
                            .Include(a => a.Tbl_CampaignDescription).AsNoTracking()
                            .Include(a => a.Tbl_CampaignDescriptionUpdates).AsNoTracking()
                            .Include(a => a.Tbl_CampaignDonation).AsNoTracking()
                            .Include(a => a.Tbl_Endorse).AsNoTracking()
                            .Include(a => a.Tbl_Like).AsNoTracking()
                            .Include(a => a.Tbl_ParentComment).AsNoTracking()
                            .Include(a => a.Tbl_Shares).AsNoTracking()
                            .Include(a => a.Tbl_StoriesAttachment).AsNoTracking()
                            .Include(a => a.Tbl_UpdatesAttachment).AsNoTracking()
                            where item.Id == Id
                            select item;
                var res = await query.FirstOrDefaultAsync();

                if (res != null)
                {
                    Model.Id = res.Id;
                    var StoryCategory = (StoryCategory)res.Category;
                    Model.CategoryName = Enum.GetName(typeof(StoryCategory), res.Category);//StoryCategory.DisplayName();
                    Model.IsApprovedbyAdmin = res.IsApprovedbyAdmin;
                    Model.CampaignTitle = res.Title != null ? res.Title : "";
                    Model.CampaignStatus = "Draft";
                    Model.UserId = res.UserId;

                    Model.CampainOrganizer = new CampainOrganizerViewModel();


                    if (Model.UserId == UserId)
                        Model.loggedinUser = true;
                    Model.Status = res.Status;
                    Model.CreatedBy = res.CreatedBy;
                    Model.CreatedOn = res.CreatedOn;
                    Model.BGroupName = res.BGroupName;
                    Model.NGOName = res.NGOName;
                    Model.BenificiaryName = res.BGroupName;

                    var BeneficiaryType = (BeneficiaryType)Enum.Parse(typeof(BeneficiaryType), Enum.GetName(typeof(BeneficiaryType), res.BeneficiaryType.Value), true);
                    Model.BeneficiaryType = Enum.GetName(typeof(BeneficiaryType), res.BeneficiaryType.Value);//BeneficiaryType.DisplayName();
                                                                                                             //  var userName = res.Tbl_User;
                    if (Model.BeneficiaryType == "Myself")
                    {
                        //var userName = GetUserDetailbyName(res.CreatedBy);
                        //Model.BenificiaryName = userName.DisplayName;
                        //Model.BGroupName = userName.DisplayName;

                        Model.BenificiaryName = res.CreatedUserName;
                        Model.BGroupName = res.CreatedUserName;
                    }

                    Model.CampaignTargetMoney = res.TargetAmount.Value;
                    //var MoneyType = (MoneyType)Enum.Parse(typeof(MoneyType), Enum.GetName(typeof(MoneyType), Convert.ToInt32(res.MoneyType)), true);

                    Model.CampaignTargetDate = res.TargetDate != null ? res.TargetDate.Value : DateTime.Now.AddDays(31);
                    Model.CampaignTargetMoneyType = res.MoneyType;// MoneyType.DisplayName();


                    int diff2 = (res.TargetDate != null) ? ((res.TargetDate.Value.Subtract(DateTime.Now).Days)) : 31;
                    if (res.TargetDate != null && res.TargetDate.Value.Date == DateTime.Now.Date)
                    {
                        Model.DaysLeft = diff2;
                    }
                    else
                    {
                        Model.DaysLeft = diff2 + 1;
                    }
                    Model.OrganizerName = res.CreatedUserName;
                    Model.OrganizerPictureUrl = "";
                    Model.Totalsupporters = res.CountUsage;
                    int i = 0;
                    //var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails where S.StoryId == Id select S).FirstOrDefault();
                    var Beneficiary = res.Tbl_BeneficiaryDetails.FirstOrDefault();
                    if (Beneficiary != null)
                    {
                        CampainOrganizerViewModel ben = new CampainOrganizerViewModel();
                        ben = createViewModelOrganizer(Beneficiary);
                        ben.Id = Beneficiary.Id;
                        Model.CampainOrganizer = ben;
                        Model.Latitude = ben.Latitude;
                        Model.Longitude = ben.longitude;
                        string key = ConfigurationManager.AppSettings["BasicUrl"];

                        Model.BDisplayPicPath = (key + ben.BDisplayPicPath);
                        ben.BDisplayPicPath = (key + ben.BDisplayPicPath);
                    }
                    // var resFiles = (from F in Entity.Tbl_StoriesAttachment where F.StoryId == Id select F).ToList();
                    var resFiles = res.Tbl_StoriesAttachment;
                    if (resFiles.Any())
                    {

                        foreach (var fItem in resFiles)
                        {
                            Files files = new Files();
                            //   files.File = fItem.MediaFile;
                            files.FileName = fItem.FileName;
                            files.ContentType = fItem.ContentType;
                            files.AttId = fItem.Id;


                            string key = ConfigurationManager.AppSettings["BasicUrl"];
                            files.FilePath = (key + fItem.DPPath);
                            files.Index = i;
                            Model.UploadedImages.Add(files);
                            i++;
                        }
                    }
                    var description = res.Tbl_CampaignDescription.FirstOrDefault();
                    //  (from S in Entity.Tbl_CampaignDescription where S.StoryId == Id select S).FirstOrDefault();
                    if (description != null)
                    {
                        CampaignDescription desc = new CampaignDescription();
                        desc.StoryDescription = description.storyDescription;
                        desc.Id = description.Id;
                        desc.StripedDescription = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                        Model.campaignDescription = desc;
                        if (res.IsApprovedbyAdmin)
                            Model.CampaignStatus = "Active";
                        else
                            Model.CampaignStatus = "Pending Approval";

                        if (!res.Status)
                            Model.CampaignStatus = "InActive";
                    }

                    //   var updates = (from S in Entity.Tbl_CampaignDescriptionUpdates where S.StoryId == Id select S).ToList();
                    var updates = res.Tbl_CampaignDescriptionUpdates.ToList();
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

                                var uptFiles = res.Tbl_UpdatesAttachment.Where(a => a.UpdateId == update.Id).ToList();
                                if (uptFiles.Any())
                                {

                                    List<Files> filesList = new List<Files>();
                                    foreach (var fItem in uptFiles)
                                    {
                                        Files files = new Files();
                                        // files.File = fItem.MediaFile;

                                        string key = ConfigurationManager.AppSettings["BasicUrl"];
                                        files.FilePath = (key + fItem.DPPath);

                                        //  files.FilePath = fItem.DPPath != null ? fItem.DPPath : "";
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

                    var Cmnts = res.Tbl_ParentComment.ToList();
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




                    //var endorseresult = (from S in Entity.Tbl_Endorse
                    //                     where S.CampaignId == Id
                    //                     select S).ToList();
                    var endorseresult = res.Tbl_Endorse.ToList();
                    ENdorsementList list = new ENdorsementList();
                    List<Endorsement> individuallist = new List<Endorsement>();
                    foreach (var val in endorseresult)
                    {
                        if (val.NGOId == UserId)
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

                    var isendorsed = res.Tbl_Endorse.Where(S => S.NGOId == UserId).FirstOrDefault();
                    Model.IsEndorsedByUser = false;
                    if (isendorsed != null)
                    {
                        Model.IsEndorsedByUser = true;
                    }
                    var shares = res.Tbl_Shares.Count();
                    Model.sharecount = shares;

                    var likes = res.Tbl_Like.Count();
                    Model.LikeCount = likes;
                    //  int UserId = UserSession.UserId;
                    var likebyuser = res.Tbl_Like.Where(S => S.UserId == UserId).FirstOrDefault();

                    if (likebyuser != null)
                        Model.IsSupportedByCtUser = true;
                    List<CampaignDonation> donationList = new List<CampaignDonation>();

                    var donResult = res.Tbl_CampaignDonation.Where(a => a.isPaid == true).GroupBy(x => new { x.Email, x.isAnanymous }).Select(g => new QueryResult()
                    {
                        SumDonation = g.Sum(x => x.DonationAmnt),
                        isAnanymous = g.Key.isAnanymous.Value,
                        DonatedBy = g.FirstOrDefault().DonatedBy,
                        Email = g.Key.Email,
                        DonatedOn = g.FirstOrDefault().DonatedOn.Value
                    });
                    if (donResult.Any())
                    {
                        CampaignDonation donationval = new CampaignDonation();
                        decimal RaisedAmt = 0;
                        long RaisedBy = 0;
                        List<string> donrList = new List<string>();
                        foreach (var dntion in donResult)
                        {
                            RaisedAmt = RaisedAmt + dntion.SumDonation;
                            RaisedBy++;
                            donrList.Add(dntion.Email);
                            donationList.Add(new CampaignDonation() { DonatedBy = (dntion.isAnanymous) ? "Anonymous" : dntion.DonatedBy, isAnanymous = dntion.isAnanymous, DonationAmnt = dntion.SumDonation, DonatedOn = dntion.DonatedOn });
                        }

                        decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                        var raisedPerc = (RaisedAmt / Model.CampaignTargetMoney) * 100;
                        Model.CampaignDonationList.AddRange(donationList);
                        Model.RaisedAmount = RaisedAmt;
                        RaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;
                        Model.RaisedBy = RaisedBy;
                        Model.RaisedPercentage = Convert.ToInt32(raisedPerc);
                        Model.CanEnableDonate = (RaisedAmt >= Model.CampaignTargetMoney) ? false : true;
                    }

                }


                return Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<CampaignDetailModel> GetCamapignDetailold(int Id, int UserId = 0)
        {
            try
            {

                var cam = new List<spres_get1cam>();
                var cam1 = new List<GetCampaigns2Test_Result>();

                using (var dbContext = new GivingActuallyEntities())
                {


                    //  var Tresults = await dbContext.Database.SqlQuery<spres_get1cam>("EXEC GetCampaigns1Test {0}, {1}", p1, p2)
                    //.ToArrayAsync();
                    var Tresults = await dbContext.Database.SqlQuery<GetCampaigns2Test_Result>("EXEC GetCampaigns2Test")
                   .ToListAsync();
                    //Get second result set
                    cam1.AddRange(Tresults);

                    return new CampaignDetailModel();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private UserModel GetUserDetailbyName(string createdBy)
        {
            try
            {
                UserModel ModelList = new UserModel();
                var res = (from S in Entity.Tbl_User.AsNoTracking() where S.UserName == createdBy select S).FirstOrDefault();
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
                    ModelList.UserDPImage = !(string.IsNullOrEmpty(res.DPPath)) ? res.DPPath : "";
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

        public CampaignMainViewModel GetCamapign(int Id)
        {
            try
            {
                // int UserId = UserSession.HasSession != false ? UserSession.UserId : 0;
                int UserId = 0;
                CampaignMainViewModel Model = new CampaignMainViewModel();
                var res = (from S in Entity.Tbl_Campaign.AsNoTracking() where S.Id == Id select S).FirstOrDefault();
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

                    //Model.CampaignTargetDate = res.TargetDate != null ? res.TargetDate.Value : DateTime.Now.AddDays(31);
                    Model.CampaignTargetMoneyType = res.MoneyType;
                    int i = 0;
                    var Beneficiary = (from S in Entity.Tbl_BeneficiaryDetails.AsNoTracking() where S.StoryId == Id select S).FirstOrDefault();
                    if (Beneficiary != null)
                    {
                        CampainOrganizerViewModel ben = new CampainOrganizerViewModel();
                        ben = createViewModelOrganizer(Beneficiary);
                        ben.Id = Beneficiary.Id;
                        Model.CampainOrganizer = ben;
                        Model.Latitude = ben.Latitude;
                        Model.Longitude = ben.longitude;
                        string key = ConfigurationManager.AppSettings["BasicUrl"];
                        ben.BDisplayPicPath = (key + ben.BDisplayPicPath);
                        //if (ben.BDisplayPic != null)
                        //{
                        //    Files files = new Files();
                        //    files.File = ben.BDisplayPic;
                        //    files.FileName = ben.BDisplayPicName;
                        //    files.ContentType = "";
                        //    files.Index = 0;
                        //    Model.Files.Add(files);
                        //    i++;
                        //}
                    }
                    var resFiles = (from F in Entity.Tbl_StoriesAttachment.AsNoTracking() where F.StoryId == Id select F).ToList();
                    if (resFiles.Any())
                    {

                        foreach (var fItem in resFiles)
                        {
                            Files files = new Files();
                            //files.File = fItem.MediaFile;
                            string key = ConfigurationManager.AppSettings["BasicUrl"];
                            files.FilePath = (key + fItem.DPPath);
                            //files.FilePath = fItem.DPPath != null ? fItem.DPPath : "";
                            files.FileName = fItem.FileName;
                            files.ContentType = fItem.ContentType;
                            files.AttId = fItem.Id;
                            files.Index = i;
                            Model.Files.Add(files);
                            i++;
                        }
                    }




                    var description = (from S in Entity.Tbl_CampaignDescription.AsNoTracking() where S.StoryId == Id select S).FirstOrDefault();
                    if (description != null)
                    {
                        CampaignDescription desc = new CampaignDescription();
                        desc.StoryDescription = description.storyDescription;
                        desc.Id = description.Id;
                        desc.StripedDescription = description.storyDescription != null ? StripTagsCharArray(description.storyDescription) : "";
                        Model.campaignDescription = desc;

                    }

                    var updates = (from S in Entity.Tbl_CampaignDescriptionUpdates.AsNoTracking() where S.StoryId == Id select S).ToList();
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

                                var uptFiles = (from F in Entity.Tbl_UpdatesAttachment.AsNoTracking() where F.StoryId == Id && F.UpdateId == update.Id select F).ToList();
                                if (uptFiles.Any())
                                {

                                    List<Files> filesList = new List<Files>();
                                    foreach (var fItem in uptFiles)
                                    {
                                        Files files = new Files();
                                        string key = ConfigurationManager.AppSettings["BasicUrl"];
                                        files.FilePath = (key + fItem.DPPath);
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

                    var Cmnts = (from S in Entity.Tbl_ParentComment.AsNoTracking() where S.StoryId == Id select S).ToList();
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




                    var endorseresult = (from S in Entity.Tbl_Endorse.AsNoTracking()
                                         where S.CampaignId == Id
                                         select S).ToList();
                    ENdorsementList list = new ENdorsementList();
                    List<Endorsement> individuallist = new List<Endorsement>();
                    foreach (var val in endorseresult)
                    {
                        if (val.NGOId == UserId)
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

                    var shares = (from S in Entity.Tbl_Shares.AsNoTracking() where S.StoryId == Id select S).ToList().Count();
                    Model.sharecount = shares;

                    var likes = (from S in Entity.Tbl_Like.AsNoTracking() where S.StoryId == Id select S).ToList().Count();
                    Model.LikeCount = likes;
                    //  int UserId = UserSession.UserId;
                    var likebyuser = (from S in Entity.Tbl_Like.AsNoTracking()
                                      where S.StoryId == Id && S.UserId == UserId
                                      select S).FirstOrDefault();
                    if (likebyuser != null)
                        Model.isLiked = true;
                    List<CampaignDonation> donationList = new List<CampaignDonation>();
                    var donations = (from S in Entity.Tbl_CampaignDonation.AsNoTracking() where S.StoryId == Id && S.isPaid == true select S).ToList().OrderByDescending(a => a.DonationAmnt); ;
                    if (donations.Any())
                    {
                        CampaignDonation donationval = new CampaignDonation();
                        decimal RaisedAmt = 0;
                        long RaisedBy = 0;
                        List<string> donrList = new List<string>();
                        foreach (var dntion in donations)
                        {
                            RaisedAmt = RaisedAmt + dntion.DonationAmnt;
                            RaisedBy++;
                            donrList.Add(dntion.Email);
                            donationList.Add(new CampaignDonation() { DonatedBy = dntion.DonatedBy, isAnanymous = dntion.isAnanymous.Value, DonationAmnt = dntion.DonationAmnt, DonatedOn = dntion.DonatedOn.Value });
                        }

                        decimal difference = Model.CampaignTargetMoney - RaisedAmt;
                        var raisedPerc = (RaisedAmt / Model.CampaignTargetMoney) * 100;
                        Model.CampaignDonationList.AddRange(donationList);
                        Model.RaisedAmount = RaisedAmt;
                        RaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;
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
        #endregion
        #region Add new Campaign details
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

        public int getLocationfromPolar(string Placename, string latitude, string longitude)
        {
            try
            {


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
        public async Task<int> CreateCampaignPhase1(CampaignVwModel1 viewModel)
        {
            try
            {

                if (viewModel.Id == 0)
                {
                    Tbl_Campaign NewCampaign = new Tbl_Campaign();

                    NewCampaign.UserId = viewModel.UserId;
                    NewCampaign.IsApprovedbyAdmin = false;
                    NewCampaign.Title = viewModel.CampaignTitle;
                    NewCampaign.Status = true;
                    NewCampaign.CreatedBy = viewModel.UserName;
                    NewCampaign.CreatedOn = DateTime.UtcNow;
                    StoryCategory cate = (StoryCategory)Enum.Parse(typeof(StoryCategory), viewModel.CategoryType);
                    int CateId = (int)cate;

                    NewCampaign.Category = CateId;
                    MoneyType MnyTyp = (MoneyType)Enum.Parse(typeof(MoneyType), viewModel.CampaignTargetMoneyType);
                    int MoneyType = (int)MnyTyp;

                    NewCampaign.MoneyType = MoneyType.ToString();
                    NewCampaign.TargetAmount = viewModel.CampaignTargetMoney;
                    NewCampaign.TargetDate = viewModel.CampaignTargetDate != null ? viewModel.CampaignTargetDate : DateTime.Now.AddDays(31);
                    BeneficiaryType BenTyp = (BeneficiaryType)Enum.Parse(typeof(BeneficiaryType), viewModel.BeneficiaryType);
                    int BenType = (int)BenTyp;

                    NewCampaign.BeneficiaryType = BenType;
                    NewCampaign.BName = viewModel.BName != null ? viewModel.BName : "";
                    NewCampaign.BGroupName = viewModel.BGroupName != null ? viewModel.BGroupName : "";
                    NewCampaign.NGOName = viewModel.NGOName != null ? viewModel.NGOName : "";


                    string CanApprove = ConfigurationManager.AppSettings["CanApprove"];
                    bool CanApprovalFlag = CanApprove == "true" ? true : false;
                    NewCampaign.IsApprovedbyAdmin = CanApprovalFlag;
                    NewCampaign.CreatedUserName = viewModel.UserDisplayName;
                    Entity.Tbl_Campaign.Add(NewCampaign);
                    await Entity.SaveChangesAsync();
                    var NewCampaignId = NewCampaign.Id;
                    return NewCampaignId;
                }
                else
                {
                    var ExistingCampaign = (from S in Entity.Tbl_Campaign where S.Id == viewModel.Id select S).FirstOrDefault();
                    if (ExistingCampaign != null)
                    {
                        ExistingCampaign.UserId = viewModel.UserId;
                        ExistingCampaign.Title = viewModel.CampaignTitle;
                        ExistingCampaign.Status = true;
                        StoryCategory cate = (StoryCategory)Enum.Parse(typeof(StoryCategory), viewModel.CategoryType);
                        int CateId = (int)cate;

                        ExistingCampaign.Category = CateId;
                        MoneyType MnyTyp = (MoneyType)Enum.Parse(typeof(MoneyType), viewModel.CampaignTargetMoneyType);
                        int MoneyType = (int)MnyTyp;

                        ExistingCampaign.MoneyType = MoneyType.ToString();
                        ExistingCampaign.TargetAmount = viewModel.CampaignTargetMoney;
                        ExistingCampaign.CreatedUserName = viewModel.UserDisplayName;
                        ExistingCampaign.TargetDate = viewModel.CampaignTargetDate != null ? viewModel.CampaignTargetDate : DateTime.Now.AddDays(31);
                        BeneficiaryType BenTyp = (BeneficiaryType)Enum.Parse(typeof(BeneficiaryType), viewModel.BeneficiaryType);
                        int BenType = (int)BenTyp;

                        ExistingCampaign.BeneficiaryType = BenType;
                        ExistingCampaign.BName = viewModel.BName != null ? viewModel.BName : "";
                        ExistingCampaign.BGroupName = viewModel.BGroupName != null ? viewModel.BGroupName : "";
                        ExistingCampaign.NGOName = viewModel.NGOName != null ? viewModel.NGOName : "";

                        ExistingCampaign.UpdatedBy = viewModel.UserName;
                        ExistingCampaign.UpdatedOn = DateTime.UtcNow;
                        await Entity.SaveChangesAsync();

                        return ExistingCampaign.Id;
                    }
                    else { return 0; }
                }
            }
            catch (Exception ex) { throw ex; }
        }
        public async Task<bool> CreateCampaignPhase2(CampaignPhase2Full viewModel)
        {
            try
            {
                var beneficiaryEntry = (from S in Entity.Tbl_BeneficiaryDetails where S.StoryId == viewModel.campaignId select S).FirstOrDefault();

                if (beneficiaryEntry == null)
                {
                    Tbl_BeneficiaryDetails beneficiary = new Tbl_BeneficiaryDetails();
                    beneficiary.StoryId = viewModel.campaignId;
                    beneficiary.Status = true;
                    beneficiary.BResidence = getLocationfromPolar(viewModel.placeName, viewModel.Latitude, viewModel.longitude);

                    string text = "campaign" + viewModel.campaignId.ToString();
                    if (viewModel.DisplayPicFile != null)
                    {
                        beneficiary.DPPath = AddtoStorage(viewModel.DisplayPicFile, text);
                        beneficiary.DP = new byte[] { };
                        beneficiary.DPName = viewModel.BDisplayPicName != null ? viewModel.BDisplayPicName : "";
                    }
                    beneficiary.CreatedBy = viewModel.UserName;
                    beneficiary.CreatedOn = DateTime.UtcNow;

                    Entity.Tbl_BeneficiaryDetails.Add(beneficiary);
                    await Entity.SaveChangesAsync();
                    var NewBEneficiaryId = beneficiary.Id;
                    return true;
                }
                else
                {
                    var beneficiary = (from S in Entity.Tbl_BeneficiaryDetails where S.StoryId == viewModel.campaignId select S).FirstOrDefault();

                    if (beneficiary != null)
                    {
                        if (viewModel.placeName != null)
                        {
                            beneficiary.BResidence = getLocationfromPolar(viewModel.placeName, viewModel.Latitude, viewModel.longitude);

                        }
                        if (viewModel.DisplayPicFile != null)
                        {

                            string text = "campaign" + viewModel.campaignId.ToString();
                            beneficiary.DPPath = AddtoStorage(viewModel.DisplayPicFile, text);
                            beneficiary.DP = new byte[] { };
                            beneficiary.DPName = viewModel.BDisplayPicName != null ? viewModel.BDisplayPicName : "";
                        }
                        beneficiary.UpdatedBy = viewModel.UserName;
                        beneficiary.UpdatedOn = DateTime.UtcNow;
                        beneficiary.Status = true;
                        await Entity.SaveChangesAsync();
                        return true;
                    }
                    else { return false; }
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        public async Task<bool> CreateCampaignPhase3Desc(CampaignPhase3Desc viewModel)
        {
            try
            {
                var exisitingStorydesc = (from S in Entity.Tbl_CampaignDescription where S.StoryId == viewModel.campaignId select S).FirstOrDefault();
                if (exisitingStorydesc == null)
                {
                    Tbl_CampaignDescription NewStorydesc = new Tbl_CampaignDescription();
                    NewStorydesc.storyDescription = viewModel.StoryDescription;
                    NewStorydesc.StoryId = viewModel.campaignId;
                    NewStorydesc.CreatedBy = viewModel.UserName;
                    NewStorydesc.CreatedOn = DateTime.UtcNow;
                    NewStorydesc.Status = true;
                    Entity.Tbl_CampaignDescription.Add(NewStorydesc);
                    await Entity.SaveChangesAsync();
                    //send mail for the campaign completion notification
                    string url = ConfigurationManager.AppSettings["UIUrl"];
                    url = url + "/fundraiser/" + viewModel.campaignId.ToString();
                    var HtmlBody = PopulateBody(viewModel.UserDisplayName, url, "~/EmailTemplates/CamConfirmation.html");
                    SendMailBase(viewModel.UserName, viewModel.UserDisplayName, "GivingActually- Campaign Creation ",
                      HtmlBody);


                    return true;
                }
                else
                {
                    exisitingStorydesc.storyDescription = viewModel.StoryDescription;
                    exisitingStorydesc.StoryId = viewModel.campaignId;
                    exisitingStorydesc.UpdatedBy = viewModel.UserName;
                    exisitingStorydesc.UpdatedOn = DateTime.UtcNow;
                    await Entity.SaveChangesAsync();
                    return true;

                }

            }
            catch (Exception ex)
            { throw ex; }
        }

        private string PopulateBody(string sendTo, string url,  string Templatepath)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(Templatepath)))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Organizer}", sendTo);
            body = body.Replace("{CampaignUrl}", url);
            return body;
        }

        public async Task<bool> CreateCampaignPhase3Image(CampaignPhase3Image viewModel)
        {
            try
            {
                var NewStoryId = viewModel.campaignId;
                if (viewModel.Attachments.Any())
                {
                    List<Tbl_StoriesAttachment> StoryAttachmentList = new List<Tbl_StoriesAttachment>();
                    foreach (var item in viewModel.Attachments)
                    {
                        Tbl_StoriesAttachment StoryAttachment = new Tbl_StoriesAttachment();
                        StoryAttachment.StoryId = NewStoryId;
                        StoryAttachment.MediaFile = new byte[] { };
                        StoryAttachment.FileName = item.FileName;
                        StoryAttachment.CreatedBy = viewModel.UserName;
                        StoryAttachment.CreatedOn = DateTime.UtcNow;
                        StoryAttachment.ContentType = item.ContentType;
                        string text = "campaign" + viewModel.campaignId.ToString();
                        StoryAttachment.DPPath = AddtoStorage(item.File, text);
                        StoryAttachmentList.Add(StoryAttachment);
                    }
                    Entity.Tbl_StoriesAttachment.AddRange(StoryAttachmentList);
                    await Entity.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            { throw ex; }
        }

        public string AddtoStorage(HttpPostedFile file, string containername)
        {

            try
            {

                if (file != null && file.ContentLength > 0)
                {
                    var filepath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/images/" + containername + "/ "));
                    if (!Directory.Exists(filepath))
                        System.IO.Directory.CreateDirectory(filepath);
                    string imageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                    if (!(file.ContentType == "video/mp4"))
                    {
                        imageName = imageName + ".png";
                    }



                    var TestUrl = System.IO.Path.Combine(filepath, imageName);
                    file.SaveAs(System.IO.Path.Combine(TestUrl));
                    return "/images/" + containername + "/" + imageName;
                }

                else
                { return ""; }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> AddtoAzure(HttpPostedFile file, string containername)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(
        ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);
                var blobStorage = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobStorage.GetContainerReference(containername);
                if (container.CreateIfNotExists())
                {
                    var permissions = container.GetPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    container.SetPermissions(permissions);
                    string imageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                    CloudBlockBlob cblob = container.GetBlockBlobReference(imageName);
                    cblob.UploadFromStream(file.InputStream);
                    var imageFullPath = cblob.Uri.ToString();
                    return imageFullPath;
                }
                else
                {
                    var permissions = container.GetPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    container.SetPermissions(permissions);
                    string imageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                    CloudBlockBlob cblob = container.GetBlockBlobReference(imageName);
                    //cblob.UploadFromStream(file.InputStream);
                    await cblob.UploadFromStreamAsync(file.InputStream);

                    var imageFullPath = cblob.Uri.ToString();
                    return imageFullPath;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        public async Task<int> createwithdrawalRequest(CampaignWithdrawModel viewModel)
        {
            try
            {
                var beneficiaryEntry = (from S in Entity.Tbl_CampaignWithdrawRequest where S.id == viewModel.Id select S).FirstOrDefault();
                var NewBEneficiaryId = 0;
                if (beneficiaryEntry == null)
                {
                    Tbl_CampaignWithdrawRequest withdrawRequest = new Tbl_CampaignWithdrawRequest();
                    withdrawRequest.CampaignId = viewModel.CampaignId;
                    withdrawRequest.createdby = viewModel.CreatedBy;
                    withdrawRequest.CreatedOn = DateTime.UtcNow;
                    withdrawRequest.withdrawalAmount = viewModel.WithdrawalAmount;
                    withdrawRequest.withdrawReason = viewModel.WithDrawalReason;
                    withdrawRequest.withdrawStatus = "Pending for Approval";
                    withdrawRequest.IsApprovedByAdmin = 0;
                    var Bank = (from S in Entity.tbl_CmpnBenBankDetails where S.campaignId == viewModel.CampaignId && S.IsApprovedByAdmin==1 select S.id).FirstOrDefault();
                    withdrawRequest.bankId = Bank;
                    Entity.Tbl_CampaignWithdrawRequest.Add(withdrawRequest);
                    Tbl_WithdrawRequestHistory withdrawRequest1 = new Tbl_WithdrawRequestHistory();
                    withdrawRequest1.CampaignId = viewModel.CampaignId;
                    withdrawRequest1.createdby = viewModel.CreatedBy;
                    withdrawRequest1.CreatedOn = DateTime.UtcNow;
                    withdrawRequest1.withdrawalAmount = viewModel.WithdrawalAmount;
                    withdrawRequest1.withdrawReason = viewModel.WithDrawalReason;
                    withdrawRequest1.IsApprovedByAdmin = 0;
                    withdrawRequest1.withdrawStatus = "Pending for Approval";
                    Entity.Tbl_WithdrawRequestHistory.Add(withdrawRequest1);
                    await Entity.SaveChangesAsync();

                    SendMailBase("givingactuallylive@gmail.com", "The GivingActually", "Withdrwal Request for " + viewModel.CampaignId.ToString(), "Please validate and approve the request of wthdrawal for teh campaign id: " + viewModel.CampaignId.ToString());
                    NewBEneficiaryId = withdrawRequest.id;

                }
                else
                {
                    var withdrawRequestEn = (from S in Entity.Tbl_CampaignWithdrawRequest where S.id == viewModel.Id select S).FirstOrDefault();

                    if (!(withdrawRequestEn == null))
                    {
                        withdrawRequestEn.withdrawalAmount = viewModel.WithdrawalAmount;
                        withdrawRequestEn.withdrawReason = viewModel.WithDrawalReason;
                        withdrawRequestEn.withdrawStatus = viewModel.WithDrawalStatus;

                        Tbl_WithdrawRequestHistory withdrawRequest1 = new Tbl_WithdrawRequestHistory();
                        withdrawRequest1.CampaignId = viewModel.CampaignId;
                        withdrawRequest1.createdby = viewModel.CreatedBy;
                        withdrawRequest1.CreatedOn = DateTime.UtcNow;
                        withdrawRequest1.withdrawalAmount = viewModel.WithdrawalAmount;
                        withdrawRequest1.withdrawReason = viewModel.WithDrawalReason;
                        withdrawRequest1.withdrawStatus = viewModel.WithDrawalStatus;
                        Entity.Tbl_WithdrawRequestHistory.Add(withdrawRequest1);


                        await Entity.SaveChangesAsync();
                        NewBEneficiaryId = withdrawRequestEn.id;

                    }
                }
                return NewBEneficiaryId;
            }
            catch (Exception ex) { throw ex; }
        }
        public async Task<CampaignWithdrawHistoryList> GetwithdrawalRequestHistory(int campaignId)
        {
            try
            {
                var query = from item in Entity.Tbl_WithdrawRequestHistory.AsNoTracking()
                            where item.CampaignId == campaignId
                            select item;
                var WithdrawRequests = await query.ToListAsync();
                CampaignWithdrawHistoryList list = new CampaignWithdrawHistoryList();
                list.WithDrawalList = new List<WithdrawModel>();
                if (WithdrawRequests.Any())
                {
                    foreach (var withdrawRequest in WithdrawRequests)
                    {
                        WithdrawModel withdrawRequestnew = new WithdrawModel();
                        withdrawRequestnew.CampaignId = withdrawRequest.CampaignId.Value;
                        withdrawRequestnew.CreatedBy = withdrawRequest.createdby;
                        withdrawRequestnew.CreatedOn = withdrawRequest.CreatedOn.Value;
                        withdrawRequestnew.WithdrawalAmount = withdrawRequest.withdrawalAmount;
                        withdrawRequestnew.WithDrawalReason = withdrawRequest.withdrawReason;
                        withdrawRequestnew.WithDrawalStatus = withdrawRequest.withdrawStatus;
                        list.WithDrawalList.Add(withdrawRequestnew);

                    }
                    return list;


                }
                return list;
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<List<ApprovalWithdrawModel>> GetwithdrawalRequestForApproval()
        {
            try
            {
                var query = from item in Entity.Tbl_CampaignWithdrawRequest.AsNoTracking()
                            join Cit in Entity.Tbl_Campaign.AsNoTracking() on item.CampaignId equals Cit.Id
                            join Ban in Entity.tbl_CmpnBenBankDetails.AsNoTracking() on item.bankId equals Ban.id
                            where item.IsApprovedByAdmin==0
                            select new { item.id,item.CampaignId, item.createdby, item.CreatedOn,
                                item.withdrawalAmount, item.withdrawReason, item.withdrawStatus, Cit.Title,item.IsApprovedByAdmin,item.IsRejectedByAdmin,item.RejectedReason,
                                item.bankId,IsBankApproved= Ban.IsApprovedByAdmin, IsBankRejected=Ban.IsRejectedByAdmin };
                var WithdrawRequests = await query.ToListAsync();
                List<ApprovalWithdrawModel> list = new List<ApprovalWithdrawModel>();
                
                if (WithdrawRequests.Any())
                {
                    foreach (var withdrawRequest in WithdrawRequests)
                    {
                        ApprovalWithdrawModel withdrawRequestnew = new ApprovalWithdrawModel();
                        withdrawRequestnew.WithdrawalId = withdrawRequest.id;

                        withdrawRequestnew.CampaignId = withdrawRequest.CampaignId.Value;
                        withdrawRequestnew.CreatedBy = withdrawRequest.createdby;
                        withdrawRequestnew.CreatedOn = withdrawRequest.CreatedOn.Value;
                        withdrawRequestnew.WithdrawalAmount = withdrawRequest.withdrawalAmount;
                        withdrawRequestnew.WithDrawalReason = withdrawRequest.withdrawReason;
                        withdrawRequestnew.WithDrawalStatus = withdrawRequest.withdrawStatus;

                        withdrawRequestnew.BankID = withdrawRequest.bankId.Value;
                        withdrawRequestnew.isBankApproved =( withdrawRequest.IsBankApproved==1) ? true:false;
                        withdrawRequestnew.isBankRejected = (withdrawRequest.IsBankRejected == 1) ? true : false;
                        withdrawRequestnew.isApproved = (withdrawRequest.IsApprovedByAdmin==1) ? true : false;
                        withdrawRequestnew.isRejected = (withdrawRequest.IsRejectedByAdmin == 1) ? true : false;
                        withdrawRequestnew.RejectedReason = withdrawRequest.RejectedReason;
                        list.Add(withdrawRequestnew);

                    }
                    return list;


                }
                return list;
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<ApprovalWithdrawModel> GetwithdrawalRequestById(int WithdrawalId)
        {
            try
            {
                var query = from item in Entity.Tbl_CampaignWithdrawRequest.AsNoTracking()
                            join Cit in Entity.Tbl_Campaign.AsNoTracking() on item.CampaignId equals Cit.Id
                            join Ban in Entity.tbl_CmpnBenBankDetails.AsNoTracking() on item.bankId equals Ban.id
                            where  item.id==  WithdrawalId
                            select new
                            {
                                item.id,
                                item.CampaignId,
                                item.createdby,
                                item.CreatedOn,
                                item.withdrawalAmount,
                                item.withdrawReason,
                                item.withdrawStatus,
                                Cit.Title,
                                item.IsApprovedByAdmin,
                                item.IsRejectedByAdmin,
                                item.RejectedReason,
                                item.bankId,
                                IsBankApproved = Ban.IsApprovedByAdmin,
                                IsBankRejected = Ban.IsRejectedByAdmin
                            };
                var withdrawRequest = await query.FirstOrDefaultAsync();
                ApprovalWithdrawModel withdrawRequestnew = new ApprovalWithdrawModel();
                if (withdrawRequest != null)
                {     
                        withdrawRequestnew.WithdrawalId = withdrawRequest.id;
                        withdrawRequestnew.CampaignId = withdrawRequest.CampaignId.Value;
                        withdrawRequestnew.CreatedBy = withdrawRequest.createdby;
                        withdrawRequestnew.CreatedOn = withdrawRequest.CreatedOn.Value;
                        withdrawRequestnew.WithdrawalAmount = withdrawRequest.withdrawalAmount;
                        withdrawRequestnew.WithDrawalReason = withdrawRequest.withdrawReason;
                        withdrawRequestnew.WithDrawalStatus = withdrawRequest.withdrawStatus;

                        withdrawRequestnew.BankID = withdrawRequest.bankId.Value;
                        withdrawRequestnew.isBankApproved = (withdrawRequest.IsBankApproved == 1) ? true : false;
                        withdrawRequestnew.isBankRejected = (withdrawRequest.IsBankRejected == 1) ? true : false;
                        withdrawRequestnew.isApproved = (withdrawRequest.IsApprovedByAdmin == 1) ? true : false;
                        withdrawRequestnew.isRejected = (withdrawRequest.IsRejectedByAdmin == 1) ? true : false;
                        withdrawRequestnew.RejectedReason = withdrawRequest.RejectedReason;
                       

                    }
                    return withdrawRequestnew;


                
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<List<ApprovalWithdrawModel>> GetCampaignwithdrawalRequest(int CampaignId)
        {
            try
            {
                var query = from item in Entity.Tbl_CampaignWithdrawRequest.AsNoTracking()
                            join Cit in Entity.Tbl_Campaign.AsNoTracking() on item.CampaignId equals Cit.Id
                            join Ban in Entity.tbl_CmpnBenBankDetails.AsNoTracking() on item.bankId equals Ban.id
                            where  item.CampaignId== CampaignId
                            select new
                            {
                                item.id,
                                item.CampaignId,
                                item.createdby,
                                item.CreatedOn,
                                item.withdrawalAmount,
                                item.withdrawReason,
                                item.withdrawStatus,
                                Cit.Title,
                                item.IsApprovedByAdmin,
                                item.IsRejectedByAdmin,
                                item.RejectedReason,
                                item.bankId,
                                IsBankApproved = Ban.IsApprovedByAdmin,
                                IsBankRejected = Ban.IsRejectedByAdmin
                            };
                var WithdrawRequests = await query.ToListAsync();
                List<ApprovalWithdrawModel> list = new List<ApprovalWithdrawModel>();

                if (WithdrawRequests.Any())
                {
                    foreach (var withdrawRequest in WithdrawRequests)
                    {
                        ApprovalWithdrawModel withdrawRequestnew = new ApprovalWithdrawModel();
                        withdrawRequestnew.WithdrawalId = withdrawRequest.id;

                        withdrawRequestnew.CampaignId = withdrawRequest.CampaignId.Value;
                        withdrawRequestnew.CreatedBy = withdrawRequest.createdby;
                        withdrawRequestnew.CreatedOn = withdrawRequest.CreatedOn.Value;
                        withdrawRequestnew.WithdrawalAmount = withdrawRequest.withdrawalAmount;
                        withdrawRequestnew.WithDrawalReason = withdrawRequest.withdrawReason;
                        withdrawRequestnew.WithDrawalStatus = withdrawRequest.withdrawStatus;

                        withdrawRequestnew.BankID = withdrawRequest.bankId.Value;
                        withdrawRequestnew.isBankApproved = (withdrawRequest.IsBankApproved == 1) ? true : false;
                        withdrawRequestnew.isBankRejected = (withdrawRequest.IsBankRejected == 1) ? true : false;
                        withdrawRequestnew.isApproved = (withdrawRequest.IsApprovedByAdmin == 1) ? true : false;
                        withdrawRequestnew.isRejected = (withdrawRequest.IsRejectedByAdmin == 1) ? true : false;
                        withdrawRequestnew.RejectedReason = withdrawRequest.RejectedReason;
                        list.Add(withdrawRequestnew);

                    }
                    return list;


                }
                return list;
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<List<ApprovalWithdrawModel>> GetCampaignApprovedwithdrawal(int CampaignId)
        {
            try
            {
                var query = from item in Entity.Tbl_CampaignWithdrawRequest.AsNoTracking()
                            join Cit in Entity.Tbl_Campaign.AsNoTracking() on item.CampaignId equals Cit.Id
                            join Ban in Entity.tbl_CmpnBenBankDetails.AsNoTracking() on item.bankId equals Ban.id
                            where item.CampaignId == CampaignId && item.IsApprovedByAdmin==1
                            select new
                            {
                                item.id,
                                item.CampaignId,
                                item.createdby,
                                item.CreatedOn,
                                item.withdrawalAmount,
                                item.withdrawReason,
                                item.withdrawStatus,
                                Cit.Title,
                                item.IsApprovedByAdmin,
                                item.IsRejectedByAdmin,
                                item.RejectedReason,
                                item.bankId,
                                IsBankApproved = Ban.IsApprovedByAdmin,
                                IsBankRejected = Ban.IsRejectedByAdmin
                            };
                var WithdrawRequests = await query.ToListAsync();
                List<ApprovalWithdrawModel> list = new List<ApprovalWithdrawModel>();

                if (WithdrawRequests.Any())
                {
                    foreach (var withdrawRequest in WithdrawRequests)
                    {
                        ApprovalWithdrawModel withdrawRequestnew = new ApprovalWithdrawModel();
                        withdrawRequestnew.WithdrawalId = withdrawRequest.id;

                        withdrawRequestnew.CampaignId = withdrawRequest.CampaignId.Value;
                        withdrawRequestnew.CreatedBy = withdrawRequest.createdby;
                        withdrawRequestnew.CreatedOn = withdrawRequest.CreatedOn.Value;
                        withdrawRequestnew.WithdrawalAmount = withdrawRequest.withdrawalAmount;
                        withdrawRequestnew.WithDrawalReason = withdrawRequest.withdrawReason;
                        withdrawRequestnew.WithDrawalStatus = withdrawRequest.withdrawStatus;

                        withdrawRequestnew.BankID = withdrawRequest.bankId.Value;
                        withdrawRequestnew.isBankApproved = (withdrawRequest.IsBankApproved == 1) ? true : false;
                        withdrawRequestnew.isBankRejected = (withdrawRequest.IsBankRejected == 1) ? true : false;
                        withdrawRequestnew.isApproved = (withdrawRequest.IsApprovedByAdmin == 1) ? true : false;
                        withdrawRequestnew.isRejected = (withdrawRequest.IsRejectedByAdmin == 1) ? true : false;
                        withdrawRequestnew.RejectedReason = withdrawRequest.RejectedReason;
                        list.Add(withdrawRequestnew);

                    }
                    return list;


                }
                return list;
            }
            catch (Exception ex) { throw ex; }
        }


        public async Task<CampaignBankViewModel> GetCampaignBankDetail(int campaignId)
        {
            try
            {
                var query = from item in Entity.tbl_CmpnBenBankDetails.AsNoTracking()
                            where item.campaignId == campaignId
                            orderby item.CreatedOn descending
                            select item;
                var viewModel = await query.FirstOrDefaultAsync();
                CampaignBankViewModel BanksDetail = new CampaignBankViewModel();
                if (viewModel != null)
                {
                    BanksDetail.CampaignId = viewModel.campaignId.Value;
                    BanksDetail.BankName = viewModel.BankName;
                    BanksDetail.AccountNumber = viewModel.AccountNumber;
                    BanksDetail.BankBranch = viewModel.BankBranch;
                    BanksDetail.BenName = viewModel.BenName;
                    BanksDetail.IFSC = viewModel.IFSC;
                    BanksDetail.BankId = viewModel.id;
                    BanksDetail.isApproved = (viewModel.IsApprovedByAdmin == 1 ? true : false);
                    BanksDetail.isRejected = (viewModel.IsRejectedByAdmin == 1 ? true : false);
                    BanksDetail.RejectedReason = viewModel.RejectedReason;
                    BanksDetail.ApprovedOn = viewModel.ApprovedOn != null ? viewModel.ApprovedOn.Value : DateTime.MinValue;
                }
                return BanksDetail;
            }
            catch (Exception ex) { throw ex; }
        }


        public async Task<CampaignBankViewModel> GetCampaignApprovedBankDetail(int campaignId)
        {
            try
            {
                var query = from item in Entity.tbl_CmpnBenBankDetails.AsNoTracking()
                            where item.campaignId == campaignId && item.IsApprovedByAdmin==1
                            orderby item.CreatedOn descending
                            select item;
                var viewModel = await query.FirstOrDefaultAsync();
                CampaignBankViewModel BanksDetail = new CampaignBankViewModel();
                if (viewModel != null)
                {
                    BanksDetail.CampaignId = viewModel.campaignId.Value;
                    BanksDetail.BankName = viewModel.BankName;
                    BanksDetail.AccountNumber = viewModel.AccountNumber;
                    BanksDetail.BankBranch = viewModel.BankBranch;
                    BanksDetail.BenName = viewModel.BenName;
                    BanksDetail.IFSC = viewModel.IFSC;
                    BanksDetail.BankId = viewModel.id;
                    BanksDetail.isApproved = (viewModel.IsApprovedByAdmin == 1 ? true : false);
                    BanksDetail.isRejected = (viewModel.IsRejectedByAdmin == 1 ? true : false);
                    BanksDetail.RejectedReason = viewModel.RejectedReason;
                    BanksDetail.ApprovedOn = viewModel.ApprovedOn != null ? viewModel.ApprovedOn.Value : DateTime.MinValue;
                }
                return BanksDetail;
            }
            catch (Exception ex) { throw ex; }
        }
        public async Task<CampaignBankViewModel> GetCampaignBankDetailById(int BankId)
        {
            try
            {
                var query = from item in Entity.tbl_CmpnBenBankDetails.AsNoTracking()
                            where item.id == BankId
                            orderby item.CreatedOn descending
                            select item;
                var viewModel = await query.FirstOrDefaultAsync();
                CampaignBankViewModel BanksDetail = new CampaignBankViewModel();
                if (viewModel != null)
                {
                    BanksDetail.CampaignId = viewModel.campaignId.Value;
                    BanksDetail.BankName = viewModel.BankName;
                    BanksDetail.AccountNumber = viewModel.AccountNumber;
                    BanksDetail.BankBranch = viewModel.BankBranch;
                    BanksDetail.BenName = viewModel.BenName;
                    BanksDetail.IFSC = viewModel.IFSC;
                    BanksDetail.BankId = viewModel.id;
                    BanksDetail.isApproved = (viewModel.IsApprovedByAdmin == 1 ? true : false);
                    BanksDetail.isRejected = (viewModel.IsRejectedByAdmin == 1 ? true : false);
                    BanksDetail.RejectedReason = viewModel.RejectedReason;
                    BanksDetail.ApprovedOn = viewModel.ApprovedOn != null ? viewModel.ApprovedOn.Value : DateTime.MinValue;
                }
                return BanksDetail;
            }
            catch (Exception ex) { throw ex; }
        }
        public async Task<List<CampaignBankModel>> GetCampaignBankforApproval()
        {
            try
            {
                var query = from item in Entity.tbl_CmpnBenBankDetails.AsNoTracking()
                            join Cit in Entity.Tbl_Campaign.AsNoTracking() on item.campaignId equals Cit.Id
                            where item.IsApprovedByAdmin == 0
                            orderby item.CreatedOn descending
                            select new
                            {
                                item.campaignId,
                                item.BankName,
                                item.AccountNumber,
                                item.BankBranch,
                                item.BenName,
                                item.IFSC,
                                item.id,
                                               item.IsApprovedByAdmin,
                                        item.IsRejectedByAdmin,item.RejectedReason,item.ApprovedOn, Cit.Title};
                                var viewModelres = await query.ToListAsync();
                List<CampaignBankModel> bankList = new List<CampaignBankModel>();
                foreach (var viewModel in viewModelres)
                {
                    CampaignBankModel BanksDetail = new CampaignBankModel();
                    if (viewModel != null)
                    {
                        BanksDetail.CampaignId = viewModel.campaignId.Value;
                        BanksDetail.BankName = viewModel.BankName;
                        BanksDetail.AccountNumber = viewModel.AccountNumber;
                        BanksDetail.BankBranch = viewModel.BankBranch;
                        BanksDetail.BenName = viewModel.BenName;
                        BanksDetail.IFSC = viewModel.IFSC;
                        BanksDetail.isApproved = (viewModel.IsApprovedByAdmin == 1 ? true : false);
                        BanksDetail.isRejected = (viewModel.IsRejectedByAdmin == 1 ? true : false);
                        BanksDetail.RejectedReason = viewModel.RejectedReason;
                        BanksDetail.CampaignTitle = viewModel.Title;
                        BanksDetail.BankId = viewModel.id;
                    }
                    bankList.Add(BanksDetail);
                }
                return bankList;
            }
            catch (Exception ex) { throw ex; }
        }
        public async Task<int> createCampaignBank(CampaignBankVwModel viewModel)
        {
            try
            {
                var beneficiaryEntry = (from S in Entity.tbl_CmpnBenBankDetails where S.campaignId == viewModel.CampaignId select S).FirstOrDefault();
                var NewBEneficiaryId = 0;
                if (beneficiaryEntry == null)
                {
                    tbl_CmpnBenBankDetails beneficiary = new tbl_CmpnBenBankDetails();
                    beneficiary.campaignId = viewModel.CampaignId;
                    beneficiary.CreatedBy = viewModel.CreatedBy;
                    beneficiary.CreatedOn = DateTime.UtcNow;
                    beneficiary.BankName = viewModel.BankName;
                    beneficiary.AccountNumber = viewModel.AccountNumber;
                    beneficiary.BankBranch = viewModel.BankBranch;
                    beneficiary.BenName = viewModel.BenName;
                    beneficiary.IFSC = viewModel.IFSC;
                    beneficiary.IsApprovedByAdmin = 0;
                    beneficiary.IsRejectedByAdmin = 0;
                    Entity.tbl_CmpnBenBankDetails.Add(beneficiary);
                    await Entity.SaveChangesAsync();
                    NewBEneficiaryId = beneficiary.id;

                }
                else
                {
                    var beneficiaryEn = (from S in Entity.tbl_CmpnBenBankDetails where S.campaignId == viewModel.CampaignId select S).FirstOrDefault();

                    if (!(beneficiaryEn == null))
                    {
                        beneficiaryEn.CreatedBy = viewModel.CreatedBy;
                        beneficiaryEn.CreatedOn = DateTime.UtcNow;
                        beneficiaryEn.BankName = viewModel.BankName;
                        beneficiaryEn.AccountNumber = viewModel.AccountNumber;
                        beneficiaryEn.BankBranch = viewModel.BankBranch;
                        beneficiaryEn.BenName = viewModel.BenName;
                        beneficiaryEn.IFSC = viewModel.IFSC;
                        await Entity.SaveChangesAsync();
                        NewBEneficiaryId = beneficiaryEn.id;

                    }
                }
                return NewBEneficiaryId;
            }
            catch (Exception ex) { throw ex; }
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
                    Organizer.FullplaceName = res.CityName;
                    String str = res.CityName != null ? res.CityName : "";
                    String[] spearator = { "," };
                    String[] strlist = str.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    var result = strlist.Reverse().Skip(2).Take(1).Reverse().ToArray();
                    var val = string.Join(", ", result);
                    Organizer.placeNmae = string.IsNullOrWhiteSpace(val) ? str : val;
                }
                // Organizer.BPinCode = beneficiary.BPincode;//need to change to exact pincode of table
                //  Organizer.BDisplayPic = beneficiary.DP;
                Organizer.BDisplayPicName = beneficiary.DPName != null ? beneficiary.DPName : "";


                Organizer.BDisplayPicPath = (beneficiary.DPPath);
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
            Result = Result.Replace("&#160", " ");
            //&#160
            return Result;
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
                    ModelList.FirstName = !(string.IsNullOrEmpty(res.FirstName)) ? res.FirstName : "";
                    ModelList.LastName = !(string.IsNullOrEmpty(res.LastName)) ? res.LastName : "";
                    ModelList.DPPAth = !(string.IsNullOrEmpty(res.DPPath)) ? res.DPPath : "";
                    ModelList.UserDPImage = !(string.IsNullOrEmpty(res.DPPath)) ? res.DPPath : "";
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


        public bool ISUserNGO(int id)
        {
            try
            {
                int UserId = id;
                UserModel ModelList = new UserModel();
                var res = (from S in Entity.Tbl_User where S.Id == UserId select S).FirstOrDefault();
                if (res != null)
                {
                    if (res.IsNGO != null && res.CanEndorse != null)
                    {
                        if (res.IsNGO.Value && res.CanEndorse.Value)
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public RegionInfo GetUserCountryByIp()
        {
            RegionInfo region = new RegionInfo("IN");

            //try
            //{

            //    IpInfo ipInfo = new IpInfo();

            //    string info = new WebClient().DownloadString("http://ipinfo.io");

            //    //JavaScriptSerializer jsonObject = new JavaScriptSerializer();
            //    //ipInfo = jsonObject.Deserialize<IpInfo>(info);
            //    //if (ipInfo.Country == "NL")
            //    //{
            //    //    region = new RegionInfo(ipInfo.Country != null ? ipInfo.Country : "IN");
            //    //}



            //    //using (var webClient = new System.Net.WebClient())
            //    //{

            //    //    var data = webClient.DownloadString("https://geolocation-db.com/json");
            //    //    JavaScriptSerializer jss = new JavaScriptSerializer();
            //    //    var d = jss.Deserialize<dynamic>(data);

            //    //    //string country_code = d["country_code"];
            //    //    //string country_name = d["country_name"];
            //    //    //string city = d["city"];
            //    //    //string postal = d["postal"];
            //    //    //string state = d["state"];
            //    //    //string ipv4 = d["IPv4"];
            //    //    decimal latitude = d["latitude"];
            //    //    decimal longitude = d["longitude"];
            //    //    string cntry= d["country_code"];
            //    //    region = new RegionInfo(d["country_code"] != null ? d["country_code"] : "IN");

            //    //    if (cntry == "IN" && cntry == "NL")
            //    //    {
            //    //        region = new RegionInfo(d["country_code"] != null ? d["country_code"] : "IN");
            //    //    }
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            return region;
        }



        #region Comments
        public commentsModel GetComments(int Id)
        {
            commentsModel comments = new commentsModel();
            var Cmnts = (from S in Entity.Tbl_ParentComment where S.StoryId == Id select S).ToList().OrderByDescending(a => a.CommentDate).ToList();
            if (Cmnts.Any())
            {
                List<CommentsVM> cmntModel = new List<CommentsVM>();
                foreach (var Cmnt in Cmnts)
                {
                    CommentsVM cmt = new CommentsVM();
                    cmt.CommentMsg = Cmnt.CommentMessage;
                    cmt.CommentedDate = Cmnt.CommentDate.Value;
                    cmt.campaignId = Cmnt.StoryId;
                    var user = GetUserDetailbyId(Cmnt.UserId);
                    cmt.SubComments = new List<SubCommentsVM>();
                    cmt.Users = user;
                    cmntModel.Add(cmt);
                }
                comments.AllComments = new List<CommentsVM>();
                comments.AllComments.AddRange(cmntModel);
                comments.CommentsCount = Cmnts.Count();
            }
            return comments;
        }

        public commentsModel GetMiniComments(int Id)
        {
            commentsModel comments = new commentsModel();
            var Cmnts = (from S in Entity.Tbl_ParentComment where S.StoryId == Id select S).ToList().OrderByDescending(a => a.CommentDate).ToList();
            if (Cmnts.Any())
            {
                int i = 1;
                List<CommentsVM> cmntModel = new List<CommentsVM>();
                comments.AllComments = new List<CommentsVM>();
                foreach (var Cmnt in Cmnts)
                {
                    CommentsVM cmt = new CommentsVM();
                    cmt.CommentMsg = Cmnt.CommentMessage;
                    cmt.CommentedDate = Cmnt.CommentDate.Value;
                    cmt.campaignId = Cmnt.StoryId;
                    var user = GetUserDetailbyId(Cmnt.UserId);
                    cmt.SubComments = new List<SubCommentsVM>();
                    cmt.Users = user;
                    cmntModel.Add(cmt);
                    i++;
                    if (i == 4)
                    { break; }
                }
                comments.AllComments.AddRange(cmntModel);
                comments.CommentsCount = Cmnts.Count();
            }
            return comments;
        }

        public CommentsVM GetMiniComment(int Id, int campaignId)
        {
            //commentsModel comments = new commentsModel();
            var Cmnt = (from S in Entity.Tbl_ParentComment where S.StoryId == campaignId && S.CommentID == Id select S).FirstOrDefault();
            CommentsVM cmt = new CommentsVM();
            if (Cmnt != null)
            {
                cmt.CommentMsg = Cmnt.CommentMessage;
                cmt.CommentedDate = Cmnt.CommentDate.Value;
                cmt.campaignId = Cmnt.StoryId;
                var user = GetUserDetailbyId(Cmnt.UserId);
                cmt.Users = user;
            }
            var CmntCount = (from S in Entity.Tbl_ParentComment.AsNoTracking() where S.StoryId == campaignId select S.CommentID).Count();
            cmt.Totalcomments = CmntCount;
            return cmt;
        }
        public CommentsVM PostComments(commentModel model)
        {
            CommentsVM comments = new CommentsVM();

            Tbl_ParentComment cmnt = new Tbl_ParentComment();
            cmnt.UserId = model.userId;
            cmnt.StoryId = model.campaignId;
            cmnt.CommentDate = DateTime.Now;
            cmnt.CommentMessage = model.CommentText;
            Entity.Tbl_ParentComment.Add(cmnt);
            Entity.SaveChanges();
            comments = GetMiniComment(cmnt.CommentID, model.campaignId);
            return comments;
        }
        #endregion

        #region Shares
        public SharesModel GetSharesdetails(int Id)
        {
            SharesModel shares = new SharesModel();
            var srs = (from S in Entity.Tbl_Shares where S.StoryId == Id select S).ToList();
            if (srs.Any())
            {
                List<share> sharesModel = new List<share>();
                foreach (var Shar in srs)
                {
                    share share = new share();

                    share.campaignId = Shar.StoryId;
                    var user = GetUserDetailbyId(Shar.UserId);
                    share.SharedbyUserId = user != null ? user.Id : 0;
                    share.SharedbyUserName = user != null ? user.UserName : "";
                    share.SharedbyDpName = user != null ? user.DisplayName : "";
                    share.Media = Shar.Media;
                    sharesModel.Add(share);
                }
                shares.AllShares = new List<share>();
                shares.AllShares.AddRange(sharesModel);
                shares.SharesCount = sharesModel.Count();
            }
            return shares;
        }

        public SharesModel GetSharecount(int Id)
        {
            SharesModel shares = new SharesModel();
            var srs = (from S in Entity.Tbl_Shares where S.StoryId == Id select S).Count();
            shares.AllShares = new List<share>();
            shares.SharesCount = srs;
            return shares;
        }

        public SharesModel PostShares(shareModel model)
        {
            SharesModel shares = new SharesModel();
            try
            {
                Tbl_Shares shr = new Tbl_Shares();
                shr.UserId = model.UserId;
                shr.StoryId = model.campaignId;
                shr.updatedDate = DateTime.Now;
                shr.Media = model.Media;
                Entity.Tbl_Shares.Add(shr);
                Entity.SaveChanges();
                shares = GetSharecount(model.campaignId);
                return shares;
            }
            catch (Exception ex)
            { throw ex; }
        }
        #endregion


        #region Likes
        public LikesModel GetLikesdetails(int Id)
        {
            LikesModel likes = new LikesModel();
            var Lks = (from S in Entity.Tbl_Like where S.StoryId == Id select S).ToList();
            if (Lks.Any())
            {
                List<Like> LikesModel = new List<Like>();
                foreach (var Lk in Lks)
                {
                    Like Like = new Like();

                    Like.campaignId = Lk.StoryId;
                    var user = GetUserDetailbyId(Lk.UserId);
                    Like.LikebyUserId = user != null ? user.Id : 0;
                    Like.LikebyUserName = user != null ? user.UserName : "";
                    Like.LikebyDpName = user != null ? user.DisplayName : "";

                    LikesModel.Add(Like);
                }
                likes.AllLikes = new List<Like>();
                likes.AllLikes.AddRange(LikesModel);
                likes.LikesCount = LikesModel.Count();
            }
            return likes;
        }

        public LikesModel GetLikesCount(int Id)
        {
            LikesModel Likes = new LikesModel();
            var srs = (from S in Entity.Tbl_Like where S.StoryId == Id select S).Count();
            Likes.AllLikes = new List<Like>();
            Likes.LikesCount = srs;
            return Likes;
        }

        public LikesModel PostLike(Like model)
        {
            LikesModel Likes = new LikesModel();

            Tbl_Like lke = new Tbl_Like();

            lke.UserId = model.LikebyUserId;
            lke.StoryId = model.campaignId;
            lke.updatedDate = DateTime.Now;
            Entity.Tbl_Like.Add(lke);
            Entity.SaveChanges();
            Likes = GetLikesCount(model.campaignId);
            return Likes;
        }
        #endregion

        #region Donors
        public DonationsModel GetDonorsList(int Id)
        {
            try
            {
                DonationsModel DonorsList = new DonationsModel();
                var donations = (from S in Entity.Tbl_CampaignDonation where S.StoryId == Id && S.isPaid == true select S).ToList().OrderByDescending(a => a.DonatedOn);
                if (donations.Any())
                {
                    List<Donation> donationList = new List<Donation>();
                    decimal RaisedAmt = 0;
                    long RaisedBy = 0;
                    decimal RaisedPerc = 0;
                    List<string> donrList = new List<string>();
                    foreach (var dntion in donations)
                    {
                        RaisedAmt = RaisedAmt + dntion.DonationAmnt;
                        RaisedBy++;
                        donrList.Add(dntion.Email);
                        Donation donation = new Donation();
                        donation.CampaignId = dntion.StoryId;
                        donation.DonorsEMail = dntion.Email;
                        donation.DonationMoneyType = "INR";
                        donation.PlaceName = dntion.PlaceName;
                        donation.DonorsName = (dntion.isAnanymous != null ? dntion.isAnanymous.Value : false) ? "Anonymous" : dntion.DonatedBy;
                        donation.DonatedOn = dntion.DonatedOn != null ? dntion.DonatedOn.Value : DateTime.Now;
                        donation.DonorsPhNo = "";
                        donation.DonationAmt = dntion.DonationAmnt;
                        donation.PaymentGSTFee = dntion.PayGSTAmnt.Value;
                        donation.PaymentProcFee = dntion.PayProcessingFeeAmnt.Value;
                        donation.ActualDonationAmnt = dntion.PaidDOnationAmt.Value;

                        donationList.Add(donation);
                    }
                    var campagin = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                    if (campagin != null)
                    {
                        decimal difference = campagin.TargetAmount.Value - RaisedAmt;
                        RaisedPerc = campagin.TargetAmount != null ?
                            (campagin.TargetAmount.Value != 0 ? ((RaisedAmt / campagin.TargetAmount.Value) * 100) : 0) : 0;
                    }
                    DonorsList.AllDonors = new List<Donation>();
                    DonorsList.AllDonors.AddRange(donationList);
                    DonorsList.TotolRaisedAmnt = RaisedAmt;
                    RaisedBy = donrList != null ? donrList.Distinct().ToList().Count() : 0;
                    DonorsList.DonorsCount = Convert.ToInt32(RaisedBy);
                    DonorsList.RaisedPercentage = Convert.ToInt32(RaisedPerc);

                }
                return DonorsList;
            }
            catch (Exception ex)
            { throw ex; }
        }

        public int PostDonation(CampaignDonation viewModel)
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

                    string RazorPayProcessingFeePerc = ConfigurationManager.AppSettings["RazorPayProcessingFeePerc"];

                    string RazorPayGSTPerc = ConfigurationManager.AppSettings["RazorPayGSTPerc"];
                    donation.PayGSTPerc = Convert.ToInt32(Convert.ToDecimal(RazorPayGSTPerc) * 100);
                    donation.PayProcessingFeePerc = Convert.ToInt32(Convert.ToDecimal(RazorPayProcessingFeePerc) * 100);
                    donation.PayProcessingFeeAmnt = viewModel.DonationAmnt * Convert.ToDecimal(RazorPayProcessingFeePerc);

                    donation.PayGSTAmnt = donation.PayProcessingFeeAmnt * Convert.ToDecimal(RazorPayGSTPerc);
                    donation.PaidDOnationAmt = viewModel.DonationAmnt - (donation.PayProcessingFeeAmnt + donation.PayGSTAmnt);
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
                        string RazorPayProcessingFeePerc = ConfigurationManager.AppSettings["RazorPayProcessingFeePerc"];

                        string RazorPayGSTPerc = ConfigurationManager.AppSettings["RazorPayGSTPerc"];
                        donation.PayGSTPerc = Convert.ToInt32(Convert.ToDecimal(RazorPayGSTPerc) * 100);
                        donation.PayProcessingFeePerc = Convert.ToInt32(Convert.ToDecimal(RazorPayProcessingFeePerc) * 100);
                        donation.PayProcessingFeeAmnt = viewModel.DonationAmnt * Convert.ToDecimal(RazorPayProcessingFeePerc);

                        donation.PayGSTAmnt = donation.PayProcessingFeeAmnt * Convert.ToDecimal(RazorPayGSTPerc);
                        donation.PaidDOnationAmt = viewModel.DonationAmnt - (donation.PayProcessingFeeAmnt + donation.PayGSTAmnt);
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
        #endregion
        #region Endorsements
        public ENdorsementList GetEndorsements(int Id)
        {
            try
            {
                var endorseresult = (from S in Entity.Tbl_Endorse
                                     where S.CampaignId == Id
                                     select S).ToList();
                ENdorsementList list = new ENdorsementList();
                List<Endorsement> individuallist = new List<Endorsement>();
                foreach (var val in endorseresult)
                {

                    var user = GetUserDetailbyId(val.NGOId);
                    Endorsement newval = new Endorsement();
                    newval.NGOId = val.NGOId;
                    newval.NGOName = user.DisplayName;
                    newval.endorsementId = val.EndorseID;
                    newval.NGOAddress = user.NGOAddress;

                    var NGOSector = (StoryCategory)(user.NGOSector);
                    newval.NGOSector = NGOSector.DisplayName();
                    var NGOType = (NGOType)(user.NGOType);
                    newval.NGOSector = NGOType.DisplayName();

                    //newval.NGOSector = user.NGOSector.ToString();
                    //newval.NGOType = user.NGOType.ToString();
                    individuallist.Add(newval);
                }
                list.CampaignId = Id;
                list.TotalCount = endorseresult.Count();
                list.EndorseList = individuallist;
                return list;
            }
            catch (Exception ex)
            { throw ex; }
        }
        public ENdorsementList GetEndorsementsCount(int Id)
        {
            try
            {
                var endorseresult = (from S in Entity.Tbl_Endorse
                                     where S.CampaignId == Id
                                     select S).ToList();
                ENdorsementList list = new ENdorsementList();
                List<Endorsement> individuallist = new List<Endorsement>();

                list.CampaignId = Id;
                list.TotalCount = endorseresult.Count();
                list.EndorseList = individuallist;
                return list;
            }
            catch (Exception ex)
            { throw ex; }
        }

        public Endorsement GetEndorsement(int Id, int campaignId)
        {
            try
            {
                var endorseresult = (from S in Entity.Tbl_Endorse
                                     where S.CampaignId == campaignId && S.EndorseID == Id
                                     select S).FirstOrDefault();
                Endorsement newval = new Endorsement();
                if (endorseresult != null)
                {
                    var user = GetUserDetailbyId(endorseresult.NGOId);

                    newval.NGOId = endorseresult.NGOId;
                    newval.NGOName = user.DisplayName;
                    newval.endorsementId = endorseresult.EndorseID;
                    newval.NGOAddress = user.NGOAddress;

                    if (user.NGOSector != 0)
                    {
                        var NGOSector = (StoryCategory)(user.NGOSector);
                        newval.NGOSector = NGOSector.DisplayName();
                    }
                    if (user.NGOType != 0)
                    {
                        var NGOType = (NGOType)(user.NGOType);
                        newval.NGOSector = NGOType.DisplayName();
                    }
                }
                return newval;
            }
            catch (Exception ex)
            { throw ex; }
        }
        public Endorsement PostEndorsements(EndorseModel model)
        {
            try
            {

                Tbl_Endorse end = new Tbl_Endorse();

                end.NGOId = model.NGOId;
                end.CampaignId = model.CampaignId;
                end.updatedDate = DateTime.Now;
                Entity.Tbl_Endorse.Add(end);
                Entity.SaveChanges();
                var list = GetEndorsement(end.EndorseID, model.CampaignId);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region post updates 

        public async Task<int> CreateUpdateDesc(CampaignPhase3Desc viewModel)
        {
            try
            {
                Tbl_CampaignDescriptionUpdates NewStorydesc = new Tbl_CampaignDescriptionUpdates();
                NewStorydesc.storyDescription = viewModel.StoryDescription;
                NewStorydesc.StoryId = viewModel.campaignId;
                NewStorydesc.CreatedBy = viewModel.UserName;
                NewStorydesc.CreatedOn = DateTime.UtcNow;
                NewStorydesc.Status = true;

                Entity.Tbl_CampaignDescriptionUpdates.Add(NewStorydesc);
                await Entity.SaveChangesAsync();
                var updateId = NewStorydesc.Id;
                return updateId;

            }
            catch (Exception ex)
            { throw ex; }
        }

        public async Task<bool> CreateUpdatesImage(CampaignPhase3Image viewModel)
        {
            try
            {
                var NewStoryId = viewModel.campaignId;
                if (viewModel.UpdateId > 0)
                {
                    if (viewModel.Attachments.Any())
                    {
                        List<Tbl_UpdatesAttachment> StoryAttachmentList = new List<Tbl_UpdatesAttachment>();
                        foreach (var item in viewModel.Attachments)
                        {
                            Tbl_UpdatesAttachment StoryAttachment = new Tbl_UpdatesAttachment();
                            StoryAttachment.StoryId = NewStoryId;
                            StoryAttachment.MediaFile = new byte[] { };
                            StoryAttachment.FileName = item.FileName;
                            StoryAttachment.CreatedBy = viewModel.UserName;
                            StoryAttachment.CreatedOn = DateTime.UtcNow;
                            StoryAttachment.UpdateId = viewModel.UpdateId;
                            StoryAttachment.ContentType = item.ContentType;
                            string text = "campaign" + viewModel.campaignId.ToString();
                            StoryAttachment.DPPath = AddtoStorage(item.File, text);
                            StoryAttachmentList.Add(StoryAttachment);
                        }
                        Entity.Tbl_UpdatesAttachment.AddRange(StoryAttachmentList);
                        await Entity.SaveChangesAsync();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            { throw ex; }
        }


        #endregion


        #region Admin Approval 

        public async Task<bool> ApproveWithdrawRequest(ApproveWithdrawModel viewModel)
        {
            try
            {
                var withdrawRequest = (from S in Entity.Tbl_CampaignWithdrawRequest where S.id == viewModel.WithdrawalId select S).FirstOrDefault();

                if (withdrawRequest != null)
                {
                    withdrawRequest.Approvedon = DateTime.UtcNow;
                    withdrawRequest.IsApprovedByAdmin = 1;
                    withdrawRequest.withdrawStatus = "Approved By Admin";

                    Tbl_WithdrawRequestHistory withdrawRequest1 = new Tbl_WithdrawRequestHistory();
                    withdrawRequest1.withdrawalAmount = withdrawRequest.withdrawalAmount;
                    withdrawRequest1.withdrawReason = withdrawRequest.withdrawReason;
                    withdrawRequest1.withdrawStatus = withdrawRequest.withdrawStatus;
                    withdrawRequest1.CampaignId = withdrawRequest.CampaignId;
                    withdrawRequest1.createdby = withdrawRequest.createdby;
                    withdrawRequest1.Updateddby = withdrawRequest.Updateddby;
                    withdrawRequest1.CreatedOn = withdrawRequest.CreatedOn;
                    withdrawRequest1.bankId = withdrawRequest.bankId;
                    withdrawRequest1.IsApprovedByAdmin = 1;
                    withdrawRequest1.isWithdrawn = withdrawRequest.isWithdrawn;
                    withdrawRequest1.IsRejectedByAdmin = withdrawRequest.IsRejectedByAdmin;
                    withdrawRequest1.RejectedReason = withdrawRequest.RejectedReason;
                    withdrawRequest1.Approvedon = DateTime.UtcNow;
                    withdrawRequest1.withdrawStatus = "Approved By Admin";
                    Entity.Tbl_WithdrawRequestHistory.Add(withdrawRequest1);
                    await Entity.SaveChangesAsync();

                    SendMailBase(withdrawRequest.createdby, "", "GivingActually- Withdrawal Approval Status",
                        "Please be informed that your withdrawal request has been approved for the campaign " + viewModel.CampaignId.ToString());


                }
                return true;
            }
            catch (Exception ex) { throw ex; }
        }
        public async Task<bool> RejectWithdrawRequest(ApproveWithdrawModel viewModel)
        {
            try
            {
                var withdrawRequest = (from S in Entity.Tbl_CampaignWithdrawRequest where S.id == viewModel.WithdrawalId select S).FirstOrDefault();

                if (withdrawRequest != null)
                {
                    withdrawRequest.Approvedon = DateTime.UtcNow;
                    withdrawRequest.IsApprovedByAdmin = 0;
                    withdrawRequest.withdrawStatus = "Rejected By Admin";
                    withdrawRequest.IsRejectedByAdmin = 1;
                    withdrawRequest.RejectedReason = viewModel.RejectedReason;
                    Tbl_WithdrawRequestHistory withdrawRequest1 = new Tbl_WithdrawRequestHistory();
                    withdrawRequest1.withdrawalAmount = withdrawRequest.withdrawalAmount;
                    withdrawRequest1.withdrawReason = withdrawRequest.withdrawReason;
                    withdrawRequest1.withdrawStatus = withdrawRequest.withdrawStatus;
                    withdrawRequest1.CampaignId = withdrawRequest.CampaignId;
                    withdrawRequest1.createdby = withdrawRequest.createdby;
                    withdrawRequest1.Updateddby = withdrawRequest.Updateddby;
                    withdrawRequest1.CreatedOn = withdrawRequest.CreatedOn;
                    withdrawRequest1.bankId = withdrawRequest.bankId;
                    withdrawRequest1.IsApprovedByAdmin = 0;
                    withdrawRequest1.isWithdrawn = withdrawRequest.isWithdrawn;
                    withdrawRequest1.IsRejectedByAdmin = 1;
                    withdrawRequest1.RejectedReason = viewModel.RejectedReason;
                    withdrawRequest1.Approvedon = DateTime.UtcNow;
                    withdrawRequest1.withdrawStatus = "Rejected By Admin";
                    Entity.Tbl_WithdrawRequestHistory.Add(withdrawRequest1);
                    await Entity.SaveChangesAsync();

                    SendMailBase(withdrawRequest.createdby, "", "GivingActually- Withdrawal Approval Status", "Please be informed that your withdrawal request has been rejected for the campaign.Please check the reason in the website. " + viewModel.CampaignId.ToString());

                }
                return true;
            }
            catch (Exception ex) { throw ex; }
        }


        public async Task<bool> ApproveCampaignBank(ApproveBankVwModel viewModel)
        {
            try
            {
                var beneficiaryEntry = (from S in Entity.tbl_CmpnBenBankDetails where S.campaignId == viewModel.CampaignId select S).FirstOrDefault();

                if (beneficiaryEntry != null)
                {
                    beneficiaryEntry.IsApprovedByAdmin = 1;
                    beneficiaryEntry.IsRejectedByAdmin = 0;
                    beneficiaryEntry.ApprovedOn = DateTime.UtcNow;
                    await Entity.SaveChangesAsync();

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RejectCampaignBank(ApproveBankVwModel viewModel)
        {
            try
            {
                var beneficiaryEntry = (from S in Entity.tbl_CmpnBenBankDetails where S.campaignId == viewModel.CampaignId select S).FirstOrDefault();

                if (beneficiaryEntry != null)
                {
                    beneficiaryEntry.IsApprovedByAdmin = 0;
                    beneficiaryEntry.IsRejectedByAdmin = 1;
                    beneficiaryEntry.RejectedReason = viewModel.RejectedReason;
                    beneficiaryEntry.ApprovedOn = DateTime.UtcNow;
                    await Entity.SaveChangesAsync();

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region delete campaign
        public bool DeleteACampaign(int Id)
        {
            try
            {
                using (var dbContext = new GivingActuallyEntities())
                {
                    var Tresults = dbContext.DeleteACampaign(Id);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string deleteImagePhase2(DeleteImageModel viewModel)
        {
            try
            {
                string result = "";
                var beneficiaryEntry = (from S in Entity.Tbl_BeneficiaryDetails
                                        where S.StoryId == viewModel.campaignId && S.DPPath == viewModel.DPPath
                                        select S).FirstOrDefault();

                if (beneficiaryEntry != null)
                {

                    beneficiaryEntry.DPPath = "";
                    beneficiaryEntry.UpdatedOn = DateTime.UtcNow;
                    Entity.SaveChanges();
                    result = "Image Deleted Successfully";
                    return result;
                }
                else
                {
                    result = "No such Image available for this campaign";
                    return result;
                }
            }
            catch (Exception ex)
            { throw ex; }
        }


        public string DeleteCampaignPhase3Image(DeleteImageModel viewModel)
        {
            try
            {
                string result = "";

                var beneficiaryEntry = (from S in Entity.Tbl_StoriesAttachment
                                        where S.StoryId == viewModel.campaignId && S.DPPath == viewModel.DPPath
                                        select S).FirstOrDefault();

                if (beneficiaryEntry != null)
                {

                    beneficiaryEntry.DPPath = "";
                    beneficiaryEntry.UpdatedOn = DateTime.UtcNow;
                    Entity.SaveChanges();
                    result = "Image Deleted Successfully";
                    return result;
                }
                else
                {
                    result = "No such Image available for this campaign";
                    return result;
                }

            }
            catch (Exception ex)
            { throw ex; }
        }

        #endregion

        #region Ask for An Update
        public string AskForUpdate(int Id, int UserId = 0, string DisplayName = "")
        {
            try
            {

                var res = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                string resultstr = "No Campaign with this Id";
                if (res != null)
                {

                    var StoryCategory = (StoryCategory)res.Category;
                    var CategoryName = Enum.GetName(typeof(StoryCategory), res.Category);//StoryCategory.DisplayName();

                    var CampaignTitle = res.Title != null ? res.Title : "";
                    var CreatedUser = res.CreatedBy;

                    //if (Model.UserId == UserId)
                    //    Model.loggedinUser = true;
                    var user = GetUserDetailbyName(CreatedUser);

                    var Subject = "GivingActually- Update";
                    var body = string.Format(@"Hi {0},<br /><br />The Donor/Supporter of your Campaign,<strong> {1} </strong> is looking for an update for the campaign created under givingactually. Please Let them know the current situation by posting an update. <br /><br />Thank You. <br> <br/>please ignore this , incase if you have already posted an update.", user.DisplayName, DisplayName);

                    var result = SendMailBase(CreatedUser, user.DisplayName, Subject, body);
                    if (result)
                    { resultstr = "Mail sent to the Organiser Successfully"; }
                    else
                    {
                        resultstr = "Issue Sending Mail to the Organiser. Please try again later";
                    }
                }
                return resultstr;
            }
            catch (Exception ex)
            { throw ex; }
        }

        public bool SendMailBase(string UserName, string DisplayName, string Subject, string Body)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            string msg = string.Empty;
            try
            {
                MailAddress fromAddress = new MailAddress("contact@givingactually.com", "The GivingActually");
                message.From = fromAddress;
                message.To.Add(new MailAddress(UserName, DisplayName));
                message.Subject = Subject;
                message.IsBodyHtml = true;
                message.Body = Body;//string.Format(@"Hi {0},<br /><br /><strong>Your password is {1}.</strong><br /><br />Thank You.", DisplayName, Password);

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

        public async Task<List<CampaignBankModel>> GetBankDetailForApprovals()
        {
            try
            {
                //var query = from item in Entity.tbl_CmpnBenBankDetails.AsNoTracking()
                //            join Cit in Entity.Tbl_Campaign.AsNoTracking() on item.campaignId equals Cit.Id
                //            where item.IsApprovedByAdmin==0
                //            select new { item.campaignId,item.BankName,item.AccountNumber,item.BankBranch,item.BenName,item.IFSC,
                //                item.IsApprovedByAdmin,
                //            item.IsRejectedByAdmin,item.RejectedReason,item.ApprovedOn, Cit.Title};

                var query = from item in Entity.tbl_CmpnBenBankDetails.AsNoTracking()
                            where item.campaignId == 45
                            orderby item.CreatedOn descending
                            select item;
                var result = await query.ToListAsync();
                List<CampaignBankModel> bankList = new List<CampaignBankModel>();
                foreach (var viewModel in result)
                {
                    CampaignBankModel BanksDetail = new CampaignBankModel();
                    if (viewModel != null)
                    {
                        BanksDetail.CampaignId = viewModel.campaignId.Value;
                        BanksDetail.BankName = viewModel.BankName;
                        BanksDetail.AccountNumber = viewModel.AccountNumber;
                        BanksDetail.BankBranch = viewModel.BankBranch;
                        BanksDetail.BenName = viewModel.BenName;
                        BanksDetail.IFSC = viewModel.IFSC;
                        BanksDetail.isApproved = (viewModel.IsApprovedByAdmin == 1 ? true : false);
                        BanksDetail.isRejected = (viewModel.IsRejectedByAdmin == 1 ? true : false);
                        BanksDetail.RejectedReason = viewModel.RejectedReason;
                        BanksDetail.ApprovedOn = viewModel.ApprovedOn != null ? viewModel.ApprovedOn.Value : DateTime.MinValue;
                        //  BanksDetail.CampaignTitle = viewModel.Title;

                    }
                    bankList.Add(BanksDetail);
                }
                return bankList;
            }
            catch (Exception ex) { throw ex; }
        }
        #endregion
    }

    internal class QueryResult
    {
        public decimal SumDonation { get; set; }
        public bool isAnanymous { get; set; }
        public string DonatedBy { get; set; }
        public DateTime DonatedOn { get; set; }
        public string Email { get; set; }
        public decimal SumReceivedDonation { get; set; }
        public decimal SumGST { get; set; }
        public decimal SumProcessingFee { get; set; }
    }
}