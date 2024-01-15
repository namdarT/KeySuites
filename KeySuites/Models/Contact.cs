namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Contact
    {
       

        public Int64 Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Email { get; set; }

        [StringLength(500)]
        public string Company { get; set; }

        [StringLength(500)]
        public string LastName { get; set; }

        [StringLength(500)]
        public string FirstName { get; set; }

        [StringLength(500)]
        public string Phone { get; set; }

        public Int64 CompanyId { get; set; }
        [StringLength(500)]
        public string Address { get; set; }

        public string Address2 { get; set; }


        public string City { get; set; }

        public string State { get; set; }
        [Column(TypeName = "numeric")]
        public Int64? Zip { get; set; }

        public bool IsActive { get; set; }

        public DateTime DOB { get; set; }


    }
}
