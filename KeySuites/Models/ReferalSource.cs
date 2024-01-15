namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ReferalSource
    {
        [Column(TypeName = "numeric")]
        public Int64 ReferalSourceId { get; set; }

        [StringLength(500)]
        public string CompanyName { get; set; }

        [StringLength(500)]
        public string Address { get; set; }


        [StringLength(500)]
        public string State { get; set; }

        [StringLength(500)]
        public string Address2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Zip { get; set; }

        [StringLength(500)]
        public string City { get; set; }


        [StringLength(50)]
        public string ReferalType { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? Number { get; set; }

        public decimal? CostPerDay { get; set; }

        public bool IsActive
        { get; set; }
    }
}
