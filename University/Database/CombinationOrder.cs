using System;
using System.Collections.Generic;

#nullable disable

namespace University.Database
{
    public partial class CombinationOrder
    {
        public int IdCombination { get; set; }
        public int Count { get; set; }
        public int AutopartId { get; set; }
        public int OrdersId { get; set; }

        public virtual Autopart Autopart { get; set; }
        public virtual Order Orders { get; set; }
    }
}
