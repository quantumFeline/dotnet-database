using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace InfoResourcesWebApplication
{
    [Table("Subjects")]
    public partial class Subject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
    }
}
