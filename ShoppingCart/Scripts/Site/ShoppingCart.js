// ShoppingCart.js

function antiForgeryToken() {
    return $('#anti-forgery-token input')[0].value;
}

function shoppingCartPostError(xhr, httpStatusMessage) {
    var message = 'Error: ';
    if (xhr && xhr.responseJSON && xhr.responseJSON.message) {
        message += '\n' + xhr.responseJSON.message;
    }
    else {
        message += xhr.statusCode;
    }
    alert(message);
}

function addToShoppingCart(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/ShoppingCart/AddOrderDetailToShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            incrementShoppingCartBadge();
        },
        error: shoppingCartPostError
    });
}

function buyNow(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/ShoppingCart/AddOrderDetailToShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            window.location = "/shoppingCart";
        },
        error: function (returnval) {
            if (returnval.status = 403) {
                window.location = "/shoppingCart";
            }
            else {
                shoppingCartPostError();
            }
        }
    });
}

function incrementItemInShoppingCart(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/ShoppingCart/IncrementItemInShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            var $qty = $('#qty-' + id)[0];
            $qty.innerText = parseInt($qty.innerText) + 1;
            recalculate();
        },
        error: shoppingCartPostError
    });
}


function decrementItemInShoppingCart(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    var $qty = $('#qty-' + id)[0];

    var qty = parseInt($qty.innerText);
    if (qty < 1) {
        alert('No items to remove!');
        return;
    }
    if (qty === 1) {
        alert('Minimum quantity of 1');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/ShoppingCart/DecrementItemInShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            $qty.innerText = parseInt($qty.innerText) - 1;
            recalculate();
        },
        error: shoppingCartPostError
    });
}

function removeItemInShoppingCart(id) {
    var postData = {
        __RequestVerificationToken: antiForgeryToken(),
        ID: id
    };
    $.ajax({
        type: 'POST',
        url: '/ShoppingCart/RemoveItemInShoppingCart/',
        data: postData,
        dataType: 'json',
        success: function (returnval) {
            var $item = $('#item-' + id)[0];
            $item.remove();

            decrementShoppingCartBadge();

            if (itemsInDetailCount() === 0) {
                location.reload();
            }
            recalculate();
        },
        error: shoppingCartPostError
    });
}

function recalculate() {
    var $itemDetailLines = $('.item-detail-line');
    var extendedPriceTotal = 0;
    var shippingTotal = 0;
    $itemDetailLines.each(function () {
        var linePrice = parseFloat($(this).find('.item-unit-price')[0].innerText.slice(1));
        var lineQty = parseInt($(this).find('.item-qty-ct')[0].innerText);

        var $itemExtendedPrice = $(this).find('.item-extended-price')[0];
        var $itemShipping = $(this).find('.item-shipping')[0];
        var $itemTotalPrice = $(this).find('.item-total-price')[0];

        var itemExtendedPrice = 1.0 * linePrice * lineQty;
        var itemShipping = parseFloat($itemShipping.innerText.slice(1)) || 0;
        var itemTotalPrice = 1.0 * itemExtendedPrice + itemShipping;

        $itemExtendedPrice.innerHTML = '$' + itemExtendedPrice.toFixed(2);
        $itemShipping.innerHTML = '$' + itemShipping.toFixed(2);
        if (itemShipping === 0) {
            $itemShipping.innerHTML = 'FREE';
        }
        $itemTotalPrice.innerHTML = '$' + itemTotalPrice.toFixed(2);

        extendedPriceTotal += itemExtendedPrice;
        shippingTotal += itemShipping;
    });

    $('.item-detail-total .item-extended-price')[0].innerText = '$' + extendedPriceTotal.toFixed(2);
    $('.item-detail-total .item-shipping')[0].innerText = '$' + shippingTotal.toFixed(2);
    $('.item-detail-total .item-total-price')[0].innerText = '$' + (extendedPriceTotal + shippingTotal).toFixed(2);
}


function itemsInDetailCount() {
    var $itemDetailLines = $('.item-detail-line');
    return $itemDetailLines.length;
}


// ****************** Shopping cart badge *******************************************************
(function getShoppingCartCount() {
    $.getJSON('/shoppingCart/ShoppingCartCount', function (data) {
        $('#shoppingCartCount').text(data.shoppingCartCount);
    });
})();

function incrementShoppingCartBadge() {
    var currentCount = parseInt($('#shoppingCartCount').text());
    $('#shoppingCartCount').text(currentCount + 1);
}

function decrementShoppingCartBadge() {
    var currentCount = parseInt($('#shoppingCartCount').text());
    $('#shoppingCartCount').text(currentCount - 1);
}

