# Sets global variables for this Terraform project.

variable "subscription_id" {
  description = "The Azure subscription ID"
  type        = string
}

variable "app_name" {
  default = "flixtube2025g00"
}

variable "location" {
  default = "westeurope"
}