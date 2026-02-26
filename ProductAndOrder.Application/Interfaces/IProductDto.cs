using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Domain.Entities;

namespace ProductAndOrder.Application.Interfaces
{
	public interface IProductDto
	{
		Task<IEnumerable<ProductDto>> GetAllProductsAsync();
		Task<ExecutionResult<ProductDto>> GetProductByIdAsync(int id);

		Task<ProductDto> AddProductAsync(CreateProductDto createproduct);
		Task<bool> DeleteProductAsync(int id);
		Task<bool> UpdateProductAsync(UpdateProductDto updateproduct);
	}
}
