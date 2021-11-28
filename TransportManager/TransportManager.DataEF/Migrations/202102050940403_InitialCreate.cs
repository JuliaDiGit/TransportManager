namespace TransportManager.DataEF.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.companies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false),
                        CompanyName = c.String(nullable: false, maxLength: 80),
                        Id = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        SoftDeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyId);
            
            CreateTable(
                "dbo.drivers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        CompanyId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        SoftDeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.vehicles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Model = c.String(nullable: false, maxLength: 80),
                        GovernmentNumber = c.String(nullable: false, maxLength: 9),
                        CompanyId = c.Int(nullable: false),
                        DriverId = c.Int(),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        SoftDeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.drivers", t => t.DriverId)
                .Index(t => t.CompanyId)
                .Index(t => t.DriverId);
            
            CreateTable(
                "dbo.users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false, maxLength: 20, unicode: false),
                        Password = c.String(nullable: false, maxLength: 50),
                        Role = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        SoftDeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Login, unique: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.vehicles", "DriverId", "dbo.drivers");
            DropForeignKey("dbo.vehicles", "CompanyId", "dbo.companies");
            DropForeignKey("dbo.drivers", "CompanyId", "dbo.companies");
            DropIndex("dbo.users", new[] { "Login" });
            DropIndex("dbo.vehicles", new[] { "DriverId" });
            DropIndex("dbo.vehicles", new[] { "CompanyId" });
            DropIndex("dbo.drivers", new[] { "CompanyId" });
            DropTable("dbo.users");
            DropTable("dbo.vehicles");
            DropTable("dbo.drivers");
            DropTable("dbo.companies");
        }
    }
}
