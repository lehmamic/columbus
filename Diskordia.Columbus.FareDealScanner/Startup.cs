using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diskordia.Columbus.Common;
using Diskordia.Columbus.Contract.FareDeals;
using Diskordia.Columbus.FareDealScanner.FareDeals;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;

namespace Diskordia.Columbus.FareDealScanner
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddFareDealBots(Configuration);
            services.AutoRegisterHandlersFromAssemblyOf<FareDealBotsHandler>();

            var serviceBusOptions = this.Configuration.GetSection("ServiceBus").Get<ServiceBusOptions>();
            services.AddRebus(config => config.Transport(t => t.UseRabbitMq(serviceBusOptions.ConnectionString, serviceBusOptions.QueueName))
                                  .Routing(r => r.TypeBased()
                                                    .Map<StartFareDealsScanCommand>("Diskordia.Columbus.FareDealScanner")
                                                    .Map<FareDealScanResult<SingaporeAirlinesFareDeal>>("Diskordia.Columbus.Staging")));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var policy = Policy.Handle<Exception>()
                               .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() => app.UseRebus());

            app.UseMvc();
        }
    }
}
