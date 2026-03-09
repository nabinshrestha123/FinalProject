using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;
using ProductAndOrder.Domain.Entities;
using ProductAndOrder.Domain.Enum;
using ProductAndOrder.Domain.Interfaces;

namespace ProductAndOrder.Application.Services
{
	public class OrderService: IOrderDto
	{    private readonly IOrder _Order;
		private readonly IUserServiceClient _UserServiceClient;
		public OrderService(IOrder order, IUserServiceClient UserServiceClient) { 
			_Order = order;
			_UserServiceClient = UserServiceClient;
		}
		public async Task<IEnumerable<OrderDto>> GetAllOrderAsync()
		{
			var orders= await _Order.GetAllOrderAsync();
			return orders.Select(o => new OrderDto
			{
              Id = o.Id,	
			  OrderDate = o.OrderDate,
			  OrderStatus= (Domain.Enum.OrderStatus)o.OrderStatus,
			  TotalAmount = o.TotalAmount,

			}).ToList();

		}
		public async Task<ExecutionResult<OrderDto>> GetOrderByIdAsync(int id)
		{
			var orders = await _Order.GetOrderByIdAsync(id);
			if (orders == null)
				return new ExecutionResult<OrderDto>()
				{
					Data = null,
					Message = "OrderId not found",
					Status = ResponseStatus.BadRequest
				};
			var customerDetail = await _UserServiceClient.GetUserAsync(orders.UserId);
			if (customerDetail == null)
				return new ExecutionResult<OrderDto>()
				{
					Data = null,
					Message = "Customer not found",
					Status = ResponseStatus.BadRequest
				};

			var result = new OrderDto
			{
				Id = orders.Id,
				OrderDate = orders.OrderDate,
				OrderStatus = (Domain.Enum.OrderStatus)orders.OrderStatus,
				TotalAmount = orders.TotalAmount,
				CustomerName = customerDetail.Name
			};
			return new ExecutionResult<OrderDto>()
			{
				Data = result,
				Message = "OrderId found",
				Status = ResponseStatus.Ok
			};
		}

		public async Task<OrderDto> AddOrderAsync(CreateOrderDto createorder)
		{
			var order = new Order
			{
				UserId = createorder.UserId,
				OrderDate = createorder.OrderDate,
				OrderStatus = (int)(OrderStatus)createorder.OrderStatus,
				TotalAmount = createorder.TotalAmount,
			};
			var createdOrder = await _Order.AddOrderAsync(order);
			return new OrderDto
			{
				Id = createdOrder.Id,
				OrderDate = createdOrder.OrderDate,
				OrderStatus = (Domain.Enum.OrderStatus)createdOrder.OrderStatus,
				TotalAmount = createdOrder.TotalAmount,
			};	

		}
		public async Task<bool> DeleteOrderAsync(int id)
		{
			var dltorder = await _Order.GetOrderByIdAsync(id);	
			if (dltorder == null)
				return false;
			await _Order.DeleteOrderAsync(dltorder);
			return true;

		}
		public async Task<bool> UpdateOrderAsync(UpdateOrderDto updateorder)
		{
			var order = await _Order.GetOrderByIdAsync(updateorder.Id);
			if (order == null)
				return false;
			order.OrderDate = updateorder.OrderDate;
		    order.OrderStatus = (int)(OrderStatus)updateorder.OrderStatus;
			order.TotalAmount = updateorder.TotalAmount;

			await _Order.UpdateOrderAsync(order);
			return true;


		}

	}
}
