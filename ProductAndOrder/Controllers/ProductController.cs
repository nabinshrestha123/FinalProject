using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
		[Authorize(Roles="Admin,Customer")]
		//[Authorize(Roles = "Customer")]
		public async Task<IActionResult> GetAllProductAsync()
		{
			var products = await _productDto.GetAllProductsAsync();
			return Ok(products);
		}
		[HttpGet("{id}")]
		//[Authorize(Roles = "Admin,Customer")]
		//[Authorize(Roles = "Customer")]
		public async Task<IActionResult> GetProductByIdAsync(int id)
		{
			var product = await _productDto.GetProductByIdAsync(id);
			if (product == null)
				return NotFound();
			return Ok(product);
		}
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddProductAsync(CreateProductDto createproduct)
		{
			int actionBy = int.Parse(User.FindFirstValue("Id")); 
			var product = await _productDto.AddProductAsync(createproduct, actionBy);
			return Ok(product);
		}
		[HttpPut]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateProductAsync(UpdateProductDto updateproduct)
		{
			int actionBy = int.Parse(User.FindFirstValue("Id"));
			var result = await _productDto.UpdateProductAsync(updateproduct,actionBy);
			if (!result)
				return NotFound();
			return Ok();
		}
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteProductAsync(int id)
		
		{
			int actionBy = int.Parse(User.FindFirstValue("Id"));

			var result = await _productDto.DeleteProductAsync(id);
			if (!result)
				return NotFound();
			return Ok();
		}
	}
}