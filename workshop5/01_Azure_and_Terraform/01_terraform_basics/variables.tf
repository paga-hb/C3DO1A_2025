# Sets global variables for this Terraform project.
# Note:
# - The block type is "variable" since we are defining a Terraform variable.
# - "subscription_id", "app_name" and "location" are the names of the Terraform variables.
# - "default" is used to set the value for a Terraform variable.
# - If "default" is omitted, Terraform will ask the user to input the value for the variable during "terraform apply".
# - The value for a variable can also be provided via an environment variable.
#   - An environment variable that starts with "TF_VAR_" will be used as the value for the variable VAR_NAME in TF_VAR_NAME
#     - E.g., if environment variable TF_VAR_subscription_id is set to "123", variable "subscription_id" will be set to "123".

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