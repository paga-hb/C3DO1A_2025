# Create a storage container in Azure (for BLoBs, i.e. arbitrary files)

# Note!
# - Resource "azurerm_storage_account.main" with a property "id" is defined in the file "storage-account.tf".
# - The value for "storage_account_id" below is set using property "id" in resource "azurerm_storage_account.main":
#   - storage_account_id  = azurerm_storage_account.main.id

resource "azurerm_storage_container" "main" {
  name                  = "videos"
  storage_account_id  = azurerm_storage_account.main.id
  container_access_type = "private"
}