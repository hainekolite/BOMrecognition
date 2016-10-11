namespace BomRainB.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Revisions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentName = c.String(nullable: false),
                        DocumentVersion = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeNumber = c.String(nullable: false, maxLength: 30),
                        Name = c.String(nullable: false, maxLength: 30),
                        LastName = c.String(nullable: false, maxLength: 30),
                        AccountName = c.String(nullable: false, maxLength: 30),
                        Password = c.String(nullable: false, maxLength: 30),
                        AccountType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AccountName, unique: true, name: "Index");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Revisions", "UserId", "dbo.Users");
            DropIndex("dbo.Users", "Index");
            DropIndex("dbo.Revisions", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.Revisions");
        }
    }
}
