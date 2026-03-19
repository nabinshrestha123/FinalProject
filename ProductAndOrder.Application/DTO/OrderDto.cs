using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Domain.Enum;

namespace ProductAndOrder.Application.DTO
{
	public class OrderDto
	{
		public int Id { get; set; }
		public DateTime? OrderDate { get; set; }
		public OrderStatus OrderStatus { get; set; }
        public int TotalAmount { get; set; }
		public int CustomerId { get; set; }
		public string CustomerName { get; set; }

	}
	public class CreateOrderDto
	{
		public int UserId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public int TotalAmount { get; set; }
		public OrderStatus OrderStatus { get; set; }
		public DateTime? OrderDate { get; set; }
	}
	public class UpdateOrderDto
	{
		public int Id { get; set; }
		public int TotalAmount { get; set; }
		public OrderStatus OrderStatus { get; set; }
		public DateTime? OrderDate { get; set; }
	}
}
