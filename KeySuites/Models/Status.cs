namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Status
    {
        [Column(TypeName = "numeric")]
        public Int64 StatusId { get; set; }

        [StringLength(500)]
        public string Statuses { get; set; }

        [StringLength(500)]
        public string StatusType { get; set; }

        [StringLength(500)]
        public string Detail { get; set; }

        public bool IsActive
        { get; set; }
    }
}
