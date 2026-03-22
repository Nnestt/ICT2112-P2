namespace ProRental.Domain.Entities;

public partial class Checkout
{
    public int GetCheckoutId() => _checkoutid;
    public int GetCustomerId() => _customerid;
    public int GetCartId() => _cartid;
    public int? GetSelectedOptionId() => _optionId;
    public DateTime GetCreatedAt() => _createdat;

    public void SetCustomerId(int customerId) => _customerid = customerId;
    public void SetCartId(int cartId) => _cartid = cartId;
    public void SetCreatedAt(DateTime createdAt) => _createdat = createdAt;
    public void SelectShippingOption(int optionId) => _optionId = optionId;
    public void ClearSelectedShippingOption() => _optionId = null;
}
