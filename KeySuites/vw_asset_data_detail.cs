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
    
    public partial class vw_asset_data_detail
    {
        public decimal data_detail_id { get; set; }
        public decimal asset_id { get; set; }
        public decimal json_data_id { get; set; }
        public string data_tag { get; set; }
        public string data_key_cd { get; set; }
        public string data_value { get; set; }
        public string data_type_cd { get; set; }
        public Nullable<System.DateTime> logical_termination_dt { get; set; }
        public string last_update_user_id { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public byte[] row_version { get; set; }
    }
}
