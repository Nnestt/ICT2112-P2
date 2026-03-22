using ProRental.Controllers;

namespace ProRental.Interfaces
{
    public interface IPurchaseOrderService
    {
        PurchaseOrderPageViewModel GetPurchaseOrderPageData(int reqId);
        int ConfirmPurchaseOrder(int reqId, int supplierId, DateOnly? expectedDeliveryDate);
    }
}