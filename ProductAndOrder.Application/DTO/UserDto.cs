using System;
using System.Collections.Generic;
using System.Text;

namespace ProductAndOrder.Application.DTO
{
	public class UserDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Role { get; set; } = null!;
		public string Phone { get; set; } = null!;
		public string Address { get; set; } = null!;

	}
}
