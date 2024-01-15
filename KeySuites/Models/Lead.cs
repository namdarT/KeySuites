namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Lead
    {
        [Key]
        [Column(TypeName = "numeric")]
        public Int64 LeadsId { get; set; }

        public string ContactType { get; set; }

        [StringLength(500)]
        public string LeadsName { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ContactInfoId { get; set; }

        [StringLength(500)]

        public string ContactNumber  { get; set; }
        
        [StringLength(500)]
        public string ContactEmail  { get; set; }

        [StringLength(500)]
        public string OcupantName  { get; set; }
        public string NoOfAdults { get; set; }

        public string NoOfChildren { get; set; }

        public string NoOfPets { get; set; }

        [StringLength(50)]
        public string Breed { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(500)]
        public string PreferedArea { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(500)]
        public string City { get; set; }

        [StringLength(500)]
        public string State { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Zip { get; set; }

        [StringLength(500)]
        public string Address2 { get; set; }

        [StringLength(500)]
        public string OccupantCity { get; set; }

        [StringLength(500)]
        public string OccupantState { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? OccupantZip { get; set; }

        public string NoOfBedRooms { get; set; }

        [Column(TypeName = "date")]
        public DateTime? MoveInDate { get; set; }

        [StringLength(500)]
        public string LeaseTerm { get; set; }

        [StringLength(500)]
        public string FloorPreference { get; set; }

        [StringLength(500)]
        public string ReferelSource { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [StringLength(500)]
        public string CompanyLogo { get; set; }

        [StringLength(500)]
        public string CompanyContactNumber { get; set; }


        [StringLength(500)]
        public string ContactNumberCompany { get; set; }


        [StringLength(500)]
        public string ContactName { get; set; }

        [StringLength(500)]
        public string PreferedAddress { get; set; }


        [StringLength(500)]
        public string Elevator { get; set; }
        public bool IsActive { get; set; }

        public ReferalSource referalSource { get; set; }

    }
}
