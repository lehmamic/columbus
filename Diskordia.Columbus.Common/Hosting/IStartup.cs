using System;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.Common.Hosting
{
	public interface IStartup
	{
		void ConfigureServices(IServiceCollection services);
	}
}