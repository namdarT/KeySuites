namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VendContact
    {
       

        public Int64 VendContactId { get; set; }
        public Int64 VendorId { get; set; }
        [Required]
        [StringLength(500)]
        public string VendContactEmail { get; set; }

        
        [StringLength(500)]
        public string VendContactLastName { get; set; }

        [StringLength(500)]
        public string VendContactFirstName { get; set; }

        [StringLength(500)]
        public string VendContactNumber { get; set; }

       
    }
}
