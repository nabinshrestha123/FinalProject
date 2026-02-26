using Microsoft.Extensions.DependencyInjection;
using ProductAndOrder.Application.DependencyInjection;
using ProductAndOrder.Infrastructure.DependencyInjection;

namespace ProductAndOrder.Api.DependencyInjection
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApiDI(this IServiceCollection services)
		{
			services.AddApplicationDI().AddInfrastructureDI();
			ArgumentNullException.ThrowIfNull(services);

			return services;
		}
	}
}
