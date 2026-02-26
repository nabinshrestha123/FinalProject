using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ProductAndOrder.Application.DependencyInjection
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplicationDI(this IServiceCollection services)
		{

			ArgumentNullException.ThrowIfNull(services);

			return services;

		}
	}
}
