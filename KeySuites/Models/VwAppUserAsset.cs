using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vidly.Models
{
    [Table("vw_app_user_asset", Schema = "dbo")]
    public partial class VwAppUserAsset
    {
        [Key]
        public decimal asset_id
        {
            get;
            set;
        }
        //public List<VwAppUserAsset> VwAppUserAssetList;
        public decimal manufacturer_id
        {
            get;
            set;
        }

        [Required]
        public string asset_name
        {
            get;
            set;
        }

        public string asset_group
        {
            get;
            set;
        }

        public string asset_type_cd
        {
            get;
            set;
        }
        public string asset_description
        {
            get;
            set;
        }
        public string external_id
        {
            get;
            set;
        }
        public string temperature
        {
            get;
            set;
        }
        public string deviceSN
        {
            get;
            set;
        }
        public string folder_Id
        {
            get;
            set;
        }

        public decimal waterLevel
        {
            get;
            set;
        }
        public double batteryStrength
        {
            get;
            set;
        }
        public double signalStrength
        {
            get;
            set;
        }
        public double batteryTypeID
        {
            get;
            set;
        }
        public string street_address_1
        {
            get;
            set;
        }
        public string street_address_2
        {
            get;
            set;
        }
        public string city
        {
            get;
            set;
        }
        public string zip
        {
            get;
            set;
        }
        public string state_cd
        {
            get;
            set;
        }
        public string country
        {
            get;
            set;
        }
        public string latitude
        {
            get;
            set;
        }
        public string longitude
        {
            get;
            set;
        }
        public bool active_ind
        {
            get;
            set;
        }

        public bool activate
        {
            get;
            set;
        }
        public string activated_by
        {
            get;
            set;
        }
        public DateTime? activated_dt
        {
            get;
            set;
        }
        public DateTime asset_create_dt
        {
            get;
            set;
        }
        public string asset_created_by
        {
            get;
            set;
        }
        public DateTime? logical_termination_dt
        {
            get;
            set;
        }
        public string last_update_user_id
        {
            get;
            set;
        }
        public DateTime last_update_dt
        {
            get;
            set;
        }
        [Timestamp]
        public Byte[] row_version
        {
            get;
            set;
        }
        public string model_number
        {
            get;
            set;
        }
        public string user_defined_group
        {
            get;
            set;
        }
        public string installation_type_cd
        {
            get;
            set;
        }
        public decimal? wet_water_level
        {
            get;
            set;
        }
        public decimal? wet_sensor_depth
        {
            get;
            set;
        }
        public decimal? dry_above_grade_offset
        {
            get;
            set;
        }
        public decimal? dry_below_grade_offset
        {
            get;
            set;
        }
        public bool disable_alarm_ind
        {
            get;
            set;
        }
        public string location_type
        {
            get;
            set;
        }
        public string username
        {
            get;
            set;
        }
        public string display_name
        {
            get;
            set;
        }
    }
}