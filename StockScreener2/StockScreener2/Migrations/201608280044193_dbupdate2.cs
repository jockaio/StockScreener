namespace StockScreener2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdate2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stocks", "StockPrice_ID", "dbo.StockPrices");
            DropIndex("dbo.Stocks", new[] { "StockPrice_ID" });
            CreateIndex("dbo.StockPrices", "StockID");
            AddForeignKey("dbo.StockPrices", "StockID", "dbo.Stocks", "ID", cascadeDelete: true);
            DropColumn("dbo.Stocks", "StockPrice_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stocks", "StockPrice_ID", c => c.Int());
            DropForeignKey("dbo.StockPrices", "StockID", "dbo.Stocks");
            DropIndex("dbo.StockPrices", new[] { "StockID" });
            CreateIndex("dbo.Stocks", "StockPrice_ID");
            AddForeignKey("dbo.Stocks", "StockPrice_ID", "dbo.StockPrices", "ID");
        }
    }
}
