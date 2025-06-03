@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param container_apps_outputs_azure_container_apps_environment_default_domain string

param container_apps_outputs_azure_container_apps_environment_id string

param container_apps_outputs_azure_container_registry_endpoint string

param container_apps_outputs_azure_container_registry_managed_identity_id string

param existingapi_containerimage string

param existingapi_containerport string

@secure()
param postgres_password_value string

resource existingapi 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'existingapi'
  location: location
  properties: {
    configuration: {
      secrets: [
        {
          name: 'connectionstrings--postgres-books-db'
          value: 'Host=postgres;Port=5432;Username=postgres;Password=${postgres_password_value};Database=postgres-books-db'
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: int(existingapi_containerport)
        transport: 'http'
      }
      registries: [
        {
          server: container_apps_outputs_azure_container_registry_endpoint
          identity: container_apps_outputs_azure_container_registry_managed_identity_id
        }
      ]
    }
    environmentId: container_apps_outputs_azure_container_apps_environment_id
    template: {
      containers: [
        {
          image: existingapi_containerimage
          name: 'existingapi'
          env: [
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES'
              value: 'true'
            }
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES'
              value: 'true'
            }
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY'
              value: 'in_memory'
            }
            {
              name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED'
              value: 'true'
            }
            {
              name: 'HTTP_PORTS'
              value: existingapi_containerport
            }
            {
              name: 'ConnectionStrings__postgres-books-db'
              secretRef: 'connectionstrings--postgres-books-db'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${container_apps_outputs_azure_container_registry_managed_identity_id}': { }
    }
  }
}