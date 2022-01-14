using k8s;
using k8s.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Worker : BackgroundService
{
  private Kubernetes _k8s;
  private readonly ILogger<Worker> _log;

  public Worker(ILogger<Worker> logger, Kubernetes client)
  {
    _log = logger;
    _k8s = client;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var nodeName = Environment.GetEnvironmentVariable("NODE_NAME");
    _log.LogInformation($"Running on node {nodeName}");

    var node = _k8s.ReadNode(nodeName);
    var newTaints = new List<V1Taint>();
    if (node.Spec.Taints != null && node.Spec.Taints.Any())
    {
      _log.LogInformation($"Found {node.Spec.Taints.Count()} taints on node {node.Metadata.Name}.");
      foreach (var t in node.Spec.Taints)
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
      var result = await _k8s.PatchNodeAsync(patch, node.Metadata.Name);
      _log.LogInformation($"Patched {node.Metadata.Name}");
    }
    else
    {
      _log.LogInformation($"No taints found on node {node.Metadata.Name}");
    }
  }
}
