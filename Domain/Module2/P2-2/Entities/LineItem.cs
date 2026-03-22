namespace ProRental.Domain.Entities;
using ProRental.Domain.Enums;

public partial class Lineitem
{
    // Public accessors for scaffolded private fields
    public int LineItemId
    {
        get => _lineitemid;
        set => _lineitemid = value;
    }

    public int? RequestId
    {
        get => _requestid;
        set => _requestid = value;
    }

    public int? ProductId
    {
        get => _productid;
        set => _productid = value;
    }

    public int? QuantityRequest
    {
        get => _quantityrequest;
        set => _quantityrequest = value;
    }

    // Private field for Reason (enum)
    private ReplenishmentReason _reason = ReplenishmentReason.LOWSTOCK;

    public ReplenishmentReason Reason
    {
        get => _reason;
        set => _reason = value;
    }

    // Business logic methods from class diagram
    // Set quantity for this line item
    public bool SetQuantity(int quantity)
    {
        if (quantity < 0)
        {
            return false;
        }

        QuantityRequest = quantity;
        return true;
    }

    // Set the reason code for this line item
    public void SetReason(ReplenishmentReason reason)
    {
        Reason = reason;
    }


    // Set remarks for this line item
    public void SetRemarks(string remarks)
    {
        _remarks = remarks;
    }

    // Get remarks for this line item
    public string? GetRemarks()
    {
        return _remarks;
    }

    // Validate if the line item has required data
    public bool IsValid()
    {
        return ProductId.HasValue &&
               ProductId.Value > 0 &&
               QuantityRequest.HasValue &&
               QuantityRequest.Value > 0;
    }
}