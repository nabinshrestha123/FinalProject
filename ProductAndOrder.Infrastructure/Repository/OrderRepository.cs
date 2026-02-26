using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Domain.Entities;
using ProductAndOrder.Domain.Interfaces;
using ProductAndOrder.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ProductAndOrder.Infrastructure.Repository
{
	public class OrderRepository(AppDBContext context): IOrder {
		public async Task<IEnumerable<Order>> GetAllOrderAsync()
		{
			return await context.Orders.ToListAsync();	

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
	}
}
