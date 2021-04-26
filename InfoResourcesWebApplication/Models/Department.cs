using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace InfoResourcesWebApplication
{
    [Table("Departments")]
    public partial class Department
    {
        public Department()
        {
            Authors = new HashSet<Author>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? Faculty { get; set; }

        public virtual Faculty FacultyNavigation { get; set; }
        public virtual ICollection<Author> Authors { get; set; }
    }
}
