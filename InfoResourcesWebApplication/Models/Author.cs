using System;
using System.Collections.Generic;
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public int? Department { get; set; }

        public virtual Department DepartmentNavigation { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
    }
}
