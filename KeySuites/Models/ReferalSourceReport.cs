namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ReferalSourceReport
    {
        [Column(TypeName = "numeric")]
        public Int64 ReferalSourceId { get; set; }

        [StringLength(500)]
        public string CompanyName { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(50)]
        public string ReferalType { get; set; }

        [Column(TypeName = "decimal")]
        public string Number { get; set; }

        [Column(TypeName = "decimal")]
        public decimal NoOfReservation { get; set; }

        [Column(TypeName = "decimal")]
        public decimal ShareAmount { get; set; }

        [Column(TypeName = "decimal")]
        public decimal PropertiesRent { get; set; }

        [Column(TypeName = "decimal")]
        public decimal TotalFinalAmount { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

    }
}
