namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class States
    {
        
        public string Code { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

    }
}
