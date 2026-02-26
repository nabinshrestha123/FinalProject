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
		public async Task<IEnumerable<Product>> GetAllProductsAsync()
		{
			return await context.Products.ToListAsync();

		}
		public async Task<Product> GetProductByIdAsync(int id)
		{
			return await context.Products.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<Product> AddProductAsync(Product product)
		{
			context.Products.Add(product);
			await context.SaveChangesAsync();
			return product;

		}
		public async Task<bool> DeleteProductAsync(Product product)
		{
			context.Products.Remove(product);
			await context.SaveChangesAsync();	
			return true;
		}
		public async Task<bool> UpdateProductAsync(Product product)
		{
        context.Products.Update(product);	
			await context.SaveChangesAsync();	
			return true;
		}
	}
}
