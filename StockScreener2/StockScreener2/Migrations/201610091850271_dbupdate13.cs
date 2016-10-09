namespace StockScreener2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdate13 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Settings", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Settings", new[] { "UserID" });
            AlterColumn("dbo.Settings", "UserID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Settings", "UserID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Settings", "UserID");
            AddForeignKey("dbo.Settings", "UserID", "dbo.AspNetUsers", "Id");
        }
    }
}
