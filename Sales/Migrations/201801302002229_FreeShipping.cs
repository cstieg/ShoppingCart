namespace Cstieg.Sales.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FreeShipping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShippingCountries", "FreeShipping", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShippingCountries", "FreeShipping");
        }
    }
}
