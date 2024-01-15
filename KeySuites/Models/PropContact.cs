namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PropContact
    {
       

        public Int64 PropContactId { get; set; }
        public Int64 PropertyId { get; set; }
        [Required]
        [StringLength(500)]
        public string PropContactEmail { get; set; }

        
        [StringLength(500)]
        public string PropContactLastName { get; set; }

        [StringLength(500)]
        public string PropContactFirstName { get; set; }

        [StringLength(500)]
        public string PropContactNumber { get; set; }

       
    }
}
