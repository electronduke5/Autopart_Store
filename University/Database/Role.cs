using System;
using System.Collections.Generic;

#nullable disable

namespace University.Database
{
    public partial class Role
    {
        public Role()
        {
            Employees = new HashSet<Employee>();
        }

        public int IdRole { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
