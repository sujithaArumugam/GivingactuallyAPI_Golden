//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Tbl_Campaign
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tbl_Campaign()
        {
            this.Tbl_Endorse = new HashSet<Tbl_Endorse>();
            this.Tbl_Like = new HashSet<Tbl_Like>();
            this.Tbl_ParentComment = new HashSet<Tbl_ParentComment>();
            this.Tbl_Shares = new HashSet<Tbl_Shares>();
            this.Tbl_UpdatesAttachment = new HashSet<Tbl_UpdatesAttachment>();
            this.Tbl_CampaignDonation = new HashSet<Tbl_CampaignDonation>();
            this.Tbl_StoriesAttachment = new HashSet<Tbl_StoriesAttachment>();
            this.Tbl_BeneficiaryDetails = new HashSet<Tbl_BeneficiaryDetails>();
            this.Tbl_CampaignDescription = new HashSet<Tbl_CampaignDescription>();
            this.Tbl_CampaignDescriptionUpdates = new HashSet<Tbl_CampaignDescriptionUpdates>();
            this.Tbl_CampaignWithdrawRequest = new HashSet<Tbl_CampaignWithdrawRequest>();
            this.tbl_CmpnBenBankDetails = new HashSet<tbl_CmpnBenBankDetails>();
            this.Tbl_WithdrawRequest_History = new HashSet<Tbl_WithdrawRequest_History>();
            this.Tbl_WithdrawRequestHistory = new HashSet<Tbl_WithdrawRequestHistory>();
        }
    
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsApprovedbyAdmin { get; set; }
        public string Title { get; set; }
        public bool Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public int Category { get; set; }
        public string Story_Expense { get; set; }
        public string storyDescription { get; set; }
        public string MoneyType { get; set; }
        public Nullable<decimal> TargetAmount { get; set; }
        public Nullable<int> BeneficiaryType { get; set; }
        public string BName { get; set; }
        public string BGroupName { get; set; }
        public string NGOName { get; set; }
        public int CountUsage { get; set; }
        public Nullable<System.DateTime> TargetDate { get; set; }
        public string CreatedUserName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_Endorse> Tbl_Endorse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_Like> Tbl_Like { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_ParentComment> Tbl_ParentComment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_Shares> Tbl_Shares { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_UpdatesAttachment> Tbl_UpdatesAttachment { get; set; }
        public virtual Tbl_User Tbl_User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_CampaignDonation> Tbl_CampaignDonation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_StoriesAttachment> Tbl_StoriesAttachment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_BeneficiaryDetails> Tbl_BeneficiaryDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_CampaignDescription> Tbl_CampaignDescription { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_CampaignDescriptionUpdates> Tbl_CampaignDescriptionUpdates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_CampaignWithdrawRequest> Tbl_CampaignWithdrawRequest { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_CmpnBenBankDetails> tbl_CmpnBenBankDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_WithdrawRequest_History> Tbl_WithdrawRequest_History { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_WithdrawRequestHistory> Tbl_WithdrawRequestHistory { get; set; }
    }
}
