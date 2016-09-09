namespace StockScreener2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdate6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockPrices", "Close", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockPrices", "Close");
        }
    }
}
