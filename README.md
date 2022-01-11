# AKS TaintRemover
This project represents a simple operator that will remove the NoSchedule taint automatically added to nodes provisioned on [Azure Spot VMs](https://azure.microsoft.com/en-us/services/virtual-machines/spot/).

Currently the project is hardcoded to only remove the ```kubernetes.azure.com/scalesetpriority=spot``` taint and is target specifically at Windows nodes.

## Design
The pod hosts a simple console app that only inspects the node it is scheduled on and only runs once. As such it runs as an init container within a daemonset. This allows the user to leverage native kubernetes scheduling to select which nodes need to be un-tainted.

## TODO
* Make the image multi-arch
* Move target taints to config (not hard coded)
* Support Helm deployment

### Resources
[kubernetes-client/csharp](https://github.com/kubernetes-client/csharp)
