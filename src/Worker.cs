using k8s;
using k8s.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Worker : BackgroundService
{
  private readonly ILogger<Worker> _log;

  public Worker(ILogger<Worker> logger)
  {
    _log = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    //TODO: add to DI container
    var config = KubernetesClientConfiguration.BuildDefaultConfig();
    var client = new Kubernetes(config);

    _log.LogInformation("Connected to cluster");

    //TODO: Make the label selector config driven
    var nodes = client.ListNode(labelSelector: "kubernetes.io/os=windows");
    _log.LogInformation($"Found {nodes.Items.Count()} nodes.");
    foreach (var n in nodes.Items)
    {
      var newTaints = new List<V1Taint>();
      if (n.Spec.Taints != null && n.Spec.Taints.Any())
      {
        foreach (var t in n.Spec.Taints)
        {
          //TODO: make this config driven
          if (t.Key != "kubernetes.azure.com/scalesetpriority"
          || t.Value != "spot")
          {
            _log.LogInformation($"Keeping taint {t.Key}");
            newTaints.Add(t);
          }
          else
          {
            _log.LogInformation($"Removing taint {t.Key}");
          }
        }
        var patch = new V1Patch(new V1Node(spec: new V1NodeSpec(taints: newTaints)), V1Patch.PatchType.MergePatch);
        _log.LogInformation($"Submitting patch");
        var result = await client.PatchNodeAsync(patch, n.Metadata.Name);
        _log.LogInformation($"Patched {n.Metadata.Name}");
      }
      else
      {
        _log.LogInformation($"No taints found on node {n.Metadata.Name}");
      }
    }
  }
}
