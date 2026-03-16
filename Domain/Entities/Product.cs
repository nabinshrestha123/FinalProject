using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductAndOrder.Domain.Entities
{
public class product
	{
		[Key]
		public int Id { get; set; }
		public string? ProductName{ get; set; }
		public int? Price { get; set; }

		public int? Stock { get; set; }
		public int CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }

	
		

	}
}
