using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductAndOrder.Domain.Entities
{
	public class Order
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("User")]
		public int UserId { get; set; }
		public string DiscountType { get; set; }= string.Empty;	
		public decimal SubTotal { get; set; }      
		public decimal DiscountAmount { get; set; }
		public decimal TotalAmount { get; set; }
		public DateTime? OrderDate { get; set; }
		public int OrderStatus { get; set; } = 0;

		
	}
}
