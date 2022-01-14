# TaintRemover
This project represents a simple operator that will remove taints that are automatically added to nodes during nodepool provisioning. As an example, nodes provisioned on [Azure Spot VMs](https://azure.microsoft.com/en-us/services/virtual-machines/spot/) are automatically tainted with ```kubernetes.azure.com/scalesetpriority=spot``` by the AKS management plane. In some cases it is easier to remove the taint than to explicitly tolerate it in all workloads targeting those nodes.

## Design
The app intentionally only inspects the current host node. Node selection should be handled via normal Kubernetes scheduling semantics. In a typical scenario the container should be run as part of a daemonset to ensure that it is applied to all nodes within the target pool (see [deploy.yaml](deploy.yaml) for an example).

## TODO
* ~~Make the image multi-arch~~
* ~~Move target taints to config (not hard coded)~~
* Support Helm deployment
* ~~Support generic k8s deployment (not just AKS)~~

### Resources
[kubernetes-client/csharp](https://github.com/kubernetes-client/csharp)
