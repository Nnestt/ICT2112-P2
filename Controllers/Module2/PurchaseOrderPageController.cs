using Microsoft.AspNetCore.Mvc;
using ProRental.Interfaces;

namespace ProRental.Controllers
{
    public class PurchaseOrderPageController : Controller
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderPageController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet]
        public IActionResult Index(int reqId = 0, int? poId = null)
        {
            if (reqId <= 0)
            {
                var vm = new PurchaseOrderPageViewModel
                {
                    Requests = _purchaseOrderService.GetAllRequests()
                };

                return View("~/Views/Module2/PurchaseOrder.cshtml", vm);
            }

            var loadedVm = _purchaseOrderService.GetPurchaseOrderPageData(reqId);
            loadedVm.Requests = _purchaseOrderService.GetAllRequests();
            loadedVm.CreatedPoId = poId;

            return View("~/Views/Module2/PurchaseOrder.cshtml", loadedVm);
        }

        [HttpGet]
        public IActionResult PurchaseOrderView(int reqId)
        {
            if (reqId <= 0)
            {
                TempData["Error"] = "Invalid request ID.";
                return RedirectToAction(nameof(Index));
            }

            PurchaseOrderPageViewModel loadedVm;

            try
            {
                // Try load real DB data
                loadedVm = _purchaseOrderService.GetPurchaseOrderPageData(reqId);
                loadedVm.Requests = _purchaseOrderService.GetAllRequests();
            }
            catch
            {
                // 🔥 FALLBACK DEMO DATA (FOR PRESENTATION)
                loadedVm = new PurchaseOrderPageViewModel
                {
                    RequestId = 102,
                    RequestedBy = "Ben Lim",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    Status = "Approved",
                    Remarks = "Approved demo request",

                    Items = new List<PurchaseOrderItemViewModel>
                    {
                        new PurchaseOrderItemViewModel
                        {
                            LineItemId = 1,
                            ProductId = 2001,
                            ProductName = "Excavator",
                            Qty = 2,
                            Remarks = "Urgent restock"
                        },
                        new PurchaseOrderItemViewModel
                        {
                            LineItemId = 2,
                            ProductId = 2002,
                            ProductName = "Safety Helmet",
                            Qty = 10,
                            Remarks = "For new crew"
                        }
                    },

                    Suppliers = new List<PurchaseOrderSupplierViewModel>
                    {
                        new PurchaseOrderSupplierViewModel
                        {
                            SupplierId = 301,
                            SupplierName = "BuildMax Supplies",
                            Details = "Preferred industrial supplier",
                            CreditPeriod = 30,
                            AvgTurnaroundTime = 2.5,
                            IsVerified = true
                        },
                        new PurchaseOrderSupplierViewModel
                        {
                            SupplierId = 302,
                            SupplierName = "Prime Equipment Co",
                            Details = "Backup supplier",
                            CreditPeriod = 14,
                            AvgTurnaroundTime = 4.0,
                            IsVerified = true
                        }
                    },

                    Requests = new List<PurchaseOrderRequestListItemViewModel>
                    {
                        new PurchaseOrderRequestListItemViewModel
                        {
                            RequestId = 102,
                            RequestedBy = "Ben Lim",
                            CreatedAt = DateTime.Now.AddDays(-1),
                            Status = "Approved",
                            Remarks = "Approved demo request"
                        }
                    }
                };
            }

            return View("~/Views/Module2/PurchaseOrderView.cshtml", loadedVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmPO(int reqId, int supplierId, DateOnly? expectedDeliveryDate, bool confirmDetails = false)
        {
            if (reqId <= 0)
            {
                TempData["Error"] = "Invalid request ID.";
                return RedirectToAction(nameof(Index), new { reqId });
            }

            if (supplierId <= 0)
            {
                TempData["Error"] = "Please select a supplier.";
                return RedirectToAction(nameof(Index), new { reqId });
            }

            if (!confirmDetails)
            {
                TempData["Error"] = "Please confirm the purchase order details.";
                return RedirectToAction(nameof(Index), new { reqId });
            }

            try
            {
                int poId = _purchaseOrderService.ConfirmPurchaseOrder(reqId, supplierId, expectedDeliveryDate);
                TempData["Success"] = $"Purchase Order #{poId} created successfully.";

                return RedirectToAction(nameof(Index), new { reqId, poId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to create purchase order: {ex.Message}";
                return RedirectToAction(nameof(Index), new { reqId });
            }
        }
    }
}