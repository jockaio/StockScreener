namespace StockScreener2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Ticker = c.String(),
                        StockPrice_StockID = c.Int(nullable: false),
                        StockPrice_High = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockPrice_Low = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockPrice_Open = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockPrice_Close = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockPrice_Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Stocks");
        }
    }
}
