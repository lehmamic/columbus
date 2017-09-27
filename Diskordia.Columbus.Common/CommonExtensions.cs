using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.Common
{
	public static class CommonExtensions
	{
		public static IServiceCollection AddSerializer(this IServiceCollection services)
		{
			services.AddSingleton<ISerializer, ObjectSerializer>();

			return services;
		}
	}
}
