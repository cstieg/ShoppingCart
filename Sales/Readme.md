


*Steps to integrate*

* In solution, add existing projects ShoppingCart, ControllerHelper, Geography, StringHelper, Image, and SessionHelper
* In main project, add reference to ShoppingCart and ControllerHelper projects

* Inherit ProductBase class in Product class in main project Models

* Add DbSet properties to ApplicationDbContext class in IdentityModels.cs
        public DbSet<Cstieg.Sales.Customer> Customers { get; set; }
        public DbSet<Cstieg.Sales.Order> Orders { get; set; }
        public DbSet<Cstieg.Sales.OrderDetail> OrderDetails { get; set; }
        public DbSet<Cstieg.Sales.ShipToAddress> Addresses { get; set; }
		public DbSet<Product> Products { get; set; }

Copy ShoppingCartController to controller in main project, substitute namespaces where necessary
Copy Scripts and Views folders to main project

Add ~/Scripts/Site/ShoppingCart.js to BundleConfig
Add @Scripts.Render statement for the bundle in _Layout.cshtml if not added to a preexisting bundle

Also in _Layout.cshtml, add the following code in a convenient place
	<div id="anti-forgery-token" class="hidden">@Html.AntiForgeryToken()</div>

Add ShoppingCart.PayPal module to pay with PayPal