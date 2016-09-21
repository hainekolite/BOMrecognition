using BomRainB.Models;
using BomRainB.Models.Mappings;
using System.Data.Entity;

namespace BomRainB.Data
{
    public class BomRainBirdDbContext : DbContext
    {
        public IDbSet<User> Users { get; set; }
        public IDbSet<Revision> Revisions { get; set; }

        public BomRainBirdDbContext() : base("BomRainBirdDbContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new RevisionMap());
        }
    }
}
