namespace Cstieg.Sales.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sales1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShoppingCarts", "OrderId", c => c.Int(nullable: false));
            CreateIndex("dbo.ShoppingCarts", "OrderId");
            AddForeignKey("dbo.ShoppingCarts", "OrderId", "dbo.Orders", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingCarts", "OrderId", "dbo.Orders");
            DropIndex("dbo.ShoppingCarts", new[] { "OrderId" });
            DropColumn("dbo.ShoppingCarts", "OrderId");
        }
    }
}
