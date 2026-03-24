using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Data;
using ProRental.Interfaces.Domain;

namespace ProRental.Domain.Controls;

public class ReturnOrderControl : iReturnOrderCRUD, iReturnOrderQuery, iReturnProcess
{
    private readonly IReturnRequestMapper _returnRequestMapper;
    private readonly IReturnItemMapper    _returnItemMapper;

    public ReturnOrderControl(
        IReturnRequestMapper returnRequestMapper,
        IReturnItemMapper    returnItemMapper)
    {
        _returnRequestMapper = returnRequestMapper ?? throw new ArgumentNullException(nameof(returnRequestMapper));
        _returnItemMapper    = returnItemMapper    ?? throw new ArgumentNullException(nameof(returnItemMapper));
    }

    // -- iReturnOrderQuery ------------------------------------------------

    public ReturnRequestStatus GetReturnStatus(int returnRequestId)
    {
        var request = _returnRequestMapper.FindById(returnRequestId);
        return request?.GetStatus() ?? ReturnRequestStatus.PROCESSING;
    }

    public Returnrequest? GetReturnRequestByOrderId(int orderId)
    {
        return _returnRequestMapper.FindByOrderId(orderId);
    }

    public ICollection<Returnrequest> GetAllReturnRequests()
    {
        return _returnRequestMapper.FindAll() ?? new List<Returnrequest>();
    }

    public Returnrequest? GetReturnRequestById(int returnRequestId)
    {
        return _returnRequestMapper.FindById(returnRequestId);
    }

    // -- iReturnOrderCRUD -------------------------------------------------

    public bool CreateReturnRequest(Returnrequest returnRequest)
    {
        if (returnRequest is null) return false;
        try { _returnRequestMapper.Insert(returnRequest); return true; }
        catch { return false; }
    }

    public bool CompleteReturnProcess(int returnRequestId)
    {
        var items = _returnItemMapper.FindByReturnRequest(returnRequestId);
        if (items is null || items.Count == 0) return false;
        if (!items.All(i => i.GetStatus() == ReturnItemStatus.RETURN_TO_INVENTORY)) return false;

        var fresh = _returnRequestMapper.FindById(returnRequestId);
        if (fresh is null) return false;

        fresh.CompleteReturn();

        try { _returnRequestMapper.Update(fresh); return true; }
        catch { return false; }
    }

    // -- Control-level methods (from diagram) -----------------------------

    public bool UpdateReturnStatus(int returnRequestId, string status)
    {
        var fresh = _returnRequestMapper.FindById(returnRequestId);
        if (fresh is null) return false;
        if (!Enum.TryParse<ReturnRequestStatus>(status, out var parsedStatus)) return false;
        fresh.SetStatus(parsedStatus);
        try { _returnRequestMapper.Update(fresh); return true; }
        catch { return false; }
    }

    public bool AcknowledgeReturn(int returnRequestId, int staffId)
    {
        var fresh = _returnRequestMapper.FindById(returnRequestId);
        if (fresh is null) return false;
        try { _returnRequestMapper.Update(fresh); return true; }
        catch { return false; }
    }

    public bool RejectReturn(int returnRequestId, int staffId, string reason)
    {
        var fresh = _returnRequestMapper.FindById(returnRequestId);
        if (fresh is null || string.IsNullOrWhiteSpace(reason)) return false;
        fresh.SetStatus(ReturnRequestStatus.COMPLETED);
        try { _returnRequestMapper.Update(fresh); return true; }
        catch { return false; }
    }

    public bool ValidateReturnRequest(Returnrequest returnRequest)
    {
        if (returnRequest is null) return false;
        if (returnRequest.GetOrderId() <= 0 || returnRequest.GetCustomerId() <= 0) return false;
        return _returnRequestMapper.FindByOrderId(returnRequest.GetOrderId()) is null;
    }

    // -- iReturnProcess ---------------------------------------------------

    public bool TriggerReturnProcess(int orderId, int customerId, DateTime requestDate, List<int> inventoryItemIds)
    {
        if (orderId <= 0 || customerId <= 0 || inventoryItemIds is null || inventoryItemIds.Count == 0) return false;
        if (_returnRequestMapper.FindByOrderId(orderId) != null) return false;

        try
        {
            var returnRequest = new Returnrequest();
            returnRequest.SetOrderId(orderId);
            returnRequest.SetCustomerId(customerId);
            returnRequest.SetStatus(ReturnRequestStatus.PROCESSING);
            returnRequest.SetRequestDate(DateTime.UtcNow);
            _returnRequestMapper.Insert(returnRequest);
            return true;
        }
        catch { return false; }
    }
}