# Initialises Terraform providers and sets their version numbers.
# Note:
# 1) We require the provider "hashicorp/azurerm" version "4.14.0" (a specific Azure provider).
# 2) We require the version of Terraform to be "1.10.3".
# 3) We are using the provider "azurerm" (with default features) that we required in (1).
# 4) We are setting the Azure provider's "subscription_id" from the Terraform variable stored in "variables.tf".

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.14.0"
    }
  }

  required_version = ">= 1.10.3"
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}