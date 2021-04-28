using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InfoResourcesWebApplication;

namespace InfoResourcesWebApplication.Data
{
    public class InfoResourcesWebApplicationContext : DbContext
    {
        public InfoResourcesWebApplicationContext (DbContextOptions<InfoResourcesWebApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<InfoResourcesWebApplication.Resource> Resource { get; set; }

        public DbSet<InfoResourcesWebApplication.Author> Author { get; set; }

        public DbSet<InfoResourcesWebApplication.Faculty> Faculty { get; set; }

        public DbSet<InfoResourcesWebApplication.Department> Department { get; set; }

        public DbSet<InfoResourcesWebApplication.ResourceType> ResourceType { get; set; }

        public DbSet<InfoResourcesWebApplication.Subject> Subject { get; set; }
    }
}
