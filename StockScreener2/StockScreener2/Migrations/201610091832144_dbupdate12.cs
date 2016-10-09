namespace StockScreener2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdate12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "CalculationType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "CalculationType");
        }
    }
}
