using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vidly.Models
{
    public class MyListClass
    {
        public string MyListName { get; set; } = "My List Name";

        //next line will show your items, but will not have changes to the items
        //returned because it's not a property aka with getter and setter.
        //public List<CheckBoxItem> Items = new List<CheckBoxItem>;
    }

}