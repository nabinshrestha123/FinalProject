using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Application.DTO;

namespace ProductAndOrder.Application.Interfaces
{
	public interface IUserServiceClient
	{
		Task<IEnumerable<UserDto>> GetAllUserAsync(int UserId);
		Task<UserDto> GetUserAsync(int UserId);
	}
}
