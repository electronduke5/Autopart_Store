using System;
using System.Collections.Generic;

#nullable disable

namespace University.Database
{
    public partial class Accounting
    {
        public int IdAccounting { get; set; }
        public decimal? Profit { get; set; }
        public decimal? Expenditure { get; set; }
        public DateTime? RecordDate { get; set; }
    }
}
