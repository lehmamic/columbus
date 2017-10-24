using System;
using Diskordia.Columbus.BackgroundWorker.Services;
using Diskordia.Columbus.Bots;
using Diskordia.Columbus.Bots.FareDeals;
using Diskordia.Columbus.Common;
using Diskordia.Columbus.Staging;
using Diskordia.Columbus.Staging.FareDeals;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
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

			services.AddSingleton<IFareDealScanProxy, FareDealScanProxy>();

			services.AutoRegisterHandlersFromAssemblyOf<FareDealBotsHandler>();
			services.AutoRegisterHandlersFromAssemblyOf<FareDealScanResultHandler>();

			var serviceBusOptions = this.Configuration.GetSection("ServiceBus").Get<ServiceBusOptions>();
			services.AddRebus(config => config.Transport(t => t.UseRabbitMq(serviceBusOptions.ConnectionString, serviceBusOptions.QueueName)));

			var hangfireOptions = this.Configuration.GetSection("HangFire").Get<MongoDbOptions>();
 			services.AddHangfire(config => config.UseMongoStorage(hangfireOptions.ConnectionString, hangfireOptions.Database));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseRebus();

			app.UseHangfireServer();
			app.UseHangfireDashboard();

			var fareDealScanOptions = this.Configuration.GetSection("FareDealScan").Get<FareDealScanOptions>();
			var fareDealsBotsProxy = app.ApplicationServices.GetService<IFareDealScanProxy>();

			RecurringJob.AddOrUpdate("Trigger-FareDealsScan", () => fareDealsBotsProxy.TriggerFareDealsScan(), fareDealScanOptions.ScheduleCronExpression);
			RecurringJob.Trigger("Trigger-FareDealsScan");
		}
	}
}
