


Steps to integrate

In solution, add existing project ShoppingCart
In main project, add reference to ShoppingCart.dll

Inherit ProductBase class in Product class in main project Models

Add references to Customer, Order, OrderDetails, ShipToAddress (Cstieg.ShoppingCart) in ApplicationDbContext
Add reference to Product model in ApplicationDbContext

Copy ShoppingCartController to controller in main project, substitute namespaces where necessary
Copy Scripts and Views folders to main project

Add ~/Scripts/Site/ShoppingCart.js to BundleConfig
Add @Scripts.Render statement for the bundle in _Layout.cshtml if not added to a preexisting bundle

Also in _Layout.cshtml, add the following code in a convenient place
	<div id="anti-forgery-token" class="hidden">@Html.AntiForgeryToken()</div>

Add ShoppingCart.PayPal module to pay with PayPal