using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;
using ProductAndOrder.Domain.Entities;
using ProductAndOrder.Domain.Interfaces;


namespace ProductAndOrder.Application.Services
{
	public class ProductService: IProductDto
	{
		private readonly IProduct _Product;
		 public ProductService(IProduct product)
		{
			_Product= product;
		}
		public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
		{
			var products= await _Product.GetAllProductsAsync();

			var productread = products.Select(p => new ProductDto
			{
				Id = p.Id,
				ProductName = p.ProductName,
				Price = p.Price

			}).ToList();
			return productread;

		}
		public async Task<ExecutionResult<ProductDto>> GetProductByIdAsync(int id)
		{
			var productreadbyid = await _Product.GetProductByIdAsync(id);

			if (productreadbyid == null)
			{
				return new ExecutionResult<ProductDto>
				{
					Data= null,
					Message = "Product not found",
					Status = ResponseStatus.BadRequest,
				};
			}
			var result =  new ProductDto
			{
				Id= productreadbyid.Id,
				ProductName= productreadbyid.ProductName,
				Price = productreadbyid.Price
			};
			return new ExecutionResult<ProductDto>
			{
				Data = result,
				Message = "Product found",
				Status = ResponseStatus.Ok,
			};
		}

		public async Task<ProductDto> AddProductAsync(CreateProductDto createproduct)
		{
			var product = new Product
			{
				ProductName = createproduct.ProductName,
				Price = createproduct.Price
			};
			var addedProduct= await _Product.AddProductAsync(product);

			return new ProductDto
			{
				Id = addedProduct.Id,
				ProductName = addedProduct.ProductName,
				Price = addedProduct.Price
			};


		}
		public async Task<bool> DeleteProductAsync(int id)
		{
			var productToDelete = await _Product.GetProductByIdAsync(id);
			if (productToDelete == null)
			{
				return false;
			}
			var dltproduct=await _Product.DeleteProductAsync(productToDelete);
			return true;
		}
		public async Task<bool> UpdateProductAsync(UpdateProductDto updateproduct)
		{
           var productToUpdate= await _Product.GetProductByIdAsync(updateproduct.Id);
			if (productToUpdate == null)
				{
				return false;
			}
			productToUpdate.Id = updateproduct.Id;
			productToUpdate.ProductName = updateproduct.ProductName;
			productToUpdate.Price = updateproduct.Price;

			var updatedProduct = await _Product.UpdateProductAsync(productToUpdate);
			return true;

		}

	}
}
