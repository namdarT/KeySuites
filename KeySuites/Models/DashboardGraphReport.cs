namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DashboardGraphReport
    {
        public Int64 Count { get; set; }

        public Int64 Month { get; set; }

        public string MonthName { get; set; }
    }
}
