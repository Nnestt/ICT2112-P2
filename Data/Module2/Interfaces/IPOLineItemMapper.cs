using ProRental.Domain.Entities;

namespace ProRental.Data.Interfaces
{
    public interface IPOLineItemMapper
    {
        void InsertItems(int poId, List<Polineitem> items);
        List<Polineitem> FindItemsByPO(int poId);
        void DeleteItemsByPO(int poId);
        void ReplaceItems(int poId, List<Polineitem> items);
    }
}