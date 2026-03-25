using ProRental.Domain.Entities;
using ProRental.Controllers;

namespace ProRental.Data.Interfaces
{
    public interface IPOLineItemMapper
    {
        void InsertItems(int poId, List<Polineitem> items);
        List<Polineitem> FindItemsByPO(int poId);
        void DeleteItemsByPO(int poId);
        void ReplaceItems(int poId, List<Polineitem> items);
        void InsertItemsFromReplenishmentRequest(int poId, int reqId);
        decimal GetTotalAmountByPO(int poId);
        List<PurchaseOrderItemViewModel> GetRequestItemsWithProductName(int reqId);
        List<POLineItemDetailViewModel> GetPOLineItemsWithDetails(int poId);
    }
}
