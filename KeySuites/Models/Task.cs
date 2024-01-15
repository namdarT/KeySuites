namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Task
    {
        public int Id { get; set; }

        [StringLength(256)]
        public string Title { get; set; }

        public int OpportunityId { get; set; }

        public DateTime DueDate { get; set; }

        public int TypeId { get; set; }

        public int? StatusId { get; set; }

        public virtual Opportunity Opportunity { get; set; }

        public virtual TaskStatus TaskStatus { get; set; }

        public virtual TaskType TaskType { get; set; }
    }
}
