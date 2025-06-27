using System;
namespace Dapeda.Models
{
    public class ProductWithCategory
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryName { get; set; }
    }
}

