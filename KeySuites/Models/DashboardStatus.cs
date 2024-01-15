using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vidly.Models
{
    public partial class DashboardStatus
    {

        public int totalAsset
        {
            get;
            set;
        }
        public int totalInactive
        {
            get;
            set;
        }
        public int totalOK
        {
            get;
            set;
        }
        public int totalFail
        {
            get;
            set;
        }

        public int totalWarning
        {
            get;
            set;
        }
        //public ICollection<AssetAlertCondition> AssetAlertConditions { get; set; }
        //public ICollection<VwAppUserAsset> VwAppUserAssets { get; set; }
        //public ICollection<AssetAlert> AssetAlerts { get; set; }
        //public ICollection<asset> assets { get; set; }
    }
}