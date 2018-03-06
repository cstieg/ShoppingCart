*Steps to integrate*

* In solution, add existing projects ShoppingCart, ControllerHelper, and FileHelper
* In main project, add reference to these projects

* In IdentityModels.cs, make ApplicationDbContext implement ISalesDbContext (see sample)

* Copy sample Controllers to main project, substituting namespaces (_________________).  Be careful not to overwrite custom code that may be existing in controllers already.
* Copy Scripts and Views folders to main project, and modify as necessary.

* Update Bundle.config and _Layout.cshtml with bundles for site and shopping cart page

* Also in _Layout.cshtml, add the following code above the Scripts
	<div id="anti-forgery-token" class="hidden">@Html.AntiForgeryToken()</div>

* Add ShoppingCart.PayPal module to pay with PayPal