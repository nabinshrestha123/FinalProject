using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Domain.Entities;

namespace ProductAndOrder.Domain.Interfaces
{
	public interface IProduct
	{
		Task <IEnumerable<Product>> GetAllProductsAsync();
		Task <Product> GetProductByIdAsync(int id);

		Task <Product> AddProductAsync(Product product);
		Task <bool> DeleteProductAsync(Product product);
		Task<bool> UpdateProductAsync(Product product);
	}
}
