


Steps to integrate

In solution, add existing project ShoppingCart
In main project, add reference to ShoppingCart.dll

Inherit ProductBase class in Product class in main project Models

Add references to Customer, Order, OrderDetails, ShipToAddress (Cstieg.ShoppingCart) in ApplicationDbContext
Add reference to Product model in ApplicationDbContext

Copy ShoppingCartController to controller in main project, substitute namespaces where necessary