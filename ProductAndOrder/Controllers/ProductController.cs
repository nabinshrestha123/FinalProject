using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;

namespace ProductAndOrder.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IProductDto _productDto;
		public ProductController(IProductDto productDto)
		{
			_productDto = productDto;
		}
		[HttpGet]
		public async Task<IActionResult> GetAllProductAsync()
		{
			var products = await _productDto.GetAllProductsAsync();
			return Ok(products);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetProductByIdAsync(int id)
		{
			var product = await _productDto.GetProductByIdAsync(id);
			if (product == null)
				return NotFound();
			return Ok(product);
		}
		[HttpPost]
		public async Task<IActionResult> AddProductAsync(CreateProductDto createproduct)
		{
			var product = await _productDto.AddProductAsync(createproduct);
			return Ok(product);
		}
		[HttpPut]
		public async Task<IActionResult> UpdateProductAsync(UpdateProductDto updateproduct)
		{
			var result = await _productDto.UpdateProductAsync(updateproduct);
			if (!result)
				return NotFound();
			return Ok();
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProductAsync(int id)
		{
			var result = await _productDto.DeleteProductAsync(id);
			if (!result)
				return NotFound();
			return Ok();
		}
	}
}