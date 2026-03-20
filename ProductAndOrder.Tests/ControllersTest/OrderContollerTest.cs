//using System.Net.NetworkInformation;
//using FluentAssertions;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using ProductAndOrder.Api.Controllers;
//using ProductAndOrder.Application.DTO;
//using ProductAndOrder.Application.Interfaces;
//using ProductAndOrder.Domain.Entities;
//using ProductAndOrder.Domain.Enum;

//namespace ProductAndOrder.Tests.Controllers
//{
//	public class OrderControllerTests
//	{
		
//		private readonly Mock<IOrderDto> _mockService;
//		private readonly OrderController _controller;

//		public OrderControllerTests()
//		{
			
//			_mockService = new Mock<IOrderDto>();
//			_controller = new OrderController(_mockService.Object);
//		}


//		[Fact]
//		public async Task GetAllOrderAsync_ShouldReturn200_WhenOrdersExist()
//		{
			
//			var fakeOrders = new List<OrderDto>
//			{
//				new OrderDto { Id = 1, TotalAmount = 100, OrderStatus = OrderStatus.Pending },
//				new OrderDto { Id = 2, TotalAmount = 200, OrderStatus = OrderStatus.Completed }
//			};

			
//			_mockService
//				.Setup(s => s.GetAllOrderAsync())
//				.ReturnsAsync(fakeOrders);

			
//			var response = await _controller.GetAllOrderAsync() as OkObjectResult;

			
//			response.Should().NotBeNull();
//			response!.StatusCode.Should().Be(200);
//			response.Value.Should().BeEquivalentTo(fakeOrders);
//		}

//		[Fact]
//		public async Task GetAllOrderAsync_ShouldReturn200WithEmpty_WhenNoOrdersExist()
//		{
//			_mockService
//				.Setup(s => s.GetAllOrderAsync())
//				.ReturnsAsync(new List<OrderDto>());

			
//			var response = await _controller.GetAllOrderAsync() as OkObjectResult;

			
//			response.Should().NotBeNull();
//			response!.StatusCode.Should().Be(200);

//			var data = response.Value as IEnumerable<OrderDto>;
//			data.Should().BeEmpty();
//		}


//		[Fact]
//		public async Task GetOrderByIdAsync_ShouldReturn200_WhenOrderExists()
//		{
			
//			var fakeResult = new ExecutionResult<OrderDto>
//			{
//				Data = new OrderDto
//				{
//					Id = 1,
//					TotalAmount = 100,
//					OrderStatus = OrderStatus.Pending,
//					OrderDate = DateTime.Now
//				},
//				Message = "OrderId found",
//				Status = ResponseStatus.Ok
//			};

			
//			_mockService
//				.Setup(s => s.GetOrderByIdAsync(1))
//				.ReturnsAsync(fakeResult);

		
//			var response = await _controller.GetOrderByIdAsync(1) as OkObjectResult;

//			response.Should().NotBeNull();
//			response!.StatusCode.Should().Be(200);             
//			response.Value.Should().BeEquivalentTo(fakeResult); 
//		}

//		[Fact]
//		public async Task GetOrderByIdAsync_ShouldReturn404_WhenOrderNotFound()
//		{
			
//			_mockService
//				.Setup(s => s.GetOrderByIdAsync(99))
//				.ReturnsAsync((ExecutionResult<OrderDto>?)null);

			
//			var response = await _controller.GetOrderByIdAsync(99) as NotFoundResult;

			
//			response.Should().NotBeNull();
//			response!.StatusCode.Should().Be(404);             
//		}

//		[Fact]
//		public async Task AddOrderAsync_ShouldReturn200_WhenOrderIsAdded()
//		{
			
//			var createDto = new CreateOrderDto
//			{
				
//				TotalAmount = 300,
//				OrderDate = DateTime.Now,
//				OrderStatus = OrderStatus.Pending
//			};

//			// This is what the fake service returns after adding
//			var addedOrder = new OrderDto
//			{
//				Id = 10,              
//				TotalAmount = 300,
//				OrderStatus = OrderStatus.Pending,
//				OrderDate = createDto.OrderDate
//			};
//			var result = new ExecutionResult<OrderDto>
//			{
//				Data = addedOrder,
//				Message = "Order added successfully",
//				Status = ResponseStatus.Ok
//			};

//			_mockService
//				.Setup(s => s.AddOrderAsync(It.IsAny<CreateOrderDto>()))
//				.ReturnsAsync(result);

			
//			var response = await _controller.AddOrderAsync(createDto);

			
//			response.Should().NotBeNull();
	
//		}

	

//		[Fact]
//		public async Task UpdateOrderAsync_ShouldReturn200_WhenOrderIsUpdated()
//		{
			
//			var updateDto = new UpdateOrderDto
//			{
//				Id = 1,
//				TotalAmount = 500,
//				OrderStatus = OrderStatus.Completed,
//				OrderDate = DateTime.Now
//			};
//			var result = new ExecutionResult<Order>
//			{
//				Data = updateDto,
//				Message = "Order added successfully",
//				Status = ResponseStatus.Ok
//			};


//			_mockService
//				.Setup(s => s.UpdateOrderAsync(It.IsAny<UpdateOrderDto>()))
//				.ReturnsAsync(true);

			
//			var response = await _controller.UpdateOrderAsync(updateDto) as OkResult;

//			// ASSERT
//			response.Should().NotBeNull();
//			response!.StatusCode.Should().Be(200);             
//		}

//		[Fact]
//		public async Task UpdateOrderAsync_ShouldReturn404_WhenOrderNotFound()
//		{
			
//			var updateDto = new UpdateOrderDto 
//			{ Id = 99,
//			TotalAmount = 500
//			};

		
//			_mockService
//				.Setup(s => s.UpdateOrderAsync(It.IsAny<UpdateOrderDto>()))
//				.ReturnsAsync(false);

			
//			var response = await _controller.UpdateOrderAsync(updateDto) as NotFoundResult;

//			// ASSERT
//			response.Should().NotBeNull();
//			response!.StatusCode.Should().Be(404);             // 404 Not Found
//		}

	

//		[Fact]
//		public async Task DeleteOrderAsync_ShouldReturn200_WhenOrderIsDeleted()
//		{
			
//			_mockService
//				.Setup(s => s.DeleteOrderAsync(1))
//				.ReturnsAsync(true);

			
//			var response = await _controller.DeleteOrderAsync(1) as OkResult;

			
//			response.Should().NotBeNull();
//			response!.StatusCode.Should().Be(200);             // 200 OK
//		}

//		[Fact]
//		public async Task DeleteOrderAsync_ShouldReturn404_WhenOrderNotFound()
//		{
			
//			_mockService
//				.Setup(s => s.DeleteOrderAsync(99))
//				.ReturnsAsync(false);

			
//			var response = await _controller.DeleteOrderAsync(99) as NotFoundResult;

			
//			response.Should().NotBeNull();
//			response!.StatusCode.Should().Be(404);             
//		}
//	}
//}
