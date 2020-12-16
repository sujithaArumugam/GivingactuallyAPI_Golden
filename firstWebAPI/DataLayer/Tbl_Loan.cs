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
    
    public partial class Tbl_Loan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsApprovedbyAdmin { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public int Category { get; set; }
        public string storyDescription { get; set; }
        public string MoneyType { get; set; }
        public Nullable<decimal> TargetAmount { get; set; }
        public Nullable<int> BeneficiaryType { get; set; }
        public string BName { get; set; }
        public string BGroupName { get; set; }
        public string NGOName { get; set; }
        public Nullable<System.DateTime> TargetDate { get; set; }
        public Nullable<decimal> RePaymentInterestPerc { get; set; }
        public string RePaymentTerm { get; set; }
        public Nullable<System.DateTime> RePaymentStartDate { get; set; }
        public Nullable<System.DateTime> RePaymentEndDate { get; set; }
        public int CountUsage { get; set; }
        public int FieldPartner { get; set; }
        public string CreatedUserName { get; set; }
        public Nullable<int> RePaymentTermDays { get; set; }
    }
}
