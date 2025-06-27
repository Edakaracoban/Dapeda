using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapeda.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class OrderController : Controller
{
    public ActionResult Index(string searchTerm)
    {
        IEnumerable<Order> orders;

        if (!string.IsNullOrEmpty(searchTerm))
        {
            if (int.TryParse(searchTerm, out int orderId))
            {
                var param = new DynamicParameters();
                param.Add("@OrderId", orderId);

                orders = DP.Listeleme<Order>("sp_SearchOrdersByOrderId", param);
            }
            else
            {
                orders = new List<Order>();
            }
        }
        else
        {
            orders = DP.Listeleme<Order>("sp_GetOrders");
        }

        return View(orders);
    }
    public ActionResult CreateOrder()
    {
        var users = DP.Listeleme<User>("sp_GetUsers");
        ViewBag.Users = new SelectList(users, "UserId", "UserName");
        return View();
    }
    [HttpPost]
    public ActionResult CreateOrder(Order o)
    {
        if (!ModelState.IsValid)
        {
            var users = DP.Listeleme<User>("sp_GetUsers");
            ViewBag.Users = new SelectList(users, "UserId", "UserName");
            return View(o);
        }
        else
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@UserId", o.UserId);
            param.Add("@OrderDate", o.OrderDate);
            param.Add("@TotalAmount", o.TotalAmount);
            DP.ExecuteReturn("sp_CreateOrder", param);
            TempData["SuccessMessage"] = "Sipariş başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }
    }
    public ActionResult EditOrder(int id)
    {
        var param = new DynamicParameters();
        param.Add("@OrderId", id);
        var order = DP.Get<Order>("sp_GetOrderById", param);
        if (order == null)
            return NotFound();

        var users = DP.Listeleme<User>("sp_GetUsers");
        ViewBag.Users = new SelectList(users, "UserId", "UserName", order.UserId);
        return View(order);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditOrder(Order o)
    {
        if (!ModelState.IsValid)
        {
            var users = DP.Listeleme<User>("sp_GetUsers");
            ViewBag.Users = new SelectList(users, "UserId", "UserName", o.UserId);
            return View(o);
        }

        try
        {
            var param = new DynamicParameters();
            param.Add("@OrderId", o.OrderId);
            param.Add("@UserId", o.UserId);
            param.Add("@OrderDate", o.OrderDate);
            param.Add("@TotalAmount", o.TotalAmount);

            DP.ExecuteReturn("sp_UpdateOrder", param);
            TempData["SuccessMessage"] = "Sipariş başarıyla güncellendi.";
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Sipariş güncellenirken hata oluştu.");
            var users = DP.Listeleme<User>("sp_GetUsers");
            ViewBag.Users = new SelectList(users, "UserId", "UserName", o.UserId);
            return View(o);
        }
    }
    [HttpPost]
    public ActionResult DeleteOrder(int id)
    {
        var param = new DynamicParameters();
        param.Add("@OrderId", id);

        try
        {
            DP.ExecuteReturn("sp_DeleteOrder", param);
            TempData["SuccessMessage"] = "Sipariş başarıyla silindi.";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Sipariş silinirken hata oluştu.";
        }

        return RedirectToAction("Index");
    }
    public ActionResult OrdersByDate()
    {
        ViewBag.StartDate = null;
        ViewBag.EndDate = null;
        return View();
    }

    [HttpPost]
    public ActionResult OrdersByDate(DateTime startDate, DateTime endDate)
    {
        var param = new DynamicParameters();
        param.Add("@StartDate", startDate);
        param.Add("@EndDate", endDate);

        var orders = DP.Listeleme<Order>("sp_GetOrdersByDateRange", param);
        ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");

        return View(orders);
    }
    [HttpGet]
    public ActionResult SortByAmount()
    {
        var param = new DynamicParameters();
        param.Add("@SortDirection", "ASC");
        ViewBag.CurrentSort = "ASC";
        var orders = DP.Listeleme<Order>("sp_SortOrdersByAmount", param);
        return View(orders);
    }

    [HttpPost]
    public ActionResult SortByAmount(string sortDirection)
    {
        if (string.IsNullOrEmpty(sortDirection))
            sortDirection = "ASC";

        var param = new DynamicParameters();
        param.Add("@SortDirection", sortDirection);
        ViewBag.CurrentSort = sortDirection;
        var orders = DP.Listeleme<Order>("sp_SortOrdersByAmount", param);
        return View(orders);
    }

    public ActionResult PastOrders()
    {
        var orders = DP.Listeleme<Order>("sp_GetPastOrders");

        if (orders == null || !orders.Any())
        {
            TempData["InfoMessage"] = "Geçmiş sipariş bulunamadı.";
        }
        else
        {
            TempData["InfoMessage"] = "Geçmiş siparişler listeleniyor.";
        }

        return View("Index", orders);
    }

    public ActionResult FutureOrders()
    {
        var orders = DP.Listeleme<Order>("sp_GetFutureOrders");

        if (orders == null || !orders.Any())
        {
            TempData["InfoMessage"] = "Gelecek sipariş bulunamadı.";
        }
        else
        {
            TempData["InfoMessage"] = "Gelecek siparişler listeleniyor.";
        }

        return View("Index", orders);
    }



}