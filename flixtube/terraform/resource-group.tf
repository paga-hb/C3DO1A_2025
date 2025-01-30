#
# Creates a resource group for FlixTube in your Azure account.
#
resource "azurerm_resource_group" "main" {
  name     = var.app_name
  location = var.location
}
