using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;

namespace ProductAndOrder.Application.Services
{
	public class UserServiceClient:IUserServiceClient
	{
		private readonly HttpClient _httpClient;

		public UserServiceClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<IEnumerable<UserDto>> GetAllUserAsync(int userID)
		{
			var response = await _httpClient.GetAsync($"/api/users");
			if (!response.IsSuccessStatusCode)
				return null;

			var user= await response.Content.ReadFromJsonAsync<UserDto>();
			return (IEnumerable<UserDto>)user;
		}

		public async Task<UserDto> GetUserAsync(int userId)
		{
			var response = await _httpClient.GetAsync($"https://localhost:44380/api/HttpUser/GetUser/{userId}");
			if (!response.IsSuccessStatusCode)
				return null;

			var user = await response.Content.ReadFromJsonAsync<UserDto>();
			return user;
		}
	}
}
