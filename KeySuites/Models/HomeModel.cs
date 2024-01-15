using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vidly.Models
{
    public class HomeModel
    {
        public IList<string> SelectedVendorTypes { get; set; }
        public IList<SelectListItem> AvailableVendorTypes { get; set; }

        public HomeModel()
        {
            SelectedVendorTypes = new List<string>();
            AvailableVendorTypes = new List<SelectListItem>();
        }
    }
}