#
# Creates a storage account with a storage container on Azure.
#

# Create a Storage Account
resource "azurerm_storage_account" "main" {
  name                     = var.app_name
  resource_group_name      = azurerm_resource_group.main.name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

output "AZURE_STORAGE_ACCOUNT_NAME" {
  value = azurerm_storage_account.main.name
}

output "AZURE_STORAGE_ACCOUNT_KEY" {
  value = azurerm_storage_account.main.primary_access_key
  sensitive = true
}