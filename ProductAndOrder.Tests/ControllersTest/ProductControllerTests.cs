using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NPOI.SS.Formula.Functions;
using ProductAndOrder.Api.Controllers;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;

namespace ProductAndOrder.Tests.Controllers
{
	public class ProductControllerTests
	{
	
		private readonly Mock<IProductDto> _mockService;
		private readonly ProductController _controller;

		public ProductControllerTests()
		{
	
			_mockService = new Mock<IProductDto>();
			_controller = new ProductController(_mockService.Object);
		}


		[Fact]
		public async Task GetAllProductAsync_ShouldReturn200_WhenProductsExist()
		{
			// ARRANGE
			// These are the fake products the fake service will return
			var fakeProducts = new List<ProductDto>
			{
				new ProductDto { Id = 1, ProductName = "Laptop", Price = 1000 },
				new ProductDto { Id = 2, ProductName = "Phone",  Price = 500  }
			};

			_mockService
				.Setup(s => s.GetAllProductsAsync())
				.ReturnsAsync(fakeProducts);

			// ACT
			// Call the real controller method
			var response = await _controller.GetAllProductAsync() as OkObjectResult;

			// ASSERT
			response.Should().NotBeNull();
			response!.StatusCode.Should().Be(200);
			response.Value.Should().BeEquivalentTo(fakeProducts); 
		}

		[Fact]
		public async Task GetAllProductAsync_ShouldReturn200WithEmpty_WhenNoProductsExist()
		{
			// ARRANGE
			// Return empty list — no products in DB
			_mockService
				.Setup(s => s.GetAllProductsAsync())
				.ReturnsAsync(new List<ProductDto>());

			// ACT
			var response = await _controller.GetAllProductAsync() as OkObjectResult;

			// ASSERT
			// Controller still returns 200 even with empty list
			// because empty is not an error
			response.Should().NotBeNull();
			response!.StatusCode.Should().Be(200);

		
		}

		[Fact]
		public async Task GetProductByIdAsync_ShouldReturn200_WhenProductExists()
		{
			// ARRANGE
			// Fake ExecutionResult the service will return
			var fakeResult = new ExecutionResult<ProductDto>
			{
				Data = new ProductDto { Id = 1, ProductName = "Laptop", Price = 1000 },
				Message = "Product found",
				Status = ResponseStatus.Ok
			};

			// Tell mock: when GetProductByIdAsync(1) is called,
			// return fakeResult
			_mockService
				.Setup(s => s.GetProductByIdAsync(1))
				.ReturnsAsync(fakeResult);

			// ACT
			var response = await _controller.GetProductByIdAsync(1) as OkObjectResult;
			
			// ASSERT
			response.Should().NotBeNull();
			response!.StatusCode.Should().Be(200);    
			response.Value.Should().BeEquivalentTo(fakeResult); // correct data
		}

		[Fact]
		public async Task GetProductByIdAsync_ShouldReturn404_WhenProductNotFound()
		{
			// ARRANGE
			// Return null to simulate product not found
			_mockService
				.Setup(s => s.GetProductByIdAsync(99))
				.ReturnsAsync((ExecutionResult<ProductDto>?)null);

			// ACT
			var response = await _controller.GetProductByIdAsync(99) as NotFoundResult;
		

			// ASSERT
			response.Should().NotBeNull();
			response!.StatusCode.Should().Be(404);    // 404 Not Found
		}


		[Fact]
		public async Task AddProductAsync_ShouldReturn200_WhenProductIsAdded()
		{
			// ARRANGE
			// This is what user sends
			var createDto = new CreateProductDto
			{
				ProductName = "Tablet",
				Price = 700
			};

			// This is what the fake service returns after adding
			var addedProduct = new ProductDto
			{
				Id = 5,                  
				ProductName = "Tablet",
				Price = 700
			};

			// Tell mock: when AddProductAsync is called with
			// any CreateProductDto, return addedProduct
			_mockService
				.Setup(s => s.AddProductAsync(It.IsAny<CreateProductDto>()))
				.ReturnsAsync(addedProduct);

			// ACT
			var response = await _controller.AddProductAsync(createDto) as OkObjectResult;

			// ASSERT
			response.Should().NotBeNull();
			response!.StatusCode.Should().Be(200);              // 200 OK
			response.Value.Should().BeEquivalentTo(addedProduct); // correct data
		}


