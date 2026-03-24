namespace ProRental.Domain.Entities;

/// <summary>
/// Feature 1 extension methods for the shared Checkout entity. They isolate the shipping
/// selection behavior that Feature 1 needs to confirm the customer's chosen option.
/// by: ernest
/// </summary>
public partial class Checkout
{
    public void Initialize(int customerId, int cartId, DateTime createdAt)
    {
        _customerid = customerId;
        _cartid = cartId;
        _createdat = createdAt;
    }

    public (int CustomerId, int CartId, DateTime CreatedAt) GetCheckoutContext()
    {
        return (_customerid, _cartid, _createdat);
    }

    public (int CheckoutId, int? SelectedOptionId) GetSelectionState()
    {
        return (_checkoutid, _optionId);
    }

    public void SelectShippingOption(int optionId) => _optionId = optionId;
    public void ClearSelectedShippingOption() => _optionId = null;
}
