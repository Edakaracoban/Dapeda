using Microsoft.AspNetCore.Mvc;
using Dapeda.Models;
using Dapper;

namespace Dapeda.Controllers
{
    public class OrderItemController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var orderItems = DP.Listeleme<OrderItem>("sp_GetAllOrderItems");
            return View(orderItems);
        }

        [HttpGet]
        public IActionResult ListByOrder(int orderId)
        {
            var param = new DynamicParameters();
            param.Add("@OrderId", orderId);
            var items = DP.Listeleme<OrderItem>("sp_GetOrderItemsByOrderId", param);

            ViewBag.OrderId = orderId;  // Bunu ekle

            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(OrderItem orderItem)
        {
            if (!ModelState.IsValid)
                return View(orderItem);

            var param = new DynamicParameters();
            param.Add("@OrderId", orderItem.OrderId);
            param.Add("@ProductId", orderItem.ProductId);
            param.Add("@Quantity", orderItem.Quantity);
            param.Add("@UnitPrice", orderItem.UnitPrice);

            try
            {
                DP.ExecuteReturn("sp_AddOrderItem", param);
                TempData["SuccessMessage"] = "Sipariş kalemi başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Sipariş kalemi eklenemedi: " + ex.Message;
                return View(orderItem);
            }
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var param = new DynamicParameters();
            param.Add("@OrderItemId", id);
            var orderItem = DP.Get<OrderItem>("sp_GetOrderItemById", param);
            if (orderItem == null)
                return NotFound();

            return View(orderItem);
        }

        [HttpPost]
        public IActionResult Edit(OrderItem orderItem)
        {
            if (!ModelState.IsValid)
                return View(orderItem);

            var param = new DynamicParameters();
            param.Add("@OrderItemId", orderItem.OrderItemId);
            param.Add("@OrderId", orderItem.OrderId);
            param.Add("@ProductId", orderItem.ProductId);
            param.Add("@Quantity", orderItem.Quantity);
            param.Add("@UnitPrice", orderItem.UnitPrice);

            try
            {
                DP.ExecuteReturn("sp_UpdateOrderItem", param);
                TempData["SuccessMessage"] = "Sipariş kalemi başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Sipariş kalemi güncellenemedi: " + ex.Message;
                return View(orderItem);
            }
        }

       [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@OrderItemId", id);
                DP.ExecuteReturn("sp_DeleteOrderItem", param);

                TempData["SuccessMessage"] = "Sipariş kalemi başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sipariş kalemi silinirken hata oluştu: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
