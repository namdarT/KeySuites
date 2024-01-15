using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Vidly.Models
{
    public partial class Login 
    {
        public string UserName
        {
            get;
            set;
        }

        
        public string PasswordHash
        {
            get;
            set;
        }
    }
}