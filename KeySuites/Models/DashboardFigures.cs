namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DashboardFigures
    {
        public Int16 LeadCountCurrentYear { get; set; }

        public Int16 LeaseCountCurrentYear { get; set; }

        public Int16 QuoteCountCurrentYear { get; set; }

        public decimal LeasedCurrentYear { get; set; }

        public decimal Leased { get; set; }

        public decimal Difference { get; set; }

        public decimal LeasedPrevMonth { get; set; }

        public Int64 LeadCount { get; set; }

        public Int64 QuoteCount { get; set; }
        
    }
}
