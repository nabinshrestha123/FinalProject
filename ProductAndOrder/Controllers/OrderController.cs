using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;

namespace ProductAndOrder.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	
	public class OrderController : ControllerBase
	{
		private readonly IOrderDto _orderDto;
		public OrderController(IOrderDto orderDto)
		{
			_orderDto = orderDto;
		}
		
		[HttpGet]
		[Authorize(Roles = "Customer,Admin")]
		public async Task<IActionResult> GetAllOrderAsync()
		{
			var orders = await _orderDto.GetAllOrderAsync();
			return Ok(orders);
		}
	
		[HttpGet("{id}")]
		[Authorize(Roles = "Customer,Admin")]
		public async Task<IActionResult> GetOrderByIdAsync(int id)
		{
			var order = await _orderDto.GetOrderByIdAsync(id);
			if (order == null)
				return NotFound();
			return Ok(order);
		}
	

		[HttpPost]
		[Authorize(Roles = "Customer")]
		public async Task<ExecutionResult<int>> AddOrderAsync(CreateOrderDto createorder)
		{
			int actionBy = int.Parse(User.FindFirstValue("Id"));
			var order = await _orderDto.AddOrderAsync(createorder,actionBy);
			return order;
		}
	
		[HttpPut]
		[Authorize(Roles = "Customer")]
		public async Task<IActionResult> UpdateOrderAsync(UpdateOrderDto updateorder)
		{
			var result = await _orderDto.UpdateOrderAsync(updateorder);
			return Ok(result);
		}
		
		[HttpDelete("{id}")]
		[Authorize(Roles = "Customer")]
		public async Task<IActionResult> DeleteOrderAsync(int id)
		{
			var result = await _orderDto.DeleteOrderAsync(id);
			if (!result)
				return NotFound();
			return Ok();
		}
	}
}
