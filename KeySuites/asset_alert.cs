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
    
    public partial class asset_alert
    {
        public decimal asset_alert_id { get; set; }
        public Nullable<decimal> asset_alert_condition_id { get; set; }
        public Nullable<decimal> json_data_id { get; set; }
        public string compare_value { get; set; }
        public Nullable<System.DateTime> reading_dt { get; set; }
        public System.DateTime create_dt { get; set; }
        public bool active_ind { get; set; }
        public Nullable<System.DateTime> logical_termination_dt { get; set; }
        public string last_update_user_id { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public byte[] row_version { get; set; }
    }
}
