using System;
using System.Collections.Generic;

#nullable disable

namespace University.Database
{
    public partial class Order
    {
        public Order()
        {
            CombinationOrders = new HashSet<CombinationOrder>();
        }

        public int IdOrders { get; set; }
        public DateTime OrdersDate { get; set; }
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<CombinationOrder> CombinationOrders { get; set; }
    }
}
