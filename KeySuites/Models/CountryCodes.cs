using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Vidly.Models
{
    
    public partial class CountryCodes
    {
        
        public string id
        {
            get;
            set;
        }
        public string name
        {
            get;
            set;
        }
    }
}