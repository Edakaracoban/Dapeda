using Dapeda.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;

public class CategoryController : Controller
{

    public IActionResult Index(string searchTerm)
    {
        IEnumerable<Category> categories;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var param = new DynamicParameters();
            param.Add("@SearchTerm", searchTerm);
            categories = DP.Listeleme<Category>("sp_SearchCategoriesByName", param);
        }
        else
        {
            categories = DP.Listeleme<Category>("sp_GetCategories");
        }

        if (!categories.Any() && !string.IsNullOrWhiteSpace(searchTerm))
        {
            ViewBag.Mesaj = "Arama kriterlerine uygun kategori bulunamadı.";
        }

        return View(categories);
    }

    public IActionResult AddCategory()
    {
        return View();
    }


    [HttpPost]
    public IActionResult AddCategory(Category model)
    {
        if (ModelState.IsValid)
        {
            var param = new DynamicParameters();
            param.Add("@CategoryName", model.CategoryName);

            DP.ExecuteReturn("sp_AddCategory", param);
            return RedirectToAction("Index");
        }
        return View(model);
    }


    public IActionResult EditCategory(int id)
    {
        var param = new DynamicParameters();
        param.Add("@CategoryId", id);

        var category = DP.GetSingle<Category>("sp_GetCategoryById", param);
        if (category == null) return NotFound();

        return View(category);
    }

  
    [HttpPost]
    public IActionResult EditCategory(Category model)
    {
        if (ModelState.IsValid)
        {
            var param = new DynamicParameters();
            param.Add("@CategoryId", model.CategoryId);
            param.Add("@CategoryName", model.CategoryName);

            DP.ExecuteReturn("sp_UpdateCategory", param);
            return RedirectToAction("Index");
        }
        return View(model);
    }


    public IActionResult DeleteCategory(int id)
    {
        var param = new DynamicParameters();
        param.Add("@CategoryId", id);

        var category = DP.GetSingle<Category>("sp_GetCategoryById", param);
        if (category == null) return NotFound();

        return View(category);
    }


    [HttpPost]
    public IActionResult DeleteCategoryConfirmed(int CategoryId)
    {
        var param = new DynamicParameters();
        param.Add("@CategoryId", CategoryId);

        DP.ExecuteReturn("sp_DeleteCategory", param);
        return RedirectToAction("Index");
    }

    public ActionResult ProductWithCategory()
    {
        var products = DP.Listeleme<ProductWithCategory>("sp_GetProducts").ToList();
        return View(products);
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



}
