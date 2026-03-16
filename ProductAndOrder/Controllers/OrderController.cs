using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;

namespace ProductAndOrder.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Customer")]
	public class OrderController : ControllerBase
	{
		private readonly IOrderDto _orderDto;
		public OrderController(IOrderDto orderDto)
		{
			_orderDto = orderDto;
		}
		[HttpGet]
		public async Task<IActionResult> GetAllOrderAsync()
		{
			var orders = await _orderDto.GetAllOrderAsync();
			return Ok(orders);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetOrderByIdAsync(int id)
		{
			var order = await _orderDto.GetOrderByIdAsync(id);
			if (order == null)
				return NotFound();
			return Ok(order);
		}
		[HttpPost]
		public async Task<IActionResult> AddOrderAsync(CreateOrderDto createorder)
		{
			var order = await _orderDto.AddOrderAsync(createorder);
			return Ok(order);
		}
		[HttpPut]
		public async Task<IActionResult> UpdateOrderAsync(UpdateOrderDto updateorder)
		{
			var result = await _orderDto.UpdateOrderAsync(updateorder);
			if (!result)
				return NotFound();
			return Ok();
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteOrderAsync(int id)
		{
			var result = await _orderDto.DeleteOrderAsync(id);
			if (!result)
				return NotFound();
			return Ok();
		}
	}
}
