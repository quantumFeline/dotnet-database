using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace InfoResourcesWebApplication
{
    public partial class DBInfoResourcesContext : DbContext
    {
        public DBInfoResourcesContext()
        {
        }

        public DBInfoResourcesContext(DbContextOptions<DBInfoResourcesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<ResourceSubject> ResourceSubjects { get; set; }
        public virtual DbSet<ResourceType> ResourceTypes { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=I16\\SQLEXPRESS; Database=DBInfoResources; Trusted_Connection=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(true);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(true);

                entity.Property(e => e.Patronymic)
                    .HasMaxLength(32)
                    .IsUnicode(true);

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Authors)
                    .HasForeignKey(d => d.Department)
                    .HasConstraintName("FK_Authors_Departments");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(32)
                    .IsUnicode(true);

                entity.HasOne(d => d.FacultyNavigation)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.Faculty)
                    .HasConstraintName("FK_Departments_Faculties");
            });

            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.Property(e => e.FacultyName)
                    .HasMaxLength(100)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.Property(e => e.AddDate)
                    .HasColumnType("date")
                    .HasDefaultValueSql("getdate()"); 

                entity.Property(e => e.Annotation).HasColumnType("text");

                entity.Property(e => e.ResourceName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.UrlAddress)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.AuthorNavigation)
                    .WithMany(p => p.Resources)
                    .HasForeignKey(d => d.Author)
                    .HasConstraintName("FK_Resources_Authors");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Resources)
                    .HasForeignKey(d => d.Type)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Resources_ResourceTypes");
            });

            modelBuilder.Entity<ResourceSubject>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ResourceSubjects");

                entity.HasIndex(e => new { e.ResourceId, e.SubjectId }, "IX_ResourceSubjects")
                    .IsUnique();

                entity.HasOne(d => d.Resource)
                    .WithMany()
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ResourceSubjects_Resources");

                entity.HasOne(d => d.Subject)
                    .WithMany()
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ResourceSubjects_Subjects");
            });

            modelBuilder.Entity<ResourceType>(entity =>
            {
                entity.ToTable("ResourceTypes");

                entity.Property(e => e.ResourceTypeDescription)
                    .HasColumnType("text")
                    .IsUnicode(true);

                entity.Property(e => e.ResourceTypeName)
                    .HasMaxLength(32)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.SubjectName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
