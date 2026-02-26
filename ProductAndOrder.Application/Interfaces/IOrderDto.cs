using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Domain.Entities;

namespace ProductAndOrder.Application.Interfaces
{
	public interface IOrderDto
	{
		Task<IEnumerable<OrderDto>> GetAllOrderAsync();
		Task<ExecutionResult<OrderDto>> GetOrderByIdAsync(int id);

		Task<OrderDto> AddOrderAsync(CreateOrderDto createorder);
		Task<bool> DeleteOrderAsync(int id);
		Task<bool> UpdateOrderAsync(UpdateOrderDto  updateorder);

	}
}
