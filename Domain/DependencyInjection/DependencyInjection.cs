using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ProductAndOrder.Domain.DependencyInjection
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDomainDI(this IServiceCollection services)
		{
			return services;
		}	
	}
}
