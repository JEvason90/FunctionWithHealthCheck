locals {
  resource_group_name = var.resource_group_name
  resource_group_location = var.resource_group_location
}

resource "azurerm_servicebus_namespace" "service_bus_namespace" {
  name                = "${var.service_name}-svcbus-ns"
  location            = local.resource_group_location
  resource_group_name = local.resource_group_name
  sku                 = "Basic"

  tags = {
    source = "terraform"
  }
}

resource "azurerm_servicebus_queue" "service_bus_queue" {
  name                = "${var.service_name}-svcbus-queue"
  resource_group_name = local.resource_group_name
  namespace_name      = azurerm_servicebus_namespace.service_bus_namespace.name
}