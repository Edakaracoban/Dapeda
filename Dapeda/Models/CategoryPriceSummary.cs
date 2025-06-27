using System;
namespace Dapeda.Models
{
	public class CategoryPriceSummary
	{
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public decimal AvgPrice { get; set; }
    }
}

