namespace SOOS_Auction.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        BidId = c.Int(nullable: false, identity: true),
                        User = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        BidDate = c.DateTime(nullable: false),
                        LotId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BidId)
                .ForeignKey("dbo.Lots", t => t.LotId, cascadeDelete: true)
                .Index(t => t.LotId);
            
            CreateTable(
                "dbo.Lots",
                c => new
                    {
                        LotId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        State = c.String(nullable: false),
                        UserId = c.String(nullable: false),
                        WinnerId = c.String(),
                        MinimalPrice = c.Double(nullable: false),
                        CurrentPrice = c.Double(nullable: false),
                        MinimalStep = c.Double(nullable: false),
                        Description = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        DaysDuration = c.Int(nullable: false),
                        FinishDate = c.DateTime(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        ImagesUrl = c.String(),
                    })
                .PrimaryKey(t => t.LotId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.LotPayments", t => t.LotId)
                .ForeignKey("dbo.LotReceivings", t => t.LotId)
                .Index(t => t.LotId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SectionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryId)
                .ForeignKey("dbo.Sections", t => t.SectionId, cascadeDelete: true)
                .Index(t => t.SectionId);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        SectionId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.SectionId);
            
            CreateTable(
                "dbo.LotPayments",
                c => new
                    {
                        LotPaymentId = c.Int(nullable: false, identity: true),
                        Cash = c.Boolean(nullable: false),
                        NonCash = c.Boolean(nullable: false),
                        FullPrepaymentPostSending = c.Boolean(nullable: false),
                        AdditionalInformation = c.String(),
                    })
                .PrimaryKey(t => t.LotPaymentId);
            
            CreateTable(
                "dbo.LotReceivings",
                c => new
                    {
                        LotReceivingId = c.Int(nullable: false, identity: true),
                        Location = c.String(nullable: false),
                        ByPost = c.Boolean(nullable: false),
                        DeliveryInPerson = c.Boolean(nullable: false),
                        ByPostToAnotherCountry = c.Boolean(nullable: false),
                        ReturnAfterBuyingIsForbidden = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LotReceivingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "LotId", "dbo.Lots");
            DropForeignKey("dbo.Lots", "LotId", "dbo.LotReceivings");
            DropForeignKey("dbo.Lots", "LotId", "dbo.LotPayments");
            DropForeignKey("dbo.Lots", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Categories", "SectionId", "dbo.Sections");
            DropIndex("dbo.Categories", new[] { "SectionId" });
            DropIndex("dbo.Lots", new[] { "CategoryId" });
            DropIndex("dbo.Lots", new[] { "LotId" });
            DropIndex("dbo.Bids", new[] { "LotId" });
            DropTable("dbo.LotReceivings");
            DropTable("dbo.LotPayments");
            DropTable("dbo.Sections");
            DropTable("dbo.Categories");
            DropTable("dbo.Lots");
            DropTable("dbo.Bids");
        }
    }
}
