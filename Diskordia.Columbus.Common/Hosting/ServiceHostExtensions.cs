using System;
using System.Threading;
using System.Threading.Tasks;

namespace Diskordia.Columbus.Common.Hosting
{
	public static class ServiceHostExtensions
	{
		public static void Run(this IServiceHost host)
		{
			host.RunAsync(default(CancellationToken)).GetAwaiter().GetResult();
		}

		public static async Task RunAsync(this IServiceHost host, CancellationToken token = default(CancellationToken))

		{
			// Wait for token shutdown if it can be canceled
			if (token.CanBeCanceled)
			{
				await host.RunAsync(token, shutdownMessage: null);
				return;
			}

			// If token cannot be canceled, attach Ctrl+C and SIGTERM shutdown
			var done = new ManualResetEventSlim(false);
			using (var cts = new CancellationTokenSource())
			{
				AttachCtrlcSigtermShutdown(cts, done, shutdownMessage: "Application is shutting down...");

				await host.RunAsync(cts.Token, "Application started. Press Ctrl+C to shut down.");
				done.Set();
			}
		}

		private static async Task RunAsync(this IServiceHost host, CancellationToken token, string shutdownMessage)
		{
			using (host)
			{
				await host.StartAsync(token);

				if (!string.IsNullOrEmpty(shutdownMessage))
				{
					Console.WriteLine(shutdownMessage);
				}

				await host.WaitForTokenShutdownAsync(token);
			}
		}

		private static void AttachCtrlcSigtermShutdown(CancellationTokenSource cts, ManualResetEventSlim resetEvent, string shutdownMessage)
		{
			void Shutdown()
			{
				if (!cts.IsCancellationRequested)
				{
					if (!string.IsNullOrEmpty(shutdownMessage))
					{
						Console.WriteLine(shutdownMessage);
					}
					try
					{
						cts.Cancel();
					}
					catch (ObjectDisposedException) { }
				}

				// Wait on the given reset event
				resetEvent.Wait();
			};

			AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Shutdown();
			Console.CancelKeyPress += (sender, eventArgs) =>
			{
				Shutdown();
				// Don't terminate the process immediately, wait for the Main thread to exit gracefully.
				eventArgs.Cancel = true;
			};
		}

		private static async Task WaitForTokenShutdownAsync(this IServiceHost host, CancellationToken token)
		{
			token.Register(() =>
			{
				host.StopAsync().GetAwaiter().GetResult();
			});

			var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
			host.HostStopped += (source, e) => waitForStop.TrySetResult(null);

			await waitForStop.Task;
		}
	}
}
