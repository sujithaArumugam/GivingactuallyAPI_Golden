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
    
    public partial class Tbl_LoanBenDetails
    {
        public int Id { get; set; }
        public string BPincode { get; set; }
        public string BCountry { get; set; }
        public Nullable<int> BResidence { get; set; }
        public byte[] DP { get; set; }
        public string DPName { get; set; }
        public int StoryId { get; set; }
        public bool Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public int Category { get; set; }
        public string DPPath { get; set; }
    }
}
