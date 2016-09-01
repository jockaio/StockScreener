namespace StockScreener2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdate1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockPrices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StockID = c.Int(nullable: false),
                        Bid = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ask = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DaysLow = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DaysHigh = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Open = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Stocks", "StockPrice_ID", c => c.Int());
            CreateIndex("dbo.Stocks", "StockPrice_ID");
            AddForeignKey("dbo.Stocks", "StockPrice_ID", "dbo.StockPrices", "ID");
            DropColumn("dbo.Stocks", "StockPrice_StockID");
            DropColumn("dbo.Stocks", "StockPrice_High");
            DropColumn("dbo.Stocks", "StockPrice_Low");
            DropColumn("dbo.Stocks", "StockPrice_Open");
            DropColumn("dbo.Stocks", "StockPrice_Close");
            DropColumn("dbo.Stocks", "StockPrice_Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stocks", "StockPrice_Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Stocks", "StockPrice_Close", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Stocks", "StockPrice_Open", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Stocks", "StockPrice_Low", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Stocks", "StockPrice_High", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Stocks", "StockPrice_StockID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Stocks", "StockPrice_ID", "dbo.StockPrices");
            DropIndex("dbo.Stocks", new[] { "StockPrice_ID" });
            DropColumn("dbo.Stocks", "StockPrice_ID");
            DropTable("dbo.StockPrices");
        }
    }
}
