namespace TransportManager.DataEF.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class DefaultCreatedDateMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.companies", 
                        "CreatedDate", 
                        c => c.DateTime(nullable: false, 
                                                                  precision: 7, 
                                                                  defaultValueSql: "GetDate()", 
                                                                  storeType: "datetime2"));
            
            AlterColumn("dbo.drivers", 
                        "CreatedDate", 
                        c => c.DateTime(nullable: false, 
                                                                  precision: 7,
                                                                  defaultValueSql: "GetDate()",
                                                                  storeType: "datetime2"));
            
            AlterColumn("dbo.vehicles", 
                        "CreatedDate", 
                        c => c.DateTime(nullable: false, 
                                                                  precision: 7, 
                                                                  defaultValueSql: "GetDate()", 
                                                                  storeType: "datetime2"));
            
            AlterColumn("dbo.users", 
                        "CreatedDate", 
                        c => c.DateTime(nullable: false, 
                                                                  precision: 7, 
                                                                  defaultValueSql: "GetDate()", 
                                                                  storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.companies", 
                        "CreatedDate", 
                        c => c.DateTime(nullable: false, 
                                                                  precision: 7, 
                                                                  storeType: "datetime2"));
            
            AlterColumn("dbo.users", 
                        "CreatedDate", 
                        c => c.DateTime(nullable: false, 
                                                                  precision: 7, 
                                                                  storeType: "datetime2"));
            
            AlterColumn("dbo.vehicles", 
                        "CreatedDate", 
                        c => c.DateTime(nullable: false,
                                                                  precision: 7, 
                                                                  storeType: "datetime2"));
            
            AlterColumn("dbo.drivers", 
                        "CreatedDate", 
                        c => c.DateTime(nullable: false, 
                                                                  precision: 7, 
                                                                  storeType: "datetime2"));
        }
    }
}
