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

		Task<ExecutionResult<int>> AddOrderAsync(CreateOrderDto createorder, int actionBy);
		Task<bool> DeleteOrderAsync(int id);
		Task<ExecutionResult<bool>> UpdateOrderAsync(UpdateOrderDto  updateorder);

	}
}
