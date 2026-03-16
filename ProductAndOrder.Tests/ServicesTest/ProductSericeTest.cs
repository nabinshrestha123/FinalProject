using Moq;
using FluentAssertions;
using ProductAndOrder.Application.Services;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Domain.Interfaces;
using ProductAndOrder.Domain.Entities;

namespace ProductAndOrder.Tests.Services
{
	public class ProductServiceTests
	{
		
		private readonly Mock<IProduct> _mockProduct;
		private readonly ProductService _service;

		public ProductServiceTests()
		{
			
			_mockProduct = new Mock<IProduct>();
			_service = new ProductService(_mockProduct.Object);
		}

		[Fact]
		public async Task GetAllProductsAsync_ShouldReturnAllProducts_WhenProductsExist()
		{
			//Arrange
			var fakeProducts = new List<product>
			{
				new product { Id = 1, ProductName = "Laptop", Price = 1000 },
				new product { Id = 2, ProductName = "Phone",  Price = 500  }
			};

			// Tell mock: when GetAllProductsAsync() is called,
			// return fakeProducts instead of hitting real DB
			_mockProduct
				.Setup(r => r.GetAllProductsAsync())
				.ReturnsAsync(fakeProducts);

			// ACT
			// Call the real service method
			var result = await _service.GetAllProductsAsync();

			// ASSERT
			result.Should().NotBeNull();         // something came back
			result.Should().HaveCount(2);        // exactly 2 products
		}

		[Fact]
		public async Task GetAllProductsAsync_ShouldReturnEmpty_WhenNoProductsExist()
		{
			// ARRANGE
			// Tell mock: return an empty list (no products in DB)
			_mockProduct
				.Setup(r => r.GetAllProductsAsync())
				.ReturnsAsync(new List<product>());

			// ACT
			var result = await _service.GetAllProductsAsync();

			// ASSERT
			// Result should be empty, not null
			result.Should().BeEmpty();
		}

		

		[Fact]
		public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
		{
			// ARRANGE
			// This is the fake product our DB will return
			var fakeProduct = new product
			{
				Id = 1,
				ProductName = "Laptop",
				Price = 1000
			};

			// Tell mock: when asked for Id 1, return fakeProduct
			_mockProduct
				.Setup(r => r.GetProductByIdAsync(1))
				.ReturnsAsync(fakeProduct);

			// ACT
			var result = await _service.GetProductByIdAsync(1);

			// ASSERT
			result.Should().NotBeNull();
			result.Status.Should().Be(ResponseStatus.Ok);     // status is Ok
			result.Message.Should().Be("Product found");      // correct message
			result.Data.Should().NotBeNull();                 // data exists
			result.Data!.Id.Should().Be(1);                  // correct Id
			result.Data.ProductName.Should().Be("Laptop");   // correct name
			result.Data.Price.Should().Be(1000);             // correct price
		}

		[Fact]
		public async Task GetProductByIdAsync_ShouldReturnBadRequest_WhenProductNotFound()
		{
			// ARRANGE
			// Tell mock: when asked for Id 99, return null
			// This simulates "product doesn't exist in database"
			_mockProduct
				.Setup(r => r.GetProductByIdAsync(99))
				.ReturnsAsync((product?)null);  // ← how to return null from mock

			// ACT
			var result = await _service.GetProductByIdAsync(99);

			// ASSERT
			// Your service has this if(null) check — we are testing it works
			result.Should().NotBeNull();
			result.Status.Should().Be(ResponseStatus.BadRequest); // status is BadRequest
			result.Message.Should().Be("Product not found");      // correct message
			result.Data.Should().BeNull();                        // no data returned
		}

		// ════════════════════════════════════════════════════════════
		// ADD PRODUCT
		// ════════════════════════════════════════════════════════════

