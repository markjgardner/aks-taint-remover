using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services =>
  {
    services.AddHostedService<Worker>();
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

await host.RunAsync();