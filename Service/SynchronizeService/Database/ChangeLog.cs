//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SynchronizeService.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChangeLog
    {
        public int ID { get; set; }
        public int ChangeTypeID { get; set; }
        public long WebServiceID { get; set; }
        public int CenterID { get; set; }
        public string PhoneNo { get; set; }
        public System.DateTime LogDate { get; set; }
        public Nullable<System.DateTime> SetupDate { get; set; }
        public Nullable<int> SubscriberTypeID { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Nullable<double> LocalCounter { get; set; }
        public Nullable<int> DomecticCounter { get; set; }
        public Nullable<int> InternationalCounter { get; set; }
        public string OldPhoneNo { get; set; }
        public Nullable<int> OldLocalCounter { get; set; }
        public Nullable<int> OldDomecticCounter { get; set; }
        public Nullable<int> OldInternationalCounter { get; set; }
        public Nullable<long> Trust { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsApplied { get; set; }
        public bool IsAutomaticLog { get; set; }
        public bool IsFoxProExport { get; set; }
        public string Elka_FI_CODE { get; set; }
        public Nullable<bool> IsAppied118 { get; set; }
        public Nullable<bool> ViewIn118 { get; set; }
    }
}