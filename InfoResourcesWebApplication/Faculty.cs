using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace InfoResourcesWebApplication
{
    [Table("Faculties")]
    public partial class Faculty
    {
        public Faculty()
        {
            Departments = new HashSet<Department>();
        }

        public int FacultyId { get; set; }
        public string FacultyName { get; set; }

        public virtual ICollection<Department> Departments { get; set; }
    }
}
