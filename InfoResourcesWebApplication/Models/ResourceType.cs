﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace InfoResourcesWebApplication
{
    public partial class ResourceType
    {
        public ResourceType()
        {
            Resources = new HashSet<Resource>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        public string ResourceTypeDescription { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }
    }
}