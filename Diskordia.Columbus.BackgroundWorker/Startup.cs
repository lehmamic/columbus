using System;
using Diskordia.Columbus.BackgroundWorker.Services;
using Diskordia.Columbus.Bots;
using Diskordia.Columbus.Bots.FareDeals;
using Diskordia.Columbus.Staging;
using Diskordia.Columbus.Staging.FareDeals;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rebus.Config;
using Rebus.Serialization.Json;
using Rebus.ServiceProvider;

namespace Diskordia.Columbus.BackgroundWorker
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			if(configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			this.Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();

			services.AddFareDealStaging(Configuration);
			services.AddFareDealBots(Configuration);

			services.AddSingleton<IFareDealBotsService, FareDealBotsService>();

			services.AutoRegisterHandlersFromAssemblyOf<FareDealBotsHandler>();
			services.AutoRegisterHandlersFromAssemblyOf<FareDealScanResultHandler>();

			var serviceBusSection = this.Configuration.GetSection("ServiceBus");
			services.AddRebus(config => config.Transport(t => t.UseRabbitMq(serviceBusSection["ConnectionString"], serviceBusSection["QueueName"])));

			var hangfireSection = this.Configuration.GetSection("HangFire");
			services.AddHangfire(config => config.UseMongoStorage(hangfireSection["ConnectionString"], hangfireSection["Database"]));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseRebus();

			app.UseHangfireServer();
			app.UseHangfireDashboard();

			var fareDealsBotsProxy = app.ApplicationServices.GetService<IFareDealBotsService>();

			RecurringJob.AddOrUpdate("Trigger-FareDealsScan", () => fareDealsBotsProxy.TriggerFareDealsScan(), Cron.Daily);
			RecurringJob.Trigger("Trigger-FareDealsScan");
		}
	}
}
