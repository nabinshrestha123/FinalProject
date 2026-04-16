using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

		// ✅ GET ALL ORDERS
		[HttpGet]
		[Authorize(Roles = "Customer,Admin")]
		public async Task<IActionResult> GetAllOrderAsync()
		{
			var orders = await _orderDto.GetAllOrderAsync();
			return Ok(orders);
		}

		// ✅ GET ORDER BY ID
		[HttpGet("{id}")]
		[Authorize(Roles = "Customer,Admin")]
		public async Task<IActionResult> GetOrderByIdAsync(int id)
		{
			var result = await _orderDto.GetOrderByIdAsync(id);

			if (result == null || result.Data == null)
				return NotFound(result);

			return Ok(result);
		}

		// ✅ CREATE ORDER (FIXED)
		[HttpPost]
		[Authorize(Roles = "Customer")]
		public async Task<IActionResult> AddOrderAsync(CreateOrderDto createorder)
		{
			// 🔐 Get User Id from JWT
			var userIdClaim = User.FindFirst("Id");

			if (userIdClaim == null)
				return Unauthorized("User Id not found in token");

			int actionBy = int.Parse(userIdClaim.Value);

			var result = await _orderDto.AddOrderAsync(createorder, actionBy);

			return Ok(result);
		}

		// ✅ UPDATE ORDER
		[HttpPut]
		[Authorize(Roles = "Customer")]
		public async Task<IActionResult> UpdateOrderAsync(UpdateOrderDto updateorder)
		{
			var result = await _orderDto.UpdateOrderAsync(updateorder);
			return Ok(result);
		}

		// ✅ DELETE ORDER
		[HttpDelete("{id}")]
		[Authorize(Roles = "Customer")]
		public async Task<IActionResult> DeleteOrderAsync(int id)
		{
			var result = await _orderDto.DeleteOrderAsync(id);

			if (!result)
				return NotFound("Order not found");

			return Ok("Order deleted successfully");
		}
	}
}