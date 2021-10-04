locals {
  resource_group_name = var.resource_group_name
  resource_group_location = var.resource_group_location
}

resource "azurerm_storage_account" "function_storage_account" {
  name                     = "${var.service_name}sa"
  location            = local.resource_group_location
  resource_group_name = local.resource_group_name
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "function_service_plan" {
  name                = "${var.service_name}-service-plan"
  location            = local.resource_group_location
  resource_group_name = local.resource_group_name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_function_app" "funciton_app" {
  name                       = "${var.service_name}-function"
  location                   = local.resource_group_location
  resource_group_name        = local.resource_group_name
  app_service_plan_id        = azurerm_app_service_plan.function_service_plan.id
  storage_account_name       = azurerm_storage_account.function_storage_account.name
  storage_account_access_key = azurerm_storage_account.function_storage_account.primary_access_key
}