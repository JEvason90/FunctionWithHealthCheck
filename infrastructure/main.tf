resource "random_string" "postfix" {
  length           = 4
  special          = false
  min_upper = 0
  min_numeric = 0
}

locals {
  service_name = "fncsvc${random_string.postfix.result}"
}

resource "azurerm_resource_group" "function_health_check" {
  name     = "je-func-test"
  location = "West Europe"
}

module "service_bus"{
    source = "./service_bus"
    resource_group_name = azurerm_resource_group.function_health_check.name
    resource_group_location = azurerm_resource_group.function_health_check.location
    service_name = local.service_name
}

module "function_app" {
    source = "./function"
    resource_group_name = azurerm_resource_group.function_health_check.name
    resource_group_location = azurerm_resource_group.function_health_check.location
    service_name = local.service_name
}