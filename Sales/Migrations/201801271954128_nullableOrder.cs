namespace Cstieg.Sales.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullableOrder : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShoppingCarts", "OrderId", "dbo.Orders");
            DropIndex("dbo.ShoppingCarts", new[] { "OrderId" });
            AlterColumn("dbo.ShoppingCarts", "OrderId", c => c.Int());
            CreateIndex("dbo.ShoppingCarts", "OrderId");
            AddForeignKey("dbo.ShoppingCarts", "OrderId", "dbo.Orders", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingCarts", "OrderId", "dbo.Orders");
            DropIndex("dbo.ShoppingCarts", new[] { "OrderId" });
            AlterColumn("dbo.ShoppingCarts", "OrderId", c => c.Int(nullable: false));
            CreateIndex("dbo.ShoppingCarts", "OrderId");
            AddForeignKey("dbo.ShoppingCarts", "OrderId", "dbo.Orders", "Id", cascadeDelete: true);
        }
    }
}
