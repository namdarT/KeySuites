//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vidly
{
    using System;
    using System.Collections.Generic;
    
    public partial class device_attribute_data
    {
        public decimal device_id { get; set; }
        public Nullable<decimal> asset_attribute_data_id { get; set; }
        public Nullable<decimal> asset_id { get; set; }
        public string deviceSN { get; set; }
        public Nullable<double> batteryStrength { get; set; }
        public Nullable<double> signalStrength { get; set; }
        public Nullable<double> batteryTypeID { get; set; }
        public System.DateTime reading_dt { get; set; }
        public byte[] row_version { get; set; }
    }
}