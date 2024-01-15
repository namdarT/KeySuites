namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vendor
    {
        [Column(TypeName = "numeric")]
        public Int64 VendorId { get; set; }

        [StringLength(500)]
        public string CompanyName { get; set; }

        [StringLength(500)]
        public string VendorType { get; set; }

        [StringLength(500)]
        public string Address { get; set; }


        [StringLength(500)]
        public string Address2 { get; set; }


        [StringLength(500)]
        public string PhoneNumber { get; set; }

        [StringLength(500)]
        
        public string Website { get; set; }

        [StringLength(500)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public decimal? DollarAmount { get; set; }
        public decimal? PercentageAmount { get; set; }

        [StringLength(500)]
        public string Zip { get; set; }

        [StringLength(500)]
        public string State { get; set; }

        [StringLength(500)]
        public string City { get; set; }

        [StringLength(5000)]
        public string Street { get; set; }

        public bool IsActive
        { get; set; }
    }
}
