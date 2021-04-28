using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace InfoResourcesWebApplication
{
    [Table("Subjects")]
    public partial class Subject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubjectId { get; set; }
        [Display(Name = "Назва предмету")]
        public string SubjectName { get; set; }
        [Display(Name = "Опис")]
        public string Description { get; set; }
    }
}
