using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ProductAndOrder.Domain.Interfaces;

namespace ProductAndOrder.Domain.Entities
{
	public class ProductOrder
	{
		[Key]
		public int Id { get; set; }  // every table should have a primary key

		[ForeignKey("Order")]
		public int OrderId { get; set; }
		public Order Order { get; set; }  // navigation property

		[ForeignKey("Product")]
		public int ProductId { get; set; }
		public product Product { get; set; }  // navigation property

		public int Quantity { get; set; }
	}
	public class  UpdateProductOrder
	{
		public int Quantity { get; set; }

	}
}
