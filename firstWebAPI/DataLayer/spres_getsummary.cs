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
    
    public partial class spres_getsummary
    {
        public int Id { get; set; }
        public int Category { get; set; }
        public bool IsApprovedbyAdmin { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string MoneyType { get; set; }
        public bool Status { get; set; }
        public Nullable<decimal> TargetAmount { get; set; }
        public Nullable<System.DateTime> TargetDate { get; set; }
        public int CountUsage { get; set; }
        public string CreatedUserName { get; set; }
        public Nullable<int> beneficiaryId { get; set; }
        public string DPPath { get; set; }
        public Nullable<int> BResidence { get; set; }
        public string CityName { get; set; }
        public string Latitude { get; set; }
        public string longitude { get; set; }
        public string DPName { get; set; }
        public Nullable<int> DescId { get; set; }
        public string storyDescription { get; set; }
        public Nullable<int> DonationId { get; set; }
        public string DonatedBy { get; set; }
        public Nullable<System.DateTime> DonatedOn { get; set; }
        public Nullable<bool> isAnanymous { get; set; }
        public Nullable<decimal> DonationAmnt { get; set; }
        public Nullable<bool> isPaid { get; set; }
        public string UserDPImage { get; set; }
        public string Name { get; set; }
    }
}
