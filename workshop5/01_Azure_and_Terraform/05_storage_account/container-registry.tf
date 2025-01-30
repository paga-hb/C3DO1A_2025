# Creates a container registry in Azure (for Docker images).

resource "azurerm_container_registry" "main" {
  name                = var.app_name
  resource_group_name = azurerm_resource_group.main.name
  location            = var.location
  admin_enabled       = true
  sku                 = "Basic"
}

output "AZURE_CONTAINER_REGISTRY_HOSTNAME" {
  value = azurerm_container_registry.main.login_server
}

output "AZURE_CONTAINER_REGISTRY_USERNAME" {
  value = azurerm_container_registry.main.admin_username
}

output "AZURE_CONTAINER_REGISTRY_PASSWORD" {
  value = azurerm_container_registry.main.admin_password
  sensitive = true
}