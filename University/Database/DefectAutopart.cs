using System;
using System.Collections.Generic;

#nullable disable

namespace University.Database
{
    public partial class DefectAutopart
    {
        public int IdDefect { get; set; }
        public int Count { get; set; }
        public int AutopartId { get; set; }

        public virtual Autopart Autopart { get; set; }
    }
}
