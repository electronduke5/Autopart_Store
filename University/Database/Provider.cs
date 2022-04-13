using System;
using System.Collections.Generic;

#nullable disable

namespace University.Database
{
    public partial class Provider
    {
        public Provider()
        {
            Autoparts = new HashSet<Autopart>();
        }

        public int IdProvider { get; set; }
        public string ProviderName { get; set; }

        public virtual ICollection<Autopart> Autoparts { get; set; }
    }
}
