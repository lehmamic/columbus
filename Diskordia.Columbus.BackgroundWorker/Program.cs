using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diskordia.Columbus.Bots;
using Diskordia.Columbus.Common;
using Diskordia.Columbus.Common.Hosting;
using Diskordia.Columbus.Staging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diskordia.Columbus.BackgroundWorker
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildServiceHost().Run();
		}

		public static IServiceHost BuildServiceHost() =>
			new ServiceHostBuilder()
				.UseStartup<Startup>()
				.Build();

		//private static async Task RunAsync(CancellationToken token = default(CancellationToken))
		//{
		//	// Wait for token shutdown if it can be canceled
		//	if (token.CanBeCanceled)
		//	{
		//		await host.RunAsync(token, shutdownMessage: null);
		//		return;
		//	}

		//	// If token cannot be canceled, attach Ctrl+C and SIGTERM shutdown
		//	var done = new ManualResetEventSlim(false);
		//	using (var cts = new CancellationTokenSource())
		//	{
		//		AttachCtrlcSigtermShutdown(cts, done, shutdownMessage: "Application is shutting down...");

		//		await host.RunAsync(cts.Token, "Application started. Press Ctrl+C to shut down.");
		//		done.Set();
		//	}
		//}

		//private static async Task RunAsync(this IWebHost host, CancellationToken token, string shutdownMessage)
		//{
		//	using (host)
		//	{
		//		await host.StartAsync(token);

		//		var hostingEnvironment = host.Services.GetService<IHostingEnvironment>();
		//		var applicationLifetime = host.Services.GetService<IApplicationLifetime>();

		//		Console.WriteLine($"Hosting environment: {hostingEnvironment.EnvironmentName}");
		//		Console.WriteLine($"Content root path: {hostingEnvironment.ContentRootPath}");

		//		var serverAddresses = host.ServerFeatures.Get<IServerAddressesFeature>()?.Addresses;
		//		if (serverAddresses != null)
		//		{
		//			foreach (var address in serverAddresses)
		//			{
		//				Console.WriteLine($"Now listening on: {address}");
		//			}
		//		}

		//		if (!string.IsNullOrEmpty(shutdownMessage))
		//		{
		//			Console.WriteLine(shutdownMessage);
		//		}

		//		await host.WaitForTokenShutdownAsync(token);
		//	}
		//}

		//private static void Start()
		//{
		//	var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

		//	var builder = new ConfigurationBuilder()
		//		.SetBasePath(Directory.GetCurrentDirectory())
		//		.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		//		.AddJsonFile($"appsettings.{environmentName}.json", optional: true)
		//		.AddEnvironmentVariables();

		//	var configuration = builder.Build();

		//	var serviceProvider = new ServiceCollection()
		//		.AddOptions()
		//		.AddSerializer()
		//		.AddFareDealStaging(configuration)
		//		.AddFareDealBots(configuration)
		//		.BuildServiceProvider();

		//	IEnumerable<IStartable> modules = serviceProvider.GetServices<IStartable>();
		//	foreach (var module in modules.AsParallel())
		//	{
		//		module.Start();
		//	}
		//}

		//public static async Task WaitForShutdownAsync(CancellationToken token = default(CancellationToken))

		//{
		//	var done = new ManualResetEventSlim(false);
		//	using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
		//	{
		//		AttachCtrlcSigtermShutdown(cts, done, shutdownMessage: string.Empty);

		//		await host.WaitForTokenShutdownAsync(cts.Token);
		//		done.Set();
		//	}
		//}

		//private static async Task WaitForTokenShutdownAsync(CancellationToken token)
		//{

		//	var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
		//	applicationLifetime.ApplicationStopping.Register(obj =>
		//	{
		//		var tcs = (TaskCompletionSource<object>)obj;
		//		tcs.TrySetResult(null);
		//	}, waitForStop);

		//	await waitForStop.Task;

		//	// WebHost will use its default ShutdownTimeout if none is specified.
		//	await host.StopAsync();
		//}
	}


}
