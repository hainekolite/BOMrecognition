using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

            this.Property(u => u.Name).IsRequired();
            this.Property(u => u.LastName).IsRequired();
            this.Property(u => u.AccountName).IsRequired();
            this.Property(u => u.Password).IsRequired();
            this.Property(u => u.AccountType).IsRequired();

            this.HasMany(u => u.Revisions);
        }
    }
}
