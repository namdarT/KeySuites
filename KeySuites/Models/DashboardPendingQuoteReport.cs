namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DashboardPendingQuoteReport
    {
        public DateTime CreatedDate { get; set; }

        public string OcupantName { get; set; }

        public string SpentDays { get; set; }

        public Int64 LeadsId { get; set; }

        public string Name { get; set; }

        public string ReferelSource { get; set; }

    }
}
