using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Domain.Entities;

namespace ProductAndOrder.Domain.Interfaces
{
	public interface IProduct
	{
		Task <IEnumerable<product>> GetAllProductsAsync();
		Task <product> GetProductByIdAsync(int id);

		Task <product> AddProductAsync(product product);
		Task <bool> DeleteProductAsync(product product);
		Task<bool> UpdateProductAsync(product product);
	}
}
