namespace StockScreener2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdate7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stocks", "Symbol", c => c.String());
            DropColumn("dbo.Stocks", "Ticker");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stocks", "Ticker", c => c.String());
            DropColumn("dbo.Stocks", "Symbol");
        }
    }
}
