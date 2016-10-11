using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.Models.Mappings
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            this.ToTable("Users");
            this.HasKey(u => u.Id);
            this.Property(u => u.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(u => u.EmployeeNumber).IsRequired().HasMaxLength(30);
            this.Property(u => u.Name).IsRequired().HasMaxLength(30);
            this.Property(u => u.LastName).IsRequired().HasMaxLength(30);
            this.Property(u => u.AccountName).IsRequired().HasMaxLength(30).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[] { new IndexAttribute(IndexAnnotation.AnnotationName) { IsUnique = true } }));
            this.Property(u => u.Password).IsRequired().HasMaxLength(30);
            this.Property(u => u.AccountType).IsRequired();

            this.HasMany(u => u.Revisions);
        }
    }
}
