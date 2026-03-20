using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Domain.Entities;

namespace ProductAndOrder.Domain.Interfaces
{
	public interface IOrder
	{
		Task<IEnumerable<Order>> GetAllOrderAsync();
		Task<Order> GetOrderByIdAsync(int OrderId);
		Task<Order> AddOrderAsync(Order order);
		Task<bool> DeleteOrderAsync(Order order);
		Task<bool> UpdateOrderAsync(Order order);
		Task<bool> AddProductOrderAsync(ProductOrder productorder);
		Task<bool> UpdateProductOrderAsync(ProductOrder productorder);
		Task<ProductOrder> GetProductOrderByOrderIdAsync(int OrderId);
		Task<List<ProductOrder>> GetAllProductOrder();

	}
}
