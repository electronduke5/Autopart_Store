using System;
using System.Collections.Generic;

#nullable disable

namespace University.Database
{
    public partial class Employee
    {
        public int IdEmployee { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}
