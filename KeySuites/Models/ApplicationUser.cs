using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Identity;

namespace Vidly.Models
{
    public class ApplicationUser 
    {
        public Int64 Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

    }
}