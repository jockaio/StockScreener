namespace StockScreener2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdate8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockPrices", "Change", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockPrices", "Change");
        }
    }
}
