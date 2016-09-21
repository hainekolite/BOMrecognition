using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.Models.Mappings
{
    public class RevisionMap : EntityTypeConfiguration<Revision>
    {
        public RevisionMap()
        {
            this.ToTable("Revisions");
            this.HasKey(r => r.Id);
            this.Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(r => r.Date).IsRequired();
            this.Property(r => r.DocumentName).IsRequired();
            this.Property(r => r.DocuemntVersion).IsRequired();

            this.HasRequired(r => r.User).WithMany(r => r.Revisions).HasForeignKey(r => r.UserId);

        }
        
    }
}
