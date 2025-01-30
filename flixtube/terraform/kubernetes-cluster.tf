#
# Creates a managed Kubernetes cluster on Azure.
#
resource "azurerm_kubernetes_cluster" "main" {
    name                = var.app_name
    location            = var.location
    resource_group_name = azurerm_resource_group.main.name
    dns_prefix          = var.app_name
    kubernetes_version  = var.kubernetes_version

    default_node_pool {
        name            = "default"
        node_count      = 1
        vm_size         = "Standard_D2S_V3"
    }

    #
    # Instead of creating a service principle have the system figure this out.
    #
    identity {
        type = "SystemAssigned"
    }    
}

#
# Attaches the container registry to the cluster.
# See example here: https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/container_registry#example-usage-attaching-a-container-registry-to-a-kubernetes-cluster
#
resource "azurerm_role_assignment" "main" {
  principal_id                     = azurerm_kubernetes_cluster.main.kubelet_identity[0].object_id
  role_definition_name             = "AcrPull"
  scope                            = azurerm_container_registry.main.id
  skip_service_principal_aad_check = true
}