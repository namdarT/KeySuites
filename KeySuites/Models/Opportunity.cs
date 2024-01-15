namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Opportunity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Opportunity()
        {
            Tasks = new HashSet<Task>();
        }

        public int Id { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }

        public int ContactId { get; set; }

        public int StatusId { get; set; }

        public DateTime CloseDate { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual OpportunityStatus OpportunityStatus { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
