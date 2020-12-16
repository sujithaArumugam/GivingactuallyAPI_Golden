using firstWebAPI.DataLayer;
using GivingActuallyAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using static GivingActuallyAPI.Models.Helper;

namespace GivingActuallyAPI.Services
{
    public class LoanService
    {
        GivingActuallyEntities Entity = new GivingActuallyEntities();

        public async Task<int> CreateLoanPhase1(LoanViewModel viewModel)
        {
            try
            {

                if (viewModel.Id == 0)
                {
                    Tbl_Loan NewCampaign = new Tbl_Loan();

                    NewCampaign.UserId = viewModel.UserId;
                    NewCampaign.IsApprovedbyAdmin = false;
                    NewCampaign.Title = viewModel.LoanTitle;
                    NewCampaign.IsActive = true;
                    NewCampaign.CreatedBy = viewModel.UserName;
                    NewCampaign.CreatedOn = DateTime.UtcNow;
                    StoryCategory cate = (StoryCategory)Enum.Parse(typeof(StoryCategory), viewModel.CategoryType);
                    int CateId = (int)cate;

                    NewCampaign.Category = CateId;
                    MoneyType MnyTyp = (MoneyType)Enum.Parse(typeof(MoneyType), viewModel.LoanTargetMoneyType);
                    int MoneyType = (int)MnyTyp;

                    NewCampaign.MoneyType = MoneyType.ToString();
                    NewCampaign.TargetAmount = viewModel.LoanTargetMoney;
                    NewCampaign.TargetDate = viewModel.LoanTargetDate != null ? viewModel.LoanTargetDate : DateTime.Now.AddDays(31);
                    BeneficiaryType BenTyp = (BeneficiaryType)Enum.Parse(typeof(BeneficiaryType), viewModel.BeneficiaryType);
                    int BenType = (int)BenTyp;

                    NewCampaign.BeneficiaryType = BenType;
                    NewCampaign.BName = viewModel.BenName != null ? viewModel.BenName : "";
                    NewCampaign.BGroupName = viewModel.BenName != null ? viewModel.BenName : "";
                    NewCampaign.NGOName = viewModel.BenName != null ? viewModel.BenName : "";


                    string CanApprove = ConfigurationManager.AppSettings["CanApprove"];
                    bool CanApprovalFlag = CanApprove == "true" ? true : false;
                    NewCampaign.IsApprovedbyAdmin = CanApprovalFlag;
                    NewCampaign.CreatedUserName = viewModel.UserDisplayName;
                    NewCampaign.FieldPartner = viewModel.FieldPartnerId;
                    //NewCampaign.FieldPartnerName = viewModel.FieldPartnerName;
                    NewCampaign.RePaymentTerm = viewModel.RepaymentTerm;
                    NewCampaign.RePaymentTermDays = viewModel.RepaymentTermDays;
                    NewCampaign.RePaymentStartDate = viewModel.RePaymentStartDate;

                    NewCampaign.RePaymentEndDate = viewModel.RePaymentEndDate;
                    NewCampaign.RePaymentInterestPerc = viewModel.RePaymentInterestPerc;
                    Entity.Tbl_Loan.Add(NewCampaign);
                    await Entity.SaveChangesAsync();
                    var NewCampaignId = NewCampaign.Id;
                    return NewCampaignId;
                }
                else
                {
                    var ExistingCampaign = (from S in Entity.Tbl_Loan where S.Id == viewModel.Id select S).FirstOrDefault();
                    if (ExistingCampaign != null)
                    {
                        ExistingCampaign.UserId = viewModel.UserId;
                        ExistingCampaign.IsApprovedbyAdmin = false;
                        ExistingCampaign.Title = viewModel.LoanTitle;
                        ExistingCampaign.IsActive = true;
                        ExistingCampaign.CreatedBy = viewModel.UserName;
                        ExistingCampaign.CreatedOn = DateTime.UtcNow;
                        StoryCategory cate = (StoryCategory)Enum.Parse(typeof(StoryCategory), viewModel.CategoryType);
                        int CateId = (int)cate;

                        ExistingCampaign.Category = CateId;
                        MoneyType MnyTyp = (MoneyType)Enum.Parse(typeof(MoneyType), viewModel.LoanTargetMoneyType);
                        int MoneyType = (int)MnyTyp;

                        ExistingCampaign.MoneyType = MoneyType.ToString();
                        ExistingCampaign.TargetAmount = viewModel.LoanTargetMoney;
                        ExistingCampaign.TargetDate = viewModel.LoanTargetDate != null ? viewModel.LoanTargetDate : DateTime.Now.AddDays(31);
                        BeneficiaryType BenTyp = (BeneficiaryType)Enum.Parse(typeof(BeneficiaryType), viewModel.BeneficiaryType);
                        int BenType = (int)BenTyp;

                        ExistingCampaign.BeneficiaryType = BenType;
                        ExistingCampaign.BName = viewModel.BenName != null ? viewModel.BenName : "";
                        ExistingCampaign.BGroupName = viewModel.BenName != null ? viewModel.BenName : "";
                        ExistingCampaign.NGOName = viewModel.BenName != null ? viewModel.BenName : "";


                        string CanApprove = ConfigurationManager.AppSettings["CanApprove"];
                        bool CanApprovalFlag = CanApprove == "true" ? true : false;
                        ExistingCampaign.IsApprovedbyAdmin = CanApprovalFlag;
                        ExistingCampaign.CreatedUserName = viewModel.UserDisplayName;
                        ExistingCampaign.FieldPartner = viewModel.FieldPartnerId;
                        //ExistingCampaign.FieldPartnerName = viewModel.FieldPartnerName;
                        ExistingCampaign.RePaymentTerm = viewModel.RepaymentTerm;
                        ExistingCampaign.RePaymentTermDays = viewModel.RepaymentTermDays;
                        ExistingCampaign.RePaymentStartDate = viewModel.RePaymentStartDate;

                        ExistingCampaign.RePaymentEndDate = viewModel.RePaymentEndDate;
                        ExistingCampaign.RePaymentInterestPerc = viewModel.RePaymentInterestPerc;
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
        public async Task<bool> CreateLoanPhase2(CampaignPhase2Full viewModel)
        {
            try
            {
                var beneficiaryEntry = (from S in Entity.Tbl_LoanBenDetails where S.StoryId == viewModel.campaignId select S).FirstOrDefault();

                if (beneficiaryEntry == null)
                {
                    Tbl_LoanBenDetails beneficiary = new Tbl_LoanBenDetails();
                    beneficiary.StoryId = viewModel.campaignId;
                    beneficiary.Status = true;
                    beneficiary.BResidence = getLocation(viewModel.placeName, viewModel.Latitude, viewModel.longitude);

                    string text = "Loan" + viewModel.campaignId.ToString();
                    if (viewModel.DisplayPicFile != null)
                    {
                        beneficiary.DPPath = AddtoStorage(viewModel.DisplayPicFile, text);
                        beneficiary.DP = new byte[] { };
                        beneficiary.DPName = viewModel.BDisplayPicName != null ? viewModel.BDisplayPicName : "";
                    }
                    beneficiary.CreatedBy = viewModel.UserName;
                    beneficiary.CreatedOn = DateTime.UtcNow;

                    Entity.Tbl_LoanBenDetails.Add(beneficiary);
                    await Entity.SaveChangesAsync();
                    var NewBEneficiaryId = beneficiary.Id;
                    return true;
                }
                else
                {
                    var beneficiary = (from S in Entity.Tbl_LoanBenDetails where S.StoryId == viewModel.campaignId select S).FirstOrDefault();

                    if (beneficiary != null)
                    {
                        if (viewModel.placeName != null)
                        {
                            beneficiary.BResidence = getLocation(viewModel.placeName, viewModel.Latitude, viewModel.longitude);

                        }
                        if (viewModel.DisplayPicFile != null)
                        {

                            string text = "Loan" + viewModel.campaignId.ToString();
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
            {
                throw ex;
            }
        }
        public int getLocation(string Placename, string latitude, string longitude)
        {
            try
            {
                Tbl_LoanCityDetails placedetails = new Tbl_LoanCityDetails();

                placedetails.CityName = Placename;
                placedetails.Latitude = latitude;
                placedetails.longitude = longitude;
                Entity.Tbl_LoanCityDetails.Add(placedetails);
                Entity.SaveChanges();
                return placedetails.CityId;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CampaignsListViewModel GetLoanCampaignsbyPage(int CategoryId = -1, int page = 1)
        {
            try
            {
                int maxRows = 12;
                CampaignsListViewModel ModelList = new CampaignsListViewModel();
                var result = (from S in Entity.Tbl_Loan select S).Where(s => CategoryId != -1 ? s.Category == CategoryId : true).ToList()
                     ;
                result = result.Where(S => S.IsApprovedbyAdmin && S.IsActive).ToList();
                var res = result.OrderByDescending(a => a.CreatedOn)
                        .Skip((page - 1) * maxRows)
                        .Take(maxRows).ToList();
                if (res.Any())
                {
                    foreach (var item in res)
                    {
                        CampaignMainViewModel Model = new CampaignMainViewModel();
                        Model = GetLoanCamapignForList(item.Id);
                        ModelList.CampaignViewModelList.Add(Model);
                    }

                }
               
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
        public CampaignMainViewModel GetLoanCamapignForList(int Id)
        {
            try
            {
               
                CampaignMainViewModel Model = new CampaignMainViewModel();
                var res = (from S in Entity.Tbl_Loan where S.Id == Id select S).FirstOrDefault();
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


                    var Beneficiary = (from S in Entity.Tbl_LoanBenDetails where S.StoryId == Id select S).FirstOrDefault();
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

        public async Task<CampaignModelsList> GetLoanCampaigns_sp(int CategoryId = -1, int page = 1, int page_size = 12,
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
                        var query = from item in Entity.Tbl_Loan.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.IsActive && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CreatedOn ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CampaignTargetMoney")
                    {
                        var query = from item in Entity.Tbl_Loan.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.IsActive && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetAmount descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "DaysLeft")
                    {
                        var query = from item in Entity.Tbl_Loan.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.IsActive && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.TargetDate ascending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "Totalsupporters")
                    {
                        var query = from item in Entity.Tbl_Loan.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.IsActive && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.CountUsage descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();

                        CampaignIdList.AddRange(CampaignRes.Select(a => a));//Totalsupporters
                    }
                    else if (SortBy == "CampaignTitle")
                    {
                        var query = from item in Entity.Tbl_Loan.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.IsActive && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Title descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "CategoryName")
                    {
                        var query = from item in Entity.Tbl_Loan.AsNoTracking()
                                    where item.IsApprovedbyAdmin && item.IsActive && (CategoryId != -1 ? item.Category == CategoryId : true)
                                    orderby item.Category descending
                                    select item.Id;
                        var CampaignRes = await query.ToListAsync();
                        CampaignIdList.AddRange(CampaignRes.Select(a => a));
                    }
                    else if (SortBy == "placeName")
                    {
                        var query = (from ben in Entity.Tbl_LoanBenDetails.AsNoTracking()
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

              //  ModelList = await getcampainCommonTest_sp(CampaignIdList, page, maxRows);
                return ModelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

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

        public CampainOrganizerViewModel createViewModelOrganizer(Tbl_LoanBenDetails beneficiary)
        {
            try
            {
                CampainOrganizerViewModel Organizer = new CampainOrganizerViewModel();
                Organizer.storyId = beneficiary.StoryId;
                //Organizer.CountryId = Convert.ToInt32(beneficiary.BCountry);
                Organizer.Bresidence = Convert.ToInt32(beneficiary.BResidence);

                var res = (from S in Entity.Tbl_LoanCityDetails where S.CityId == Organizer.Bresidence select S).FirstOrDefault();
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
        public async Task<bool> CreateLoanPhase3Desc(CampaignPhase3Desc viewModel)
        {
            try
            {
                var exisitingStorydesc = (from S in Entity.Tbl_LoanDescription where S.StoryId == viewModel.campaignId select S).FirstOrDefault();
                if (exisitingStorydesc == null)
                {
                    Tbl_LoanDescription NewStorydesc = new Tbl_LoanDescription();
                    NewStorydesc.storyDescription = viewModel.StoryDescription;
                    NewStorydesc.StoryId = viewModel.campaignId;
                    NewStorydesc.CreatedBy = viewModel.UserName;
                    NewStorydesc.CreatedOn = DateTime.UtcNow;
                    NewStorydesc.Status = true;
                    Entity.Tbl_LoanDescription.Add(NewStorydesc);
                    await Entity.SaveChangesAsync();
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

        public async Task<bool> CreateLoanPhase3Image(CampaignPhase3Image viewModel)
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

        public async Task<int> createLoanBank(CampaignBankVwModel viewModel)
        {
            try
            {
                var beneficiaryEntry = (from S in Entity.tbl_LoanBankDetails where S.StoryId == viewModel.CampaignId select S).FirstOrDefault();
                var NewBEneficiaryId = 0;
                if (beneficiaryEntry == null)
                {
                    tbl_LoanBankDetails beneficiary = new tbl_LoanBankDetails();
                    beneficiary.StoryId = viewModel.CampaignId;
                    beneficiary.CreatedBy = viewModel.CreatedBy;
                    beneficiary.CreatedOn = DateTime.UtcNow;
                    beneficiary.BankName = viewModel.BankName;
                    beneficiary.AccountNumber = viewModel.AccountNumber;
                    beneficiary.BankBranch = viewModel.BankBranch;
                    beneficiary.BenName = viewModel.BenName;
                    beneficiary.IFSC = viewModel.IFSC;
                    beneficiary.IsApprovedByAdmin = 0;
                    beneficiary.IsRejectedByAdmin = 0;
                    Entity.tbl_LoanBankDetails.Add(beneficiary);
                    await Entity.SaveChangesAsync();
                    NewBEneficiaryId = beneficiary.id;

                }
                else
                {
                    var beneficiaryEn = (from S in Entity.tbl_LoanBankDetails where S.StoryId == viewModel.CampaignId select S).FirstOrDefault();

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

        public async Task<int> createLoanWithdrawalRequest(CampaignWithdrawModel viewModel)
        {
            try
            {
                var beneficiaryEntry = (from S in Entity.Tbl_LoanWithdrawRequest where S.id == viewModel.Id select S).FirstOrDefault();
                var NewBEneficiaryId = 0;
                if (beneficiaryEntry == null)
                {
                    Tbl_LoanWithdrawRequest withdrawRequest = new Tbl_LoanWithdrawRequest();
                    withdrawRequest.StoryId = viewModel.CampaignId;
                    withdrawRequest.createdby = viewModel.CreatedBy;
                    withdrawRequest.CreatedOn = DateTime.UtcNow;
                    withdrawRequest.withdrawalAmount = viewModel.WithdrawalAmount;
                    withdrawRequest.withdrawReason = viewModel.WithDrawalReason;
                    withdrawRequest.withdrawStatus = "Pending for Approval";
                    withdrawRequest.IsApprovedByAdmin = 0;
                    var Bank = (from S in Entity.tbl_LoanBankDetails where S.StoryId == viewModel.CampaignId && S.IsApprovedByAdmin == 1 select S.id).FirstOrDefault();
                    withdrawRequest.bankId = Bank;
                    Entity.Tbl_LoanWithdrawRequest.Add(withdrawRequest);
                    Tbl_LoanWithdrawRequestHistory withdrawRequest1 = new Tbl_LoanWithdrawRequestHistory();
                    withdrawRequest1.StoryId = viewModel.CampaignId;
                    withdrawRequest1.createdby = viewModel.CreatedBy;
                    withdrawRequest1.CreatedOn = DateTime.UtcNow;
                    withdrawRequest1.withdrawalAmount = viewModel.WithdrawalAmount;
                    withdrawRequest1.withdrawReason = viewModel.WithDrawalReason;
                    withdrawRequest1.IsApprovedByAdmin = 0;
                    withdrawRequest1.withdrawStatus = "Pending for Approval";
                    Entity.Tbl_LoanWithdrawRequestHistory.Add(withdrawRequest1);
                    await Entity.SaveChangesAsync();

                    SendMailBase("givingactuallylive@gmail.com", "The GivingActually", "Withdrwal Request for " + viewModel.CampaignId.ToString(), "Please validate and approve the request of withdrawal for the Loan Story id: " + viewModel.CampaignId.ToString());
                    NewBEneficiaryId = withdrawRequest.id;

                }
                else
                {
                    var withdrawRequestEn = (from S in Entity.Tbl_LoanWithdrawRequest where S.id == viewModel.Id select S).FirstOrDefault();

                    if (!(withdrawRequestEn == null))
                    {
                        withdrawRequestEn.withdrawalAmount = viewModel.WithdrawalAmount;
                        withdrawRequestEn.withdrawReason = viewModel.WithDrawalReason;
                        withdrawRequestEn.withdrawStatus = viewModel.WithDrawalStatus;

                        Tbl_LoanWithdrawRequestHistory withdrawRequest1 = new Tbl_LoanWithdrawRequestHistory();
                        withdrawRequest1.StoryId = viewModel.CampaignId;
                        withdrawRequest1.createdby = viewModel.CreatedBy;
                        withdrawRequest1.CreatedOn = DateTime.UtcNow;
                        withdrawRequest1.withdrawalAmount = viewModel.WithdrawalAmount;
                        withdrawRequest1.withdrawReason = viewModel.WithDrawalReason;
                        withdrawRequest1.withdrawStatus = viewModel.WithDrawalStatus;
                        Entity.Tbl_LoanWithdrawRequestHistory.Add(withdrawRequest1);


                        await Entity.SaveChangesAsync();
                        NewBEneficiaryId = withdrawRequestEn.id;

                    }
                }
                return NewBEneficiaryId;
            }
            catch (Exception ex) { throw ex; }
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
    }
}