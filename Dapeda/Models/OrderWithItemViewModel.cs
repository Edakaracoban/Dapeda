using System;
namespace Dapeda.Models
{
	public class OrderWithItemViewModel
	{
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public string UserName { get; set; }
        public string ProductName { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

