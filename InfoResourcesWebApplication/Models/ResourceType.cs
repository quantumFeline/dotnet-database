using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace InfoResourcesWebApplication
{
    [Table("ResourceTypes")]
    public partial class ResourceType
    {
        public ResourceType()
        {
            Resources = new HashSet<Resource>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResourceTypeId { get; set; }
        [Display(Name = "Назва")]
        public string ResourceTypeName { get; set; }
        [Display(Name = "Опис")]
        public string ResourceTypeDescription { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }
    }
}