		[Fact]
		public async Task UpdateProductAsync_ShouldReturn200_WhenProductIsUpdated()
		{
			// ARRANGE
			var updateDto = new UpdateProductDto
			{
				Id = 1,
				ProductName = "New Laptop",
				Price = 1200
			};
	

			// Tell mock: when UpdateProductAsync is called,
			// return true = pretend that update was successful
			_mockService
				.Setup(s => s.UpdateProductAsync(It.IsAny<UpdateProductDto>()))
				.ReturnsAsync(true);

			// ACT
			var response = await _controller.UpdateProductAsync(updateDto) as OkResult;
			
			response.Should().NotBeNull();
			response!.StatusCode.Should().Be(200);   
		}

		[Fact]
		public async Task UpdateProductAsync_ShouldReturn404_WhenProductNotFound()
		{
			// ARRANGE
			var updateDto = new UpdateProductDto { Id = 99, ProductName = "Test",Price=900};

			// Tell mock: return false = product not found
			_mockService
				.Setup(s => s.UpdateProductAsync(It.IsAny<UpdateProductDto>()))
				.ReturnsAsync(false);

			// ACT
			var response = await _controller.UpdateProductAsync(updateDto) as NotFoundResult;

			// ASSERT
			response.Should().NotBeNull();
			response!.StatusCode.Should().Be(404);   // 404 Not Found
		}

		// ════════════════════════════════════════════════════════════

		[Fact]
		public async Task DeleteProductAsync_ShouldReturn200_WhenProductIsDeleted()
		{
			// ARRANGE
			
			_mockService
				.Setup(s => s.DeleteProductAsync(1))
				.ReturnsAsync(true);

			// ACT
			var response = await _controller.DeleteProductAsync(1) as OkResult;
		
			// ASSERT
			response.Should().NotBeNull();
			response!.StatusCode.Should().Be(200);   // 200 OK
		}

		[Fact]
		public async Task DeleteProductAsync_ShouldReturn404_WhenProductNotFound()
		{
			// ARRANGE
			// Tell mock: return false = product not found
			_mockService
				.Setup(s => s.DeleteProductAsync(99))
				.ReturnsAsync(false);

			// ACT
			var response = await _controller.DeleteProductAsync(99) as NotFoundResult;

			// ASSERT
			response.Should().NotBeNull();
			response!.StatusCode.Should().Be(404);   // 404 Not Found
		}
	}
}
//Step by Step — What Happens Internally
//ARRANGE
//csharp_mockService
//    .Setup(s => s.DeleteProductAsync(1))
//    .ReturnsAsync(true);
//```
//At this point nothing has run yet.You are just giving the fake service its instructions:
//```
//"Hey fake service, I am telling you in advance:
// IF someone calls DeleteProductAsync with id = 1,
// you must return TRUE"
//Think of it like writing a script for an actor before filming starts.

//ACT
//csharpvar response = await _controller.DeleteProductAsync(1) as OkResult;
//```

//This ONE line triggers a whole chain internally. Here is exactly what happens:

//### → Step 1: Test calls the real controller
//```
//Test says: "Controller, delete product with id = 1"
//→ Step 2: Controller calls the fake service
//csharpvar result = await _productDto.DeleteProductAsync(1);
//```
//```
//Controller says to fake service: "Delete product 1"
//Fake service checks its instructions...
//Fake service finds: "Oh! I was told to return TRUE for id = 1"
//Fake service returns: TRUE
//result = true
//→ Step 3: Controller checks the result
//csharpif (!result)          // if(!true) = if(false)
//    return NotFound(); // ← this is SKIPPED because result is true
//```
//```
//Controller checks: is result false?
//Answer: NO, result is true
//So NotFound() is SKIPPED
//→ Step 4: Controller returns Ok()
//csharpreturn Ok(); // ← this runs
//```
//```
//Controller says: "Delete was successful, sending back 200 OK"
//→ Step 5: Cast to OkResult
//csharpas OkResult
//```
//```
//The Ok() response comes back to the test
//Test casts it to OkResult so it can inspect it
//response = OkResult { StatusCode = 200 }

//ASSERT
//csharpresponse.Should().NotBeNull();
//```
//```
//Checks: did the cast work? Is response actually an OkResult?
//Answer: YES ✅
//csharpresponse!.StatusCode.Should().Be(200);
//```
//```
//Checks: is the status code 200?
//Answer: YES ✅ → TEST PASSES
//```