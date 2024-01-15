namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LeasedList
    {
        [Key]
        [Column(TypeName = "numeric")]
        public Int64 RId { get; set; }

        [Column(TypeName = "numeric")]
        public Int64? QouteId { get; set; }

        [Column(TypeName = "numeric")]
        public Int64? PropertyId { get; set; }

        [StringLength(1000)]
        public string PropertyDescription { get; set; }

        public decimal? AdminFee { get; set; }

        public decimal? ApplicationFee { get; set; }

        public decimal? CleaningFee { get; set; }

        public decimal? PetFee { get; set; }

        public DateTime? LeaseStartDate { get; set; }

        public DateTime? LeaseEndDate { get; set; }

        public DateTime? CheckInTime { get; set; }

        [StringLength(1000)]
        public string ArrivalInstructions { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [StringLength(1000)]
        public string DepartureInstructions { get; set; }

        public decimal? TotalOneTime { get; set; }

        public decimal? TotalMonthly { get; set; }

        public string ContactNumber { get; set; }

        [StringLength(500)]
        public string ContactEmail { get; set; }

        [StringLength(500)]
        public string OcupantName { get; set; }
    }
}
