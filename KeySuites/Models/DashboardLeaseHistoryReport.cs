namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DashboardLeaseHistoryReport
    {
        public decimal Charges { get; set; }

        public string OcupantName { get; set; }

        public string Name { get; set; }

    }
}
