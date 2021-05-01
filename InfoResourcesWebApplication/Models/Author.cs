using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace InfoResourcesWebApplication
{

    [Table("Authors")]
    public partial class Author
    {
        public Author()
        {
            Resources = new HashSet<Resource>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthorId { get; set; }
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; }
        [Display(Name = "Прізвище")]
        public string LastName { get; set; }
        [Display(Name = "По-батькові")]
        public string Patronymic { get; set; }
        [Display(Name = "Кафедра")]
        public int? Department { get; set; }

        [ForeignKey("Department")]
        public virtual Department DepartmentNavigation { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }

        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName + " " + Patronymic;
            }
        }
    }
}
