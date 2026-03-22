namespace ProRental.Domain.Entities;
using ProRental.Domain.Enums;

public partial class Replenishmentrequest
{
    // Public accessors for scaffolded private fields
    public int RequestId
    {
        get => _requestid;
        set => _requestid = value;
    }

    public string? RequestedBy
    {
        get => _requestedby;
        set => _requestedby = value;
    }

    public ReplenishmentStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public DateTime? CreatedAt
    {
        get => _createdat;
        set => _createdat = value;
    }

    public DateTime? CompletedAt
    {
        get => _completedat;
        set => _completedat = value;
    }

    public string? CompletedBy
    {
        get => _completedby;
        set => _completedby = value;
    }

    // Private field for Status (enum)
    private ReplenishmentStatus _status = ReplenishmentStatus.DRAFT;

    // Business logic methods from class diagram

    // Check if the request can be edited (only DRAFT requests can be edited)
    public bool CanEdit()
    {
        return Status == ReplenishmentStatus.DRAFT;
    }

    /// Check if the request has any line items
    public bool HasLineItem()
    {
        return Lineitems != null && Lineitems.Any();
    }

    // Find a specific line item by ID
    public Lineitem? FindLineItem(int lineItemId)
    {
        return Lineitems?.FirstOrDefault(li => li.LineItemId == lineItemId);
    }

    // Add a new line item to the request
    public Lineitem AddLineItem(int productId)
    {
        if (!CanEdit())
        {
            throw new InvalidOperationException("Cannot add line items to a non-draft request");
        }

        var lineItem = new Lineitem
        {
            RequestId = RequestId,
            ProductId = productId,
            QuantityRequest = 0
        };
        lineItem.SetRemarks(string.Empty);

        Lineitems.Add(lineItem);
        return lineItem;
    }

    /// Remove a line item from the request
    public bool RemoveLineItem(int lineItemId)
    {
        if (!CanEdit())
        {
            return false;
        }

        var lineItem = FindLineItem(lineItemId);
        if (lineItem == null)
        {
            return false;
        }

        Lineitems.Remove(lineItem);
        return true;
    }

    // Submit the request (change status from DRAFT to SUBMITTED)
    public bool Submit()
    {
        if (Status != ReplenishmentStatus.DRAFT)
        {
            return false;
        }

        if (!HasLineItem())
        {
            throw new InvalidOperationException("Cannot submit a request without line items");
        }

        // Validate all line items
        foreach (var lineItem in Lineitems)
        {
            if (!lineItem.IsValid())
            {
                throw new InvalidOperationException($"Line item {lineItem.LineItemId} is invalid");
            }
        }

        Status = ReplenishmentStatus.SUBMITTED;
        return true;
    }

    // Cancel the request
    public bool Cancel()
    {
        if (Status == ReplenishmentStatus.COMPLETED || Status == ReplenishmentStatus.CANCELLED)
        {
            return false;
        }

        Status = ReplenishmentStatus.CANCELLED;
        return true;
    }
}