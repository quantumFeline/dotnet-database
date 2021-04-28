using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Назва кафедри")]
        public string DepartmentName { get; set; }
        [Display(Name = "Факультет")]
        public int? Faculty { get; set; }

        [ForeignKey("Faculty")]
        public virtual Faculty FacultyNavigation { get; set; }
        public virtual ICollection<Author> Authors { get; set; }
    }
}
