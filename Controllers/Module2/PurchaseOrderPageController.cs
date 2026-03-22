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
                return View("~/Views/Module2/PurchaseOrder.cshtml", new PurchaseOrderPageViewModel());
            }

            var vm = _purchaseOrderService.GetPurchaseOrderPageData(reqId);
            vm.CreatedPoId = poId;

            return View("~/Views/Module2/PurchaseOrder.cshtml", vm);
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