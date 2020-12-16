﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace firstWebAPI.DataLayer
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class GivingActuallyEntities : DbContext
    {
        public GivingActuallyEntities()
            : base("name=GivingActuallyEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<ExceptionLogger> ExceptionLoggers { get; set; }
        public virtual DbSet<Tbl_BeneficiaryDetails> Tbl_BeneficiaryDetails { get; set; }
        public virtual DbSet<Tbl_Campaign> Tbl_Campaign { get; set; }
        public virtual DbSet<Tbl_CampaignDescription> Tbl_CampaignDescription { get; set; }
        public virtual DbSet<Tbl_CampaignDescriptionUpdates> Tbl_CampaignDescriptionUpdates { get; set; }
        public virtual DbSet<Tbl_CampaignDonation> Tbl_CampaignDonation { get; set; }
        public virtual DbSet<Tbl_CityDetails> Tbl_CityDetails { get; set; }
        public virtual DbSet<Tbl_Endorse> Tbl_Endorse { get; set; }
        public virtual DbSet<Tbl_GeoLocation> Tbl_GeoLocation { get; set; }
        public virtual DbSet<Tbl_Like> Tbl_Like { get; set; }
        public virtual DbSet<Tbl_ParentComment> Tbl_ParentComment { get; set; }
        public virtual DbSet<Tbl_Shares> Tbl_Shares { get; set; }
        public virtual DbSet<Tbl_StoriesAttachment> Tbl_StoriesAttachment { get; set; }
        public virtual DbSet<Tbl_UpdatesAttachment> Tbl_UpdatesAttachment { get; set; }
        public virtual DbSet<Tbl_User> Tbl_User { get; set; }
        public virtual DbSet<Tbl_UserAuthentication> Tbl_UserAuthentication { get; set; }
        public virtual DbSet<tbl_UserNGOEndorsement> tbl_UserNGOEndorsement { get; set; }
        public virtual DbSet<tbl_SearchCampaignResult> tbl_SearchCampaignResult { get; set; }
        public virtual DbSet<tbl_searchResult> tbl_searchResult { get; set; }
        public virtual DbSet<tbl_CommentCount> tbl_CommentCount { get; set; }
        public virtual DbSet<tbl_DntionCount> tbl_DntionCount { get; set; }
        public virtual DbSet<tbl_DonorsCount> tbl_DonorsCount { get; set; }
        public virtual DbSet<tbl_EndorseCount> tbl_EndorseCount { get; set; }
        public virtual DbSet<tbl_LikesCount> tbl_LikesCount { get; set; }
        public virtual DbSet<Tbl_search> Tbl_search { get; set; }
        public virtual DbSet<tbl_placeCount> tbl_placeCount { get; set; }
        public virtual DbSet<Tbl_CampaignWithdrawRequest> Tbl_CampaignWithdrawRequest { get; set; }
        public virtual DbSet<tbl_CmpnBenBankDetails> tbl_CmpnBenBankDetails { get; set; }
        public virtual DbSet<tbl_DistanceCount> tbl_DistanceCount { get; set; }
        public virtual DbSet<Tbl_WithdrawRequest_History> Tbl_WithdrawRequest_History { get; set; }
        public virtual DbSet<Tbl_WithdrawRequestHistory> Tbl_WithdrawRequestHistory { get; set; }
        public virtual DbSet<tbl_lookup> tbl_lookup { get; set; }
        public virtual DbSet<tbl_CmpnBenBankDetails1> tbl_CmpnBenBankDetails1 { get; set; }
        public virtual DbSet<Tbl_Loan> Tbl_Loan { get; set; }
        public virtual DbSet<Tbl_LoanBenDetails> Tbl_LoanBenDetails { get; set; }
        public virtual DbSet<Tbl_LoanCityDetails> Tbl_LoanCityDetails { get; set; }
        public virtual DbSet<Tbl_LoanDescription> Tbl_LoanDescription { get; set; }
        public virtual DbSet<tbl_LoanBankDetails> tbl_LoanBankDetails { get; set; }
        public virtual DbSet<Tbl_LoanComment> Tbl_LoanComment { get; set; }
        public virtual DbSet<Tbl_LoanDonation> Tbl_LoanDonation { get; set; }
        public virtual DbSet<Tbl_LoanLike> Tbl_LoanLike { get; set; }
        public virtual DbSet<Tbl_LoanShares> Tbl_LoanShares { get; set; }
        public virtual DbSet<Tbl_LoanUpdatesAttachment> Tbl_LoanUpdatesAttachment { get; set; }
        public virtual DbSet<Tbl_LoanWithdrawRequest> Tbl_LoanWithdrawRequest { get; set; }
        public virtual DbSet<Tbl_LoanWithdrawRequestHistory> Tbl_LoanWithdrawRequestHistory { get; set; }
    
        public virtual int GetnearestCities(string lAT, string lONG)
        {
            var lATParameter = lAT != null ?
                new ObjectParameter("LAT", lAT) :
                new ObjectParameter("LAT", typeof(string));
    
            var lONGParameter = lONG != null ?
                new ObjectParameter("LONG", lONG) :
                new ObjectParameter("LONG", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetnearestCities", lATParameter, lONGParameter);
        }
    
        public virtual int GetTopCampaigns()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetTopCampaigns");
        }
    
        public virtual int spSearchStringInTable(string searchString, string table_Schema, string table_Name)
        {
            var searchStringParameter = searchString != null ?
                new ObjectParameter("SearchString", searchString) :
                new ObjectParameter("SearchString", typeof(string));
    
            var table_SchemaParameter = table_Schema != null ?
                new ObjectParameter("Table_Schema", table_Schema) :
                new ObjectParameter("Table_Schema", typeof(string));
    
            var table_NameParameter = table_Name != null ?
                new ObjectParameter("Table_Name", table_Name) :
                new ObjectParameter("Table_Name", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spSearchStringInTable", searchStringParameter, table_SchemaParameter, table_NameParameter);
        }
    
        public virtual int GetSearchCampaigns(string searchString)
        {
            var searchStringParameter = searchString != null ?
                new ObjectParameter("SearchString", searchString) :
                new ObjectParameter("SearchString", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetSearchCampaigns", searchStringParameter);
        }
    
        public virtual int GetCampaignsByComment()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCampaignsByComment");
        }
    
        public virtual int GetCampaignsByDistance(string lAT, string lONG)
        {
            var lATParameter = lAT != null ?
                new ObjectParameter("LAT", lAT) :
                new ObjectParameter("LAT", typeof(string));
    
            var lONGParameter = lONG != null ?
                new ObjectParameter("LONG", lONG) :
                new ObjectParameter("LONG", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCampaignsByDistance", lATParameter, lONGParameter);
        }
    
        public virtual int GetCampaignsByDonors()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCampaignsByDonors");
        }
    
        public virtual int GetCampaignsByEndorse()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCampaignsByEndorse");
        }
    
        public virtual int GetCampaignsByLikes()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCampaignsByLikes");
        }
    
        public virtual int GetCampaignsBydntion()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCampaignsBydntion");
        }
    
        public virtual int GetCampaignsByPlace()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCampaignsByPlace");
        }
    
        public virtual int GetCampaignsBydntion1()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCampaignsBydntion1");
        }
    
        public virtual int DeactivateCampaigns()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeactivateCampaigns");
        }
    
        public virtual ObjectResult<spRes_getcampaigns> GetCampaignsTest()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spRes_getcampaigns>("GetCampaignsTest");
        }
    
        public virtual ObjectResult<spres_get1cam> GetCampaigns1Test()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spres_get1cam>("GetCampaigns1Test");
        }
    
        public virtual ObjectResult<GetCampaigns2Test_Result> GetCampaigns2Test()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCampaigns2Test_Result>("GetCampaigns2Test");
        }
    
        public virtual int GetCampaignSummary(string listOfIDs)
        {
            var listOfIDsParameter = listOfIDs != null ?
                new ObjectParameter("listOfIDs", listOfIDs) :
                new ObjectParameter("listOfIDs", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCampaignSummary", listOfIDsParameter);
        }
    
        public virtual ObjectResult<spres_getsummary> GetCampaignSummary1(string listOfIDs)
        {
            var listOfIDsParameter = listOfIDs != null ?
                new ObjectParameter("listOfIDs", listOfIDs) :
                new ObjectParameter("listOfIDs", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spres_getsummary>("GetCampaignSummary1", listOfIDsParameter);
        }
    
        public virtual ObjectResult<spres_getusercampaigns> GetUserCampaignSummary(string listOfIDs)
        {
            var listOfIDsParameter = listOfIDs != null ?
                new ObjectParameter("listOfIDs", listOfIDs) :
                new ObjectParameter("listOfIDs", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spres_getusercampaigns>("GetUserCampaignSummary", listOfIDsParameter);
        }
    
        public virtual ObjectResult<spres_getusercampaigns> GetUserCampaignSummary1(string listOfIDs)
        {
            var listOfIDsParameter = listOfIDs != null ?
                new ObjectParameter("listOfIDs", listOfIDs) :
                new ObjectParameter("listOfIDs", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spres_getusercampaigns>("GetUserCampaignSummary1", listOfIDsParameter);
        }
    
        public virtual ObjectResult<spres_getsearchRes> GetSearchCampaignsResults(string searchString, Nullable<int> skip, Nullable<int> take)
        {
            var searchStringParameter = searchString != null ?
                new ObjectParameter("SearchString", searchString) :
                new ObjectParameter("SearchString", typeof(string));
    
            var skipParameter = skip.HasValue ?
                new ObjectParameter("Skip", skip) :
                new ObjectParameter("Skip", typeof(int));
    
            var takeParameter = take.HasValue ?
                new ObjectParameter("Take", take) :
                new ObjectParameter("Take", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spres_getsearchRes>("GetSearchCampaignsResults", searchStringParameter, skipParameter, takeParameter);
        }
    
        public virtual ObjectResult<spres_getsearchRes> GetSearchCampaignsResults1(string searchString, Nullable<int> skip, Nullable<int> take)
        {
            var searchStringParameter = searchString != null ?
                new ObjectParameter("SearchString", searchString) :
                new ObjectParameter("SearchString", typeof(string));
    
            var skipParameter = skip.HasValue ?
                new ObjectParameter("Skip", skip) :
                new ObjectParameter("Skip", typeof(int));
    
            var takeParameter = take.HasValue ?
                new ObjectParameter("Take", take) :
                new ObjectParameter("Take", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spres_getsearchRes>("GetSearchCampaignsResults1", searchStringParameter, skipParameter, takeParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> GetSearchResultsCount(string searchString)
        {
            var searchStringParameter = searchString != null ?
                new ObjectParameter("SearchString", searchString) :
                new ObjectParameter("SearchString", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetSearchResultsCount", searchStringParameter);
        }
    
        public virtual int DeleteACampaign(Nullable<int> campaignID)
        {
            var campaignIDParameter = campaignID.HasValue ?
                new ObjectParameter("CampaignID", campaignID) :
                new ObjectParameter("CampaignID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteACampaign", campaignIDParameter);
        }
    
        public virtual int DeleteInactiveCampaign()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteInactiveCampaign");
        }
    }
}
