namespace Cstieg.Sales.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShippingPerOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShippingCountries", "BaseShippingIsPerItem", c => c.Boolean(nullable: false));
            AddColumn("dbo.ShippingCountries", "AdditionalShippingIsPerItem", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShippingCountries", "AdditionalShippingIsPerItem");
            DropColumn("dbo.ShippingCountries", "BaseShippingIsPerItem");
        }
    }
}
