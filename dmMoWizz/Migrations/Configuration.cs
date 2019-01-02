namespace dmMoWizz.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<dmMoWizz.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(dmMoWizz.Models.ApplicationDbContext context)
        {
            context.Users.AddOrUpdate(p => p.UserName, new Models.ApplicationUser { UserName = "Tomi" });
        }
    }
}