		[Fact]
		public async Task AddProductAsync_ShouldReturnProductDto_WhenProductIsAdded()
		{
			// ARRANGE

			// This is what the user sends to create a product
			var actionBy = 1; // Simulate user with Id=1 is adding the product
			var createDto = new CreateProductDto
			{
				ProductName = "Tablet",
				Price = 700
			};

			// This is what the fake DB returns after saving
			// Notice Id = 5 — database generates the Id
			var savedProduct = new product
			{
				Id = 5,                  // ← DB generated this
				ProductName = createDto.ProductName,
				Price = createDto.Price
			};

		
			_mockProduct
				.Setup(r => r.AddProductAsync(It.IsAny<product>()))
				.ReturnsAsync(savedProduct);
		

			// ACT
			var result = await _service.AddProductAsync(createDto,actionBy);

			// ASSERT
			result.Should().NotBeNull();
			result.Id.Should().Be(5);                // DB generated Id came through
			result.ProductName.Should().Be("Tablet"); // name mapped correctly
			result.Price.Should().Be(700);            // price mapped correctly

			// Verify the repo was actually called once
			// (confirms the save actually happened)
			_mockProduct.Verify(
				r => r.AddProductAsync(It.IsAny<product>()),
				Times.Once
			);
		}

		// ════════════════════════════════════════════════════════════
		// DELETE PRODUCT
		// ════════════════════════════════════════════════════════════

	

		[Fact]
		public async Task DeleteProductAsync_ShouldReturnFalse_WhenProductNotFound()
		{
			// ARRANGE
			// Tell mock: return null = product doesn't exist
			_mockProduct
				.Setup(r => r.GetProductByIdAsync(99))
				.ReturnsAsync((product?)null);

			// ACT
			var result = await _service.DeleteProductAsync(99);

			// ASSERT
			result.Should().BeFalse(); // service returned false = not found

			// MOST IMPORTANT:
			// Verify delete was NEVER called
			// because product didn't exist, we must not try to delete
			_mockProduct.Verify(
				r => r.DeleteProductAsync(It.IsAny<product>()),
				Times.Never
			);
		}

		// ════════════════════════════════════════════════════════════
		// UPDATE PRODUCT
		// ════════════════════════════════════════════════════════════

		[Fact]
		public async Task UpdateProductAsync_ShouldReturnTrue_WhenProductExists()
		{
			// ARRANGE
			var actionBy = 1; // Simulate user with Id=1 is updating the product

			// This is the existing product in the fake DB
			var existingProduct = new product
			{
				Id = 1,
				ProductName = "Old Laptop",  // old name
				Price = 800                  // old price
			};

			// This is the new data user wants to update to
			var updateDto = new UpdateProductDto
			{
				Id = 1,
				ProductName = "New Laptop",  // new name
				Price = 1200                 // new price
			};

			
			_mockProduct
				.Setup(r => r.GetProductByIdAsync(1))
				.ReturnsAsync(existingProduct);

			// Tell mock: when UpdateProductAsync is called,
			// return the updated product
			_mockProduct
				.Setup(r => r.UpdateProductAsync(It.IsAny<product>()))
				.ReturnsAsync(true);
		

			// ACT
			var result = await _service.UpdateProductAsync(updateDto,actionBy);

			// ASSERT
			result.Should().BeTrue(); // success

			// Verify update was actually called once
			_mockProduct.Verify(
				r => r.UpdateProductAsync(It.IsAny<product>()),
				Times.Once
			);
		}

		[Fact]
		public async Task UpdateProductAsync_ShouldReturnFalse_WhenProductNotFound()
		{
			// ARRANGE
			// Tell mock: return null = product doesn't exist
			_mockProduct
				.Setup(r => r.GetProductByIdAsync(99))
				.ReturnsAsync((product?)null);
			var actionBy = 1; // Simulate user with Id=1 is trying to update a non-existent product

			var updateDto = new UpdateProductDto
			{
				Id = 99,              // this Id doesn't exist
				ProductName = "Test",
				Price = 500
			};

			// ACT
			var result = await _service.UpdateProductAsync(updateDto, actionBy);

			// ASSERT
			result.Should().BeFalse(); // not found = false

			// Verify update was NEVER called
			// because product didn't exist
			_mockProduct.Verify(
				r => r.UpdateProductAsync(It.IsAny<product>()),
				Times.Never
			);
		}
	}
}