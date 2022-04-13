using System;
using System.Collections.Generic;

#nullable disable

namespace University.Database
{
    public partial class Category
    {
        public Category()
        {
            Autoparts = new HashSet<Autopart>();
        }

        public int IdCategory { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<Autopart> Autoparts { get; set; }
    }
}
