# AKS TaintRemover
This project represents a simple operator that will remove the NoSchedule taint automatically added to nodes provisioned on [Azure Spot VMs](https://azure.microsoft.com/en-us/services/virtual-machines/spot/).

Currently the project is hardcoded to only remove the ```kubernetes.azure.com/scalesetpriority=spot``` taint.

## Design
The app intentionally only inspects the current host node. Node selection should be handled via normal Kubernetes scheduling semantics. In a typical scenario the container should be run as an init container for a daemonset to ensure that it is applied to all nodes within the target pool (see [deploy.yaml](deploy.yaml) for an example).

## TODO
* ~~Make the image multi-arch~~
* ~~Move target taints to config (not hard coded)~~
* Support Helm deployment
* Support generic k8s deployment (not just AKS)

### Resources
[kubernetes-client/csharp](https://github.com/kubernetes-client/csharp)
