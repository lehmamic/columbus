using System;
using AutoMapper;
using Diskordia.Columbus.BackgroundWorker.Services;
using Diskordia.Columbus.Common;
using Diskordia.Columbus.Contract.FareDeals;
using Diskordia.Columbus.Staging;
using Diskordia.Columbus.Staging.FareDeals;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Rebus.Config;
using Rebus.Routing.TypeBased;
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

			services.AddAutoMapper(opt => opt.AddProfiles(
				new []
				{
					typeof(StagingExtensions).Assembly
				}));

			services.AddFareDealStaging(Configuration);
			

			services.AddSingleton<IFareDealScanProxy, FareDealScanProxy>();

			
			services.AutoRegisterHandlersFromAssemblyOf<FareDealScanResultHandler>();

			var serviceBusOptions = this.Configuration.GetSection("ServiceBus").Get<ServiceBusOptions>();
			services.AddRebus(config => config.Transport(t => t.UseRabbitMq(serviceBusOptions.ConnectionString, serviceBusOptions.QueueName))
			                                  .Routing(r => r.TypeBased()
                                                                .Map<StartFareDealsScanCommand>("Diskordia.Columbus.FareDealScanner")
                                                                .Map<FareDealScanResult<SingaporeAirlinesFareDeal>>("Diskordia.Columbus.Staging")));

			var hangfireOptions = this.Configuration.GetSection("HangFire").Get<MongoDbOptions>();
			services.AddHangfire(config =>
			{
				config.UseMongoStorage(hangfireOptions.ConnectionString, hangfireOptions.Database);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
            var policy = Policy.Handle<Exception>()
                               .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
 
            policy.Execute(() => app.UseRebus());

            policy.Execute(() =>
            {
                app.UseHangfireServer();
                app.UseHangfireDashboard(options: new DashboardOptions
                {
                  Authorization = new [] { new DisableHangfireDashboardAuthorizationFilter() }
                });
            });

            var schedulerOptions = this.Configuration.GetSection("Scheduler").Get<SchedulerOptions>();
            var fareDealsBotsProxy = app.ApplicationServices.GetService<IFareDealScanProxy>();

            RecurringJob.AddOrUpdate(schedulerOptions.SingaporeAirlines.JobId, () => fareDealsBotsProxy.TriggerFareDealsScan(), schedulerOptions.SingaporeAirlines.CronExpression);
		}
	}
}
