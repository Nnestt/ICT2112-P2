using Microsoft.EntityFrameworkCore;
using ProRental.Data.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Controllers;

namespace ProRental.Data.Gateways
{
    public class POLineItemMapper : IPOLineItemMapper
    {
        private readonly AppDbContext _context;

        public POLineItemMapper(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void InsertItems(int poId, List<Polineitem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                _context.Entry(item).Property("Poid").CurrentValue = poId;
                _context.Polineitems.Add(item);
            }

            _context.SaveChanges();
        }

        public List<Polineitem> FindItemsByPO(int poId)
        {
            return _context.Polineitems
                .Where(item => EF.Property<int?>(item, "Poid") == poId)
                .OrderBy(item => EF.Property<int>(item, "Polineid"))
                .ToList();
        }

        public void DeleteItemsByPO(int poId)
        {
            var items = _context.Polineitems
                .Where(item => EF.Property<int?>(item, "Poid") == poId)
                .ToList();

            if (items.Count == 0)
            {
                return;
            }

            _context.Polineitems.RemoveRange(items);
            _context.SaveChanges();
        }

        public void ReplaceItems(int poId, List<Polineitem> items)
        {
            DeleteItemsByPO(poId);
            InsertItems(poId, items);
        }

        public void InsertItemsFromReplenishmentRequest(int poId, int reqId)
        {
            var sourceItems = (from li in _context.Lineitems
                               join pd in _context.Productdetails
                                   on EF.Property<int?>(li, "Productid") equals (int?)EF.Property<int>(pd, "Productid")
                                   into pdGroup
                               from pd in pdGroup.DefaultIfEmpty()
                               where EF.Property<int?>(li, "Requestid") == reqId
                               orderby EF.Property<int>(li, "Lineitemid")
                               select new
                               {
                                   ProductId = EF.Property<int?>(li, "Productid"),
                                   Qty = EF.Property<int?>(li, "Quantityrequest"),
                                   UnitPrice = pd != null ? EF.Property<decimal>(pd, "Price") : 0m
                               })
                               .ToList();

            var poItems = sourceItems.Select(item => new Polineitem
            {
            }).ToList();

            for (var i = 0; i < poItems.Count; i++)
            {
                var source = sourceItems[i];
                var poItem = poItems[i];
                var qty = source.Qty ?? 0;
                var lineTotal = qty * source.UnitPrice;

                _context.Entry(poItem).Property("Poid").CurrentValue = poId;
                _context.Entry(poItem).Property("Productid").CurrentValue = source.ProductId;
                _context.Entry(poItem).Property("Qty").CurrentValue = source.Qty;
                _context.Entry(poItem).Property("Unitprice").CurrentValue = source.UnitPrice;
                _context.Entry(poItem).Property("Linetotal").CurrentValue = lineTotal;
            }

            if (poItems.Count == 0)
            {
                return;
            }

            _context.Polineitems.AddRange(poItems);
            _context.SaveChanges();
        }

        public decimal GetTotalAmountByPO(int poId)
        {
            var total = _context.Polineitems
                .Where(item => EF.Property<int?>(item, "Poid") == poId)
                .Sum(item => (decimal?)EF.Property<decimal?>(item, "Linetotal"));

            return total ?? 0m;
        }

        public List<POLineItemDetailViewModel> GetPOLineItemsWithDetails(int poId)
        {
            return (from pli in _context.Polineitems
                    join pd in _context.Productdetails
                        on EF.Property<int?>(pli, "Productid") equals (int?)EF.Property<int>(pd, "Productid")
                        into pdGroup
                    from pd in pdGroup.DefaultIfEmpty()
                    where EF.Property<int?>(pli, "Poid") == poId
                    orderby EF.Property<int>(pli, "Polineid")
                    select new POLineItemDetailViewModel
                    {
                        ProductId = EF.Property<int?>(pli, "Productid") ?? 0,
                        ProductName = pd != null ? (EF.Property<string?>(pd, "Name") ?? "Unknown") : "Unknown",
                        Qty = EF.Property<int?>(pli, "Qty") ?? 0,
                        UnitPrice = EF.Property<decimal?>(pli, "Unitprice") ?? 0m,
                        LineTotal = EF.Property<decimal?>(pli, "Linetotal") ?? 0m
                    })
                .ToList();
        }

        public List<PurchaseOrderItemViewModel> GetRequestItemsWithProductName(int reqId)
        {
            return (from li in _context.Lineitems
                    join pd in _context.Productdetails
                        on EF.Property<int?>(li, "Productid") equals (int?)EF.Property<int>(pd, "Productid")
                        into pdGroup
                    from pd in pdGroup.DefaultIfEmpty()
                    where EF.Property<int?>(li, "Requestid") == reqId
                    orderby EF.Property<int>(li, "Lineitemid")
                    select new PurchaseOrderItemViewModel
                    {
                        LineItemId = EF.Property<int>(li, "Lineitemid"),
                        ProductId = EF.Property<int?>(li, "Productid") ?? 0,
                        ProductName = pd != null ? (EF.Property<string?>(pd, "Name") ?? "Unknown Product") : "Unknown Product",
                        Qty = EF.Property<int?>(li, "Quantityrequest") ?? 0,
                        Remarks = EF.Property<string?>(li, "Remarks") ?? ""
                    })
                .ToList();
        }
    }
}
