# Creates a container registry in Azure (for Docker images).
# Note!
# - Resource "azurerm_resource_group.main" with a property "name" is defined in the file "resource-group.tf".
# - The value for "resource_group_name" below is set using property "name" in resource "azurerm_resource_group.main":
#   - resource_group_name = azurerm_resource_group.main.name
# - "name" and "location" below are set from Terraform variables defined in the file "variables.tf".
# - The ouputs below are used to print out the Azure Container Registry's hostname, username and password.
#   - The password is sensitive and will be redacted when printed out.


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