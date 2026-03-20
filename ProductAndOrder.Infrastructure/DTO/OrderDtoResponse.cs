using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Domain.Enum;

namespace ProductAndOrder.Infrastructure.DTO
{
		public class OrderDtoResponse
		{
			public int Id { get; set; }
			public DateTime? OrderDate { get; set; }
			public OrderStatus OrderStatus { get; set; }
			public int TotalAmount { get; set; }
			public int CustomerId { get; set; }
			public int Quantity { get; set; }
			public string CustomerName { get; set; }

	}
}
