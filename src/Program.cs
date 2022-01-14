using k8s;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHost host = null;

try {
  host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
      services.AddHostedService<Worker>();
      services.AddSingleton((s) => {
        var config = KubernetesClientConfiguration.BuildDefaultConfig();
        return new Kubernetes(config);
      });
    })
    .ConfigureLogging((_, logging) => 
    {
      logging.AddSimpleConsole(options => 
      { 
        options.IncludeScopes = true; 
        options.SingleLine = true; 
      });
    })
    .Build();


  await host.StartAsync();
  await host.StopAsync();
  await host.WaitForShutdownAsync();
}
finally
{
  if (host is IAsyncDisposable d) await d.DisposeAsync();
}