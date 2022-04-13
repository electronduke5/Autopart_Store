using System;
using System.Collections.Generic;

#nullable disable

namespace University.Database
{
    public partial class Autopart
    {
        public Autopart()
        {
            CombinationOrders = new HashSet<CombinationOrder>();
            DefectAutoparts = new HashSet<DefectAutopart>();
        }

        public int IdAutopart { get; set; }
        public string AutopartName { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public int CategoryId { get; set; }
        public int ProviderId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual ICollection<CombinationOrder> CombinationOrders { get; set; }
        public virtual ICollection<DefectAutopart> DefectAutoparts { get; set; }
    }
}
