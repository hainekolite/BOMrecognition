namespace BomRainB.Data.Migrations
{
    using Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<BomRainB.Data.BomRainBirdDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BomRainB.Data.BomRainBirdDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Users.AddOrUpdate( u => u.Id,
                new User { Id = 1, Name = "Jaime", LastName = "Carpintero Carrillo", AccountName = "Jaime", Password = "hola", AccountType = 1},
                new User { Id = 1, Name = "Esmeralda", LastName = "Quintero Flores", AccountName = "Jaime", Password = "hola", AccountType = 1 }
            );

            context.SaveChanges();
        }
    }
}
