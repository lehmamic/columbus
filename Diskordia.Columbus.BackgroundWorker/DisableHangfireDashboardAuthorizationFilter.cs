using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Diskordia.Columbus.BackgroundWorker
{
	public class DisableHangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
	{
		public bool Authorize([NotNull] DashboardContext context)
		{
			return true;
		}
	}
}
