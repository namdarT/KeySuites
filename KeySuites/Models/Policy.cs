namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Policy
    {
        [Column(TypeName = "numeric")]
        public decimal PolicyId { get; set; }

        [Column("Policy")]
        [StringLength(8000)]
        public string Policy1 { get; set; }
    }
}
