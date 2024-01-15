namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ContactInfo")]
    public partial class ContactInfo
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ContactInfoId { get; set; }

        public bool? Company_Individual { get; set; }

        [StringLength(500)]
        public string ContactName { get; set; }

        [StringLength(500)]
        public string CompanyName { get; set; }

        public string OccupantName { get; set; }

        [StringLength(500)]
        public string Email { get; set; }

        [StringLength(500)]
        public string WebSite { get; set; }

        [StringLength(500)]
        public string CompanyAddress { get; set; }

        [StringLength(500)]
        public string CompanyPhone { get; set; }

        [StringLength(500)]
        public string ContactAddress { get; set; }

        [StringLength(500)]
        public string ContactPhone { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ContactBirthDay { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? CreatedBy { get; set; }

        [StringLength(500)]
        public string CompanyType { get; set; }
    }
}
