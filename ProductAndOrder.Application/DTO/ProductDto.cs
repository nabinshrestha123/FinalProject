using System;
using System.Collections.Generic;
using System.Text;

namespace ProductAndOrder.Application.DTO
{
	public class ProductDto
	{
		public int Id { get; set; }
		public string? ProductName { get; set; }
		public decimal? Price { get; set; }
	


	}
	public class CreateProductDto
	{   
		public string? ProductName { get; set; }
		public decimal? Price { get; set; }

		

	
	}
	public class UpdateProductDto
	{
		public int Id { get; set; }
		public string? ProductName { get; set; }
		public int? Price { get; set; }


	}
}
