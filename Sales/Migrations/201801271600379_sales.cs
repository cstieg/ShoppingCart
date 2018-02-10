namespace Cstieg.Sales.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sales : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(),
                        Recipient = c.String(maxLength: 50),
                        Address1 = c.String(maxLength: 50),
                        Address2 = c.String(maxLength: 50),
                        City = c.String(maxLength: 50),
                        State = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 15),
                        Country = c.String(maxLength: 50),
                        Phone = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Registered = c.DateTime(nullable: false),
                        LastVisited = c.DateTime(nullable: false),
                        TimesVisited = c.Int(nullable: false),
                        EmailAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsoCode2 = c.String(nullable: false, maxLength: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        PlacedInCart = c.DateTime(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderId = c.Int(nullable: false),
                        IsPromotionalItem = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Cart = c.String(maxLength: 20),
                        CustomerId = c.Int(),
                        DateOrdered = c.DateTime(),
                        ShipToAddressId = c.Int(),
                        BillToAddressId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.BillToAddressId)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Addresses", t => t.ShipToAddressId)
                .Index(t => t.Cart)
                .Index(t => t.CustomerId)
                .Index(t => t.ShipToAddressId)
                .Index(t => t.BillToAddressId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        UrlName = c.String(),
                        MetaDescription = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShippingSchemeId = c.Int(),
                        ProductInfo = c.String(maxLength: 2000),
                        DisplayOnFrontPage = c.Boolean(nullable: false),
                        DoNotDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShippingSchemes", t => t.ShippingSchemeId)
                .Index(t => t.ShippingSchemeId);
            
            CreateTable(
                "dbo.ShippingSchemes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PromoCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 100),
                        PromotionalItemId = c.Int(),
                        PromotionalItemPrice = c.Decimal(precision: 18, scale: 2),
                        WithPurchaseOfId = c.Int(),
                        MinimumQualifyingPurchase = c.Decimal(precision: 18, scale: 2),
                        PercentOffOrder = c.Decimal(precision: 18, scale: 2),
                        PercentOffItem = c.Decimal(precision: 18, scale: 2),
                        SpecialPrice = c.Decimal(precision: 18, scale: 2),
                        SpecialPriceItemId = c.Int(),
                        CodeStart = c.DateTime(),
                        CodeEnd = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.PromotionalItemId)
                .ForeignKey("dbo.Products", t => t.SpecialPriceItemId)
                .ForeignKey("dbo.Products", t => t.WithPurchaseOfId)
                .Index(t => t.Code, unique: true)
                .Index(t => t.PromotionalItemId)
                .Index(t => t.WithPurchaseOfId)
                .Index(t => t.SpecialPriceItemId);
            
            CreateTable(
                "dbo.ShippingCountries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShippingSchemeId = c.Int(nullable: false),
                        CountryId = c.Int(nullable: false),
                        MinQty = c.Int(),
                        MaxQty = c.Int(),
                        AdditionalShipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .ForeignKey("dbo.ShippingSchemes", t => t.ShippingSchemeId, cascadeDelete: true)
                .Index(t => t.ShippingSchemeId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.ShoppingCarts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OwnerId = c.String(maxLength: 36),
                        Country = c.String(),
                        PayeeEmail = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.WebImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageUrl = c.String(nullable: false, maxLength: 200),
                        ImageSrcSet = c.String(maxLength: 1000),
                        Caption = c.String(maxLength: 100),
                        ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebImages", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ShippingCountries", "ShippingSchemeId", "dbo.ShippingSchemes");
            DropForeignKey("dbo.ShippingCountries", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.PromoCodes", "WithPurchaseOfId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "PromotionalItemId", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "ShippingSchemeId", "dbo.ShippingSchemes");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "ShipToAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Orders", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Orders", "BillToAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Addresses", "CustomerId", "dbo.Customers");
            DropIndex("dbo.WebImages", new[] { "ProductId" });
            DropIndex("dbo.ShoppingCarts", new[] { "OwnerId" });
            DropIndex("dbo.ShippingCountries", new[] { "CountryId" });
            DropIndex("dbo.ShippingCountries", new[] { "ShippingSchemeId" });
            DropIndex("dbo.PromoCodes", new[] { "SpecialPriceItemId" });
            DropIndex("dbo.PromoCodes", new[] { "WithPurchaseOfId" });
            DropIndex("dbo.PromoCodes", new[] { "PromotionalItemId" });
            DropIndex("dbo.PromoCodes", new[] { "Code" });
            DropIndex("dbo.Products", new[] { "ShippingSchemeId" });
            DropIndex("dbo.Orders", new[] { "BillToAddressId" });
            DropIndex("dbo.Orders", new[] { "ShipToAddressId" });
            DropIndex("dbo.Orders", new[] { "CustomerId" });
            DropIndex("dbo.Orders", new[] { "Cart" });
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            DropIndex("dbo.OrderDetails", new[] { "ProductId" });
            DropIndex("dbo.Addresses", new[] { "CustomerId" });
            DropTable("dbo.WebImages");
            DropTable("dbo.ShoppingCarts");
            DropTable("dbo.ShippingCountries");
            DropTable("dbo.PromoCodes");
            DropTable("dbo.ShippingSchemes");
            DropTable("dbo.Products");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Countries");
            DropTable("dbo.Customers");
            DropTable("dbo.Addresses");
        }
    }
}
