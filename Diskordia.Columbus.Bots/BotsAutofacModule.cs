using Autofac;
using Diskordia.Columbus.Bots.Services.SingaporeAirlines;

namespace Diskordia.Columbus.Bots
{
	public class BotsAutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SingaporeAirlinesFareDealService>().AsImplementedInterfaces();
			builder.RegisterType<FareDealServiceStartable>().AsImplementedInterfaces();
		}
	}
}
