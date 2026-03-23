using ProRental.Domain.Entities;

namespace ProRental.Data.Interfaces
{
    public interface IPurchaseOrderMapper
    {
        int Insert(Purchaseorder po);
        Purchaseorder? FindById(int poId);
        Purchaseorder? FindByRequestId(int reqId);
        void UpdateExpectedDeliveryDate(int poId, DateOnly expectedDeliveryDate);
    }
}