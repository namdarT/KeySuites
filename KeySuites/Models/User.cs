namespace Vidly.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
       

        public Int64 Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Email { get; set; }

        [StringLength(500)]
        public string Phone { get; set; }

        public string Password { get; set; }

        public string PasswordHash { get; set; }

        public string UserName { get; set; }

        [StringLength(500)]
        public string UserType { get; set; }

        public string Department { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsActive { get; set; }

       
    }
}
