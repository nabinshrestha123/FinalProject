using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Domain.Entities;
using ProductAndOrder.Domain.Enum;
using ProductAndOrder.Domain.Interfaces;
using ProductAndOrder.Infrastructure.Data;

namespace ProductAndOrder.Infrastructure.Repository
{
	public class OrderRepository(AppDBContext context) : IOrder
	{
		public async Task<IEnumerable<Order>> GetAllOrderAsync()
		{
			var result = await context.Orders.ToListAsync();
			return result;
		}

		public async Task<Order> GetOrderByIdAsync(int Id)
		{
			return await context.Orders.FirstOrDefaultAsync(o => o.Id == Id);
		}

		public async Task<Order> AddOrderAsync(Order order)
		{
			context.Orders.Add(order);
			await context.SaveChangesAsync();
			return order;
		}
		public async Task<bool> DeleteOrderAsync(Order order)
		{
			context.Orders.Remove(order);
			await context.SaveChangesAsync();
			return true;

		}
		public async Task<bool> UpdateOrderAsync(Order order)
		{
			context.Orders.Update(order);
			await context.SaveChangesAsync();
			return true;

		}

		public async Task<bool> AddProductOrderAsync(ProductOrder productorder)
		{
			context.ProductOrders.Add(productorder);
			var id = await context.SaveChangesAsync();
			return id > 0;
		}

		public async Task<bool> UpdateProductOrderAsync(ProductOrder productorders)
		{
			context.ProductOrders.Update(productorders);
			await context.SaveChangesAsync();
			return true;

		}

		public Task<ProductOrder> GetProductOrderByOrderIdAsync(int OrderId)
		{
			return context.ProductOrders.FirstOrDefaultAsync(po => po.OrderId == OrderId);




		}

		public Task<List<ProductOrder>> GetAllProductOrder()
		{
			var result= context.ProductOrders.ToListAsync();
			return result;
			
		}
	}
}
