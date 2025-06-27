using System;
using System.Data.SqlClient;
using Dapeda.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dapeda.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Index(string searchTerm)
        {
            IEnumerable<Product> urunler;

            bool isSearchTermPresent = Request.Query.ContainsKey("searchTerm");

            if (string.IsNullOrWhiteSpace(searchTerm) && isSearchTermPresent)
            {
                ViewBag.Mesaj = "Lütfen arama için bir ürün adı giriniz.";
            }

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                urunler = DP.Listeleme<Product>("sp_GetProducts");
            }
            else
            {
                var parameters = new DynamicParameters();
                parameters.Add("@SearchTerm", searchTerm);

                urunler = DP.Listeleme<Product>("sp_SearchProductsByName", parameters);
            }

            return View(urunler);
        }




        public ActionResult AddProduct()
        {
            var categories = DP.Listeleme<Category>("sp_GetCategori");
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product p)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@ProductName", p.ProductName);
            param.Add("@Price", p.Price);
            param.Add("@Stock", p.Stock);
            param.Add("@CategoryId", p.CategoryId);

            DP.ExecuteReturn("sp_AddProduct", param);

            return RedirectToAction("Index");
        }
        public ActionResult DeleteProduct(int id)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@ProductId", id);

            DP.ExecuteReturn("sp_DeleteProduct", param);
            return RedirectToAction("Index");
        }
        public ActionResult EditProduct(int id)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@ProductId", id);
            var product = DP.GetSingle<Product>("sp_GetProductById", param);


            var categories = DP.Listeleme<Category>("sp_GetCategori");
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");

            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(Product p)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@ProductId", p.ProductId);
            param.Add("@ProductName", p.ProductName);
            param.Add("@Price", p.Price);
            param.Add("@Stock", p.Stock);
            param.Add("@CategoryId", p.CategoryId);

            DP.ExecuteReturn("sp_UpdateProduct", param);

            return RedirectToAction("Index");
        }


        public ActionResult ProductWithCategory()
        {
            var products = DP.Listeleme<ProductWithCategory>("sp_GetProducts").ToList();
            return View(products);
        }


        public ActionResult GetProductReport()
        {
            var products = DP.Listeleme<ProductReport>("sp_GetProductReport").ToList();
            return View(products);
        }

        public IActionResult SortedProducts(string sortBy, string sortDirection = "ASC")
        {
            IEnumerable<Product> products;

            if (sortBy == "price")
                products = GetProductsSortedByPrice(sortDirection);
            else if (sortBy == "stock")
                products = GetProductsSortedByStock(sortDirection);
            else
                products = DP.Listeleme<Product>("sp_GetProducts");

            return View("ProductList", products); // ProductList.cshtml ya da ürün listeleme view dosyanın adı
        }
        public IEnumerable<Product> GetProductsSortedByPrice(string sortDirection)
        {
            var param = new DynamicParameters();
            param.Add("@SortDirection", sortDirection);

            return DP.Listeleme<Product>("sp_SortProductsByPrice", param);
        }

        public IEnumerable<Product> GetProductsSortedByStock(string sortDirection)
        {
            var param = new DynamicParameters();
            param.Add("@SortDirection", sortDirection);

            return DP.Listeleme<Product>("sp_SortProductsByStock", param);
        }


        public IActionResult CategoryPriceReport()
        {
            var model = GetAveragePriceByCategory();
            return View(model);
        }

        public IEnumerable<CategoryPriceSummary> GetAveragePriceByCategory()
        {
            return DP.Listeleme<CategoryPriceSummary>("sp_GetAveragePriceByCategory");
        }


        public Product GetProductWithMaxStock()
        {
            return DP.GetSingle<Product>("sp_GetProductWithMaxStock");
        }

        public Product GetProductWithMinStock()
        {
            return DP.GetSingle<Product>("sp_GetProductWithMinStock");
        }

        public Product GetProductWithMaxPrice()
        {
            return DP.GetSingle<Product>("sp_GetProductWithMaxPrice");
        }

        public Product GetProductWithMinPrice()
        {
            return DP.GetSingle<Product>("sp_GetProductWithMinPrice");
        }

        public IActionResult ProductWithMaxStock()
        {
            var product = GetProductWithMaxStock();
            return View(product); // Tek ürün view’ı için
        }

        public IActionResult ProductWithMinStock()
        {
            var product = GetProductWithMinStock();
            return View("ProductDetail", GetProductWithMinStock());
        }

        public IActionResult ProductWithMaxPrice()
        {
            var product = GetProductWithMaxPrice();
            return View("ProductDetail", product);
        }

        public IActionResult ProductWithMinPrice()
        {
            var product = GetProductWithMinPrice();
            return View("ProductDetail", GetProductWithMinPrice());
        }


        public IActionResult OutOfStockProducts()
        {
            var products = DP.Listeleme<Product>("sp_GetOutOfStockProducts");
            return View(products); // Çoklu ürün listesi view’ı
        }

        // Belirli stok seviyesinin altındaki ürünleri getirir
        public IActionResult ProductsBelowStock(int threshold)
        {
            var param = new DynamicParameters();
            param.Add("@Threshold", threshold);

            var products = DP.Listeleme<Product>("sp_GetProductsBelowStock", param);
            return View(products); // Çoklu ürün listesi view’ı
        }



    }
}