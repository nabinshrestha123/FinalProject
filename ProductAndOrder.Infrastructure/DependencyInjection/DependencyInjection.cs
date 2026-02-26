using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ProductAndOrder.Infrastructure.DependencyInjection
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
		{
			
			
			ArgumentNullException.ThrowIfNull(services);



	return services;
		}
	}
}
