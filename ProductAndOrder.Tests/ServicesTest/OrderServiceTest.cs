using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NPOI.SS.Formula.Functions;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Services;
using ProductAndOrder.Domain.Entities;
using ProductAndOrder.Domain.Enum;
using ProductAndOrder.Domain.Interfaces;

namespace ProductAndOrder.Tests.Services
{
	public class OrderServiceTest
	{
		private readonly Mock<IOrder> _orderMock;
		private readonly OrderService _orderService;

		public OrderServiceTest()
		{
			_orderMock = new Mock<IOrder>();
			_orderService = new OrderService(_orderMock.Object);
		}
		[Fact]
		public async Task GetAllOrderAsync_ShouldReturnAllOrders_WhenOrdersExist()
		{
			var fakeOrders = new List<Order>
			{
				new Order { Id = 1, UserId = 1, OrderDate = DateTime.Now, OrderStatus = 1, TotalAmount = 100 },
				new Order { Id = 2, UserId = 2, OrderDate = DateTime.Now, OrderStatus = 2, TotalAmount = 200 }

			};
			_orderMock
					.Setup(r => r.GetAllOrderAsync())
					.ReturnsAsync(fakeOrders);

			var result = await _orderService.GetAllOrderAsync();

			result.Should().NotBeNull();

		}
		[Fact]
		public async Task GetOrderByIdAsync_ShouldReturnBadRequest_WhenOrderNotFound()
		{
			// ARRANGE
			// Tell mock: when asked for id 99, return NULL
			// This simulates "order doesn't exist in database"
			_orderMock
				.Setup(r => r.GetOrderByIdAsync(99))
				.ReturnsAsync((Order?)null);
			// ↑ this is how you tell mock to return null

			// ACT
			var result = await _orderService.GetOrderByIdAsync(99);

			// ASSERT
			result.Status.Should().Be(ResponseStatus.BadRequest);
			result.Data.Should().BeNull();
			result.Message.Should().Be("OrderId not found");
		}
		[Fact]
		public async Task DeleteOrderAsync_ShouldReturnFalse_WhenOrderNotFound()
		{
			// ARRANGE — return null to simulate not found
			_orderMock
				.Setup(r => r.GetOrderByIdAsync(99))
				.ReturnsAsync((Order?)null);

			// ACT
			var result = await _orderService.DeleteOrderAsync(99);

			// ASSERT
			result.Should().BeFalse();
		}
		[Fact]
		public async Task AddOrderAsync_ShouldReturnOrderDto_WhenOrderIsCreated()
		{
			// ── ARRANGE ──────────────────────────────────────────────────

			// This is the INPUT you are sending to the service
			// Just like a user filling in a form
			var createDto = new CreateOrderDto
			{
				UserId = 1,
				TotalAmount = 300,
				OrderDate = DateTime.Now,
				OrderStatus = OrderStatus.Pending
			};

			// This is what the FAKE DATABASE will return after saving
			// Notice it has an Id = 10, because database generates the ID
			var savedOrder = new Order
			{
				Id = 10,                        // ← DB generated this
				UserId = 1,
				TotalAmount = createDto.TotalAmount,
				OrderDate = createDto.OrderDate,
				OrderStatus = (int)OrderStatus.Pending
			};

			// Tell the mock:
			// "When AddOrderAsync is called with ANY Order object,
			//  pretend you saved it and return savedOrder"
			//
			
			_orderMock
				.Setup(r => r.AddOrderAsync(It.IsAny<Order>()))
				.ReturnsAsync(savedOrder);

			// ── ACT ──────────────────────────────────────────────────────

			// Call the real service method with our input
			var result = await _orderService.AddOrderAsync(createDto);

			// ── ASSERT ───────────────────────────────────────────────────

			// Was something returned at all?
			result.Should().NotBeNull();

			// Did the returned OrderDto have the correct Id from DB?
			result.Id.Should().Be(10);

			// Did the TotalAmount map correctly?
			result.TotalAmount.Should().Be(300);

			// Was AddOrderAsync on the repo actually called once?
			_orderMock.Verify(
				r => r.AddOrderAsync(It.IsAny<Order>()),
				Times.Once  // ← confirm the save actually happened
			);
		}

[Fact]
public async Task UpdateOrderAsync_ShouldReturnTrue_WhenOrderExists()
{
    // ── ARRANGE ──────────────────────────────────────────────────

    // This is the existing order already in the "database"
    var existingOrder = new Order
    {
        Id = 1,
        TotalAmount = 100,              // old value
        OrderStatus = (int)OrderStatus.Pending,  // old status
        OrderDate = DateTime.Now.AddDays(-1)     // old date
    };

    // This is the new data the user wants to update to
    var updateDto = new UpdateOrderDto
    {
        Id = 1,
        TotalAmount = 500,              // new value
        OrderStatus = OrderStatus.Completed,  // new status
        OrderDate = DateTime.Now              // new date
    };

    // Tell mock: "When looking for order with Id 1,
    //             return existingOrder"
    _orderMock
        .Setup(r => r.GetOrderByIdAsync(1))
        .ReturnsAsync(existingOrder);

			// Tell mock: "When UpdateOrderAsync is called,
			//             pretend it saved successfully"
			_orderMock
			   .Setup(r => r.UpdateOrderAsync(It.IsAny<Order>()))
			   .ReturnsAsync(true);


			// ── ACT ──────────────────────────────────────────────────────

			var result = await _orderService.UpdateOrderAsync(updateDto);

    // ── ASSERT ───────────────────────────────────────────────────

    // Service should return true (success)
    result.Should().BeTrue();

			// Confirm UpdateOrderAsync was actually called once
			_orderMock.Verify(
        r => r.UpdateOrderAsync(It.IsAny<Order>()),
        Times.Once
    );
}
	}


}