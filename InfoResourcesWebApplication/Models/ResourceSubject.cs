using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace InfoResourcesWebApplication
{
    [Table("ResourceSubjects")]
    public partial class ResourceSubject
    {
        public int SubjectId { get; set; }
        public int ResourceId { get; set; }

        public virtual Resource Resource { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
