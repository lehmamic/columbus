using System;
using System.Threading;
using System.Threading.Tasks;

namespace Diskordia.Columbus.Common.Hosting
{
	public interface IServiceHost : IDisposable
	{
		event EventHandler HostStopped;

		Task StartAsync(CancellationToken token);

		Task StopAsync();
	}
}
