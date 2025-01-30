# Creates a resource group in your Azure account.
# Note:
# - The block type is "resource" since we are defining a resource.
# - The resource we are defining is "azurerm_resource_group" (an Azure Resource Group).
# - "main" is the name used to identify the resource in Terraform as "azurerm_resource_group.main"
# - "name" is the name of the resource group in Azure.
# - "location" is the region in Azure (where the resource group metadata will be stored).
# - Notice how the Terrform variables (defined in the file "variables.tf") are used:
#   - "var.app_name" refers to the variable "app_name" and returns its associated value.
#   - "var.location" refers to the variable "location" and returns its associated value.

resource "azurerm_resource_group" "main" {
  name     = var.app_name
  location = var.location
}