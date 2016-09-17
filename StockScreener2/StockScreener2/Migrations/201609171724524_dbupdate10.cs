namespace StockScreener2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdate10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HistoricalStockPrices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StockID = c.Int(nullable: false),
                        Symbol = c.String(),
                        Date = c.DateTime(nullable: false),
                        Open = c.Decimal(nullable: false, precision: 18, scale: 2),
                        High = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Low = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Close = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Volume = c.Int(nullable: false),
                        AdjClose = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Stocks", t => t.StockID, cascadeDelete: true)
                .Index(t => t.StockID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HistoricalStockPrices", "StockID", "dbo.Stocks");
            DropIndex("dbo.HistoricalStockPrices", new[] { "StockID" });
            DropTable("dbo.HistoricalStockPrices");
        }
    }
}
