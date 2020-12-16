using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GivingActuallyAPI.Models
{
    public class APIMiniViewModel
    {
    }
    public class CampaignModelsList
    {
        public CampaignModelsList()
        { CampaignLists = new List<CampaignModel>(); }
        public List<CampaignModel> CampaignLists { get; set; }
        public double PageCount { get; set; }
        public int CurrentPageIndex { get; set; }
        public int TotalCampaigns { get; set; }
    }
    public class UserCampaignModelsList
    {
        public UserCampaignModelsList()
        { CampaignLists = new List<UserCampaignModel>(); }
        public List<UserCampaignModel> CampaignLists { get; set; }
        public double PageCount { get; set; }
        public int CurrentPageIndex { get; set; }
        public int TotalCampaigns { get; set; }
    }

    public class UserStatisticsModel
    {
        public int userId { get; set; }
        public long TotalCampaigns { get; set; }
        public decimal TotalGoals { get; set; }
        public decimal TotalRecievedMoney { get; set; }
        public long TotalDonors { get; set; }
    }
    public class SerachModel
    {
        public CampaignModel Model { get; set; }
        public bool isEMptyModel { get; set; }
    }

    public class CampaignMiniDonation
    {

        public int id { get; set; }
        public int CampaignId { get; set; }
        public decimal DonationAmnt { get; set; }
        public string PhoneNumber { get; set; }
        public string EMail { get; set; }
        public string DonationMoneyType { get; set; }
        public string PlaceName { get; set; }
        public string DonorName { get; set; }
        public bool isAnanymous { get; set; }

    }

    public class RazorPayResponseclass
    {
        public string razorpay_order_id { get; set; }
        public string razorpay_payment_id { get; set; }
        public string razorpay_signature { get; set; }
    }

    public class CampaignVwModel1
    {
        public int Id { get; set; }
        public string CategoryType { get; set; }
        public string CampaignTitle { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string BGroupName { get; set; }
        public string BName { get; set; }
        public string NGOName { get; set; }
        public string BeneficiaryType { get; set; }
        public decimal CampaignTargetMoney { get; set; }
        public string CampaignTargetMoneyType { get; set; }
        public DateTime CampaignTargetDate { get; set; }
        public string UserDisplayName { get; set; }
    }

    public class LoanViewModel
    {
        public int Id { get; set; }
        public string CategoryType { get; set; }
        public string LoanTitle { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string BenName { get; set; }
        public string BeneficiaryType { get; set; }
        public decimal LoanTargetMoney { get; set; }
        public string LoanTargetMoneyType { get; set; }
        public DateTime LoanTargetDate { get; set; }
        public string UserDisplayName { get; set; }
        public string FieldPartnerName { get; set; }
        public int FieldPartnerId { get; set; }
        public decimal RePaymentInterestPerc { get; set; }
        public int RepaymentTermDays { get; set; }
        public string RepaymentTerm { get; set; }
        public DateTime RePaymentStartDate { get; set; }
        public DateTime RePaymentEndDate { get; set; }
    }

    public class CampaignBankVwModel
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string BenName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSC { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class ApproveBankVwModel
    {
        public int BankId { get; set; }
        public int CampaignId { get; set; }
        public decimal isApproved { get; set; }
        public string RejectedReason { get; set; }
        public string isRejected { get; set; }
        public DateTime AdminUpdatedon { get; set; }
    }
    public class CampaignWithdrawModel
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string WithDrawalReason { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public string WithDrawalStatus { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class ApproveWithdrawModel
    {
        public int WithdrawalId { get; set; }
        public int CampaignId { get; set; }
        public string ApprovalReason { get; set; }
        public decimal isApproved { get; set; }
        public string RejectedReason { get; set; }
        public string isRejected { get; set; }
        public DateTime AdminUpdatedon { get; set; }
    }
    public class WithdrawModel
    {
        public WithdrawModel()
        {
            BeneBank = new CampaignBankVwModel();
        }
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string WithDrawalReason { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public string WithDrawalStatus { get; set; }
        public CampaignBankVwModel BeneBank { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class ApprovalWithdrawModel
    {
       
        public int WithdrawalId { get; set; }
        public int CampaignId { get; set; }
        public string WithDrawalReason { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public string WithDrawalStatus { get; set; }
        public int BankID { get; set; }
        public bool isApproved { get; set; }
        public bool isRejected { get; set; }
        public bool isBankApproved { get; set; }
        public bool isBankRejected { get; set; }
        public string RejectedReason { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class CampaignWithdrawHistoryList
    {

        public List<WithdrawModel> WithDrawalList { get; set; }
    }
    public class CampaignBankViewModel
    {
        public int BankId { get; set; }
        public int CampaignId { get; set; }
        public string BenName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSC { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }

        public bool isApproved { get; set; }
        public bool isRejected { get; set; }
        public string RejectedReason { get; set; }
        public DateTime ApprovedOn { get; set; }
    }
    public class CampaignBankModel
    {
        public int BankId { get; set; }
        public int CampaignId { get; set; }
        public string BenName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSC { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public bool isApproved { get; set; }
        public bool isRejected { get; set; }
        public string RejectedReason { get; set; }
        public DateTime ApprovedOn { get; set; }
        public string CampaignTitle { get; set; } 
    }

    public class CampaignPhase2Full
    {

        public int Id { get; set; }
        public int campaignId { get; set; }
        public string placeName { get; set; }
        public string Latitude { get; set; }
        public string longitude { get; set; }
        public string BDisplayPicName { get; set; }
        public string BDPContentType { get; set; }
        public HttpPostedFile DisplayPicFile { get; set; }
        public string UserName { get; set; }
    }
    public class CampaignPhase2Desc
    {
        public int Id { get; set; }
        public int campaignId { get; set; }
        public string placeName { get; set; }
        public string Latitude { get; set; }
        public string longitude { get; set; }
    }
    public class CampaignPhase2Image
    {
        public int Id { get; set; }
        public int campaignId { get; set; }
        public string BDisplayPicName { get; set; }
        public string BDPContentType { get; set; }
        public HttpPostedFile DisplayPicFile { get; set; }
        public string UserName { get; set; }
    }

    public class CampaignPhase3Ful
    {
        public CampaignPhase3Desc Desc { get; set; }
        public CampaignPhase3Image Image { get; set; }
    }
    public class CampaignPhase3Desc
    {
        public int Id { get; set; }
        public int campaignId { get; set; }
        public string StoryDescription { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
    }
    public class CampaignPhase3Image
    {
        public int Id { get; set; }
        public int campaignId { get; set; }
        public List<Attachment> Attachments { get; set; }
        public string UserName { get; set; }
        public int UpdateId { get; set; }
    }
    public class Attachment
    {
        public int AttId { get; set; }
        public HttpPostedFile File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public int updtId { get; set; }
        public int Index { get; set; }
    }
    public class ResponseObject
    {
        public string campaignId { get; set; }
        public string userId { get; set; }
        public string UpdateId { get; set; }
        public string ResponseMsg { get; set; }
        public string ExceptionMsg { get; set; }
        public string ErrorCode { get; set; }
        public string UserDPpath { get; set; }
    }
    public class DeleteImageModel
    {
        public int campaignId { get; set; }
        public string DPPath { get; set; }
    }

    public class returnModel
    {
        public CampaignModel CampaignModel { get; set; }
        public bool IsActive { get; set; }
    }
    public class CampaignModel
    {
        public int Id { get; set; }
        public bool IsApprovedbyAdmin { get; set; }
        public string CampaignTitle { get; set; }
        public string CampaignDescriptionDtl { get; set; }
        public string CampaignDescription { get; set; }
        public string CategoryName { get; set; }
        public string placeName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string FullplaceName { get; set; }
        public decimal CampaignTargetMoney { get; set; }
        public string CampaignTargetMoneyType { get; set; }
        public DateTime CampaignTargetDate { get; set; }
        public decimal RaisedAmount { get; set; }
        public long RaisedBy { get; set; }
        public int RaisedPercentage { get; set; }
        public int DaysLeft { get; set; }
        public long Totalsupporters { get; set; }
        public string OrganizerName { get; set; }
        public string DisplayInitial { get; set; }
        public string UserDPImage { get; set; }
        public string OrganizerPictureUrl { get; set; }
        public string BDisplayPicName { get; set; }
        public string BDisplayPicPath { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserId { get; set; }
        public bool isNGOUser { get; set; }
        public string CampaignStatus { get; set; }


    }

    public class UserCampaignModel
    {
        public int Id { get; set; }
        public bool IsApprovedbyAdmin { get; set; }
        public string CampaignTitle { get; set; }
        public string CampaignDescriptionDtl { get; set; }
        public string CampaignDescription { get; set; }
        public string CategoryName { get; set; }
        public string placeName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string FullplaceName { get; set; }
        public decimal CampaignTargetMoney { get; set; }
        public string CampaignTargetMoneyType { get; set; }
        public DateTime CampaignTargetDate { get; set; }
        public decimal RaisedAmount { get; set; }
        public long RaisedBy { get; set; }
        public int RaisedPercentage { get; set; }
        public int DaysLeft { get; set; }
        public long Totalsupporters { get; set; }
        public string OrganizerName { get; set; }

        public string UserDPImage { get; set; }
        public string OrganizerPictureUrl { get; set; }
        public string BDisplayPicName { get; set; }
        public string BDisplayPicPath { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserId { get; set; }
        public bool isNGOUser { get; set; }
        public string CampaignStatus { get; set; }

        public long CommentTotalCount { get; set; }
        public long ShareTotalCount { get; set; }
        public bool isBankAdded { get; set; }
        public bool isBankVerified { get; set; }
        public bool iswithdrawalavailable { get; set; }
        public decimal WithDrawnAmount { get; set; }
        public decimal ELigibleAmtForWithdw { get; set; }
        public long LikesTotalCount { get; set; }
        public long EndorseTotalCount { get; set; }
        public long DonorsTotalCount { get; set; }

    }
    public class CampaignDetailModel
    {
        public CampaignDetailModel()
        {
            campaignDescription = new CampaignDescription();
            UploadedImages = new List<Files>();
            Updates = new List<CampaignUpdates>();
            Comments = new List<CommentsVM>();
            CampaignDonationList = new List<CampaignDonation>();
            EndorsementsList = new ENdorsementList();
        }
        public int Id { get; set; }
        public byte[] BDisplayPic { get; set; }
        public string BDisplayPicPath { get; set; }
        public string CampaignTitle { get; set; }
        public string CategoryName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public List<Files> UploadedImages { get; set; }
        public bool isCtNGOEndorsed { get; set; }
        public ENdorsementList EndorsementsList { get; set; }
        public CampainOrganizerViewModel CampainOrganizer { get; set; }
        public bool IsApprovedbyAdmin { get; set; }
        public CampaignDonation CampaignDonations { get; set; }
        public List<CampaignDonation> CampaignDonationList { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
        public string BGroupName { get; set; }
        public string BenificiaryName { get; set; }
        public string NGOName { get; set; }
        public string BeneficiaryType { get; set; }
        public decimal CampaignTargetMoney { get; set; }
        public bool iswithdrawalavailable { get; set; }
        public decimal WithDrawnAmount { get; set; }
        public decimal ELigibleAmtForWithdw { get; set; }
        public string CampaignTargetMoneyType { get; set; }
        public CampaignTargetModel CampaignTarget { get; set; }
        public DateTime CampaignTargetDate { get; set; }

        public int DaysLeft { get; set; }
        public long Totalsupporters { get; set; }
        public string OrganizerName { get; set; }
        public string OrganizerPictureUrl { get; set; }
        public CampaignDescription campaignDescription { get; set; }
        public List<CampaignUpdates> Updates { get; set; }
        public List<CommentsVM> Comments { get; set; }
        public decimal RaisedAmount { get; set; }
        public long RaisedBy { get; set; }
        public long sharecount { get; set; }
        public bool IsSupportedByCtUser;
        public int RaisedPercentage { get; set; }
        public long CommentCount { get; set; }
        public long LikeCount { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public bool loggedinUser { get; set; }
        public bool isNGOUser { get; set; }
        public string CampaignStatus { get; set; }
        public bool IsEndorsedByUser { get; set; }
        public bool CanEnableDonate { get; set; }

    }

    public class commentsModel
    {
        public List<CommentsVM> AllComments { get; set; }
        public int CommentsCount { get; set; }
    }
    public class commentModel
    {
        public int campaignId { get; set; }
        public string CommentText { get; set; }
        public int commentId { get; set; }
        public int userId { get; set; }
    }
    public class Comment
    {
        public int ComID { get; set; }
        public string CommentMsg { get; set; }
        public DateTime CommentedDate { get; set; }
        public int campaignId { get; set; }
        public int CreatedbyId { get; set; }
        public string CreatedbyUserName { get; set; }
        public string CreatedbyDpName { get; set; }

    }
    public class SharesModel
    {
        public List<share> AllShares { get; set; }
        public int SharesCount { get; set; }
    }
    public class share
    {
        public int campaignId { get; set; }
        public int ShareId { get; set; }
        public int SharedbyUserId { get; set; }
        public string Media { get; set; }
        public string SharedbyUserName { get; set; }
        public string SharedbyDpName { get; set; }
    }
    public class shareModel
    {
        public int campaignId { get; set; }
        public int ShareId { get; set; }
        public int UserId { get; set; }
        public string Media { get; set; }
    }

    public class LikesModel
    {
        public List<Like> AllLikes { get; set; }
        public int LikesCount { get; set; }
    }
    public class Like
    {
        public int campaignId { get; set; }
        public int LikeId { get; set; }
        public int LikebyUserId { get; set; }
        public string LikebyUserName { get; set; }
        public string LikebyDpName { get; set; }
    }
    public class LikeModel
    {
        public int campaignId { get; set; }
        public int LikeId { get; set; }
        public int UserId { get; set; }
    }

    public class DonationsModel
    {
        public List<Donation> AllDonors { get; set; }
        public int DonorsCount { get; set; }
        public decimal TotolRaisedAmnt { get; set; }
        public decimal TotolPaymentProcFee { get; set; }
        public decimal TotolPaymentGSTFee { get; set; }
        public decimal TotolActualdonationAmnt { get; set; }
        public int RaisedPercentage { get; set; }
        public bool CanEnableDonate { get; set; }
    }
    public class Donation
    {
        public int id { get; set; }
        public int CampaignId { get; set; }
        public string DonorsEMail { get; set; }
        public string DonationMoneyType { get; set; }
        public string PlaceName { get; set; }
        public string DonorsName { get; set; }
        public DateTime DonatedOn { get; set; }
        public string UserDPImage { get; set; }
        public string DonorsPhNo { get; set; }
        public decimal DonationAmt { get; set; }
        public decimal PaymentProcFee { get; set; }
        public decimal PaymentGSTFee { get; set; }
        public decimal ActualDonationAmnt {get;set;}
    }
    //public class DonationModel
    //{
    //    public int campaignId { get; set; }
    //    public decimal DonationAmnt { get; set; }
    //    public string DonationMoneyType { get; set; }
    //    public int countryName { get; set; }
    //    public string pincode { get; set; }
    //    public string EMail { get; set; }
    //    public string PlaceName { get; set; }
    //    public string Latitude { get; set; }
    //    public string Longitude { get; set; }
    //    public string IdentityName { get; set; }
    //    [Display(Name = "Make your Donation as Ananymous")]
    //    public bool isAnanymous { get; set; }
    //    public string Status { get; set; }
    //    public string DonatedBy { get; set; }
    //    public DateTime DonatedOn { get; set; }
    //    public string UpdatedBy { get; set; }
    //    public string UpdatedOn { get; set; }
    //    [Display(Name = "Phone")]
    //    [Required]
    //    public string PhNo { get; set; }

    //}
}
