using System;
namespace Diskordia.Columbus.Common.Hosting
{
	public interface IServiceHostBuilder
	{
		IServiceHostBuilder UseStartup<T>() where T : IStartup, new();

		IServiceHost Build();
	}
}
