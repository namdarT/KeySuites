namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Company
    {
        
        [Required]
        [StringLength(500)]
        public string CompanyEmail { get; set; }

        [StringLength(500)]
        public string CompanyName { get; set; }

        
        [StringLength(500)]
        public string Website { get; set; }

        [StringLength(500)]
        public string CompanyContact { get; set; }

        public Int64 CompanyId { get; set; }
        [StringLength(500)]
        public string Address { get; set; }

        public string Address2 { get; set; }

        public string PreferedArea { get; set; }

        public string City { get; set; }

        public string State { get; set; }
        [Column(TypeName = "numeric")]
        public decimal? Zip { get; set; }
        public bool IsActive { get; set; }

        
    }
}
