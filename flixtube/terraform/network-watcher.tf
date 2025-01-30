# Creates a Network Watcher on Azure.
# Note!
# - We create a "networkwatcher" (in its own Resource Group).
#   - This is required when a virtual network is created in Azure.
#     - An Azure Kubernetes Service (AKS) will create a virtual network.
#   - This is automatically created by Azure, but we explicitly create it here.
#     - The only reason we do this explicitly is so Terraform Destroy will automatically delete it for us.

resource "azurerm_resource_group" "networkwatcher" {
  name     = "NetworkWatcherRG"
  location = var.location
}

resource "azurerm_network_watcher" "networkwatcher" {
  name                = "NetworkWatcher_westeurope"
  location            = var.location
  resource_group_name = azurerm_resource_group.networkwatcher.name
}