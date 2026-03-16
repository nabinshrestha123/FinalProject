using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProductAndOrder.Domain.Entities;
using ProductAndOrder.Domain.Interfaces;
using ProductAndOrder.Infrastructure.Data;

namespace ProductAndOrder.Infrastructure.Repository
{
	public class ProductRepository(AppDBContext context) : IProduct
	{
		public async Task<IEnumerable<product>> GetAllProductsAsync()
		{
			return await context.Products.ToListAsync();

		}
		public async Task<product> GetProductByIdAsync(int id)
		{
			return await context.Products.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<product> AddProductAsync(product product)
		{
			context.Products.Add(product);
			await context.SaveChangesAsync();
			return product;

		}
		public async Task<bool> DeleteProductAsync(product product)
		{
			context.Products.Remove(product);
			await context.SaveChangesAsync();	
			return true;
		}
		public async Task<bool> UpdateProductAsync(product product)
		{
        context.Products.Update(product);	
			await context.SaveChangesAsync();	
			return true;
		}
	}
}
