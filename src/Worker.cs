using k8s;
using k8s.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class Worker : BackgroundService
{
  private readonly Kubernetes _k8s;
  private readonly ILogger<Worker> _log;
  private readonly IConfiguration _config;

  public Worker(ILogger<Worker> logger, IConfiguration config, Kubernetes client)
  {
    _log = logger;
    _config = config;
    _k8s = client;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var nodeName = _config["NODE_NAME"];    
    var taint = new V1Taint().Parse(_config["TAINT"]);
    
    _log.LogInformation($"Running on node {nodeName} looking for taint {taint}");

    var node = _k8s.ReadNode(nodeName);
    var newTaints = new List<V1Taint>();
    if (node.Spec.Taints != null && node.Spec.Taints.Any())
    {
      _log.LogInformation($"Found {node.Spec.Taints.Count()} taints on node {node.Metadata.Name}.");
      foreach (var t in node.Spec.Taints)
      {
        if (t.Key == taint.Key && t.Effect == taint.Effect
          && (taint.Value == null || t.Value == taint.Value))
        {
          _log.LogInformation($"Removing taint {t.Key}");
        }
        else
        {
          _log.LogInformation($"Keeping taint {t.Key}");
          newTaints.Add(t);
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
