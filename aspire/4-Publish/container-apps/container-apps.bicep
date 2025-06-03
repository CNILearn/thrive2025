@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param userPrincipalId string

param tags object = { }

resource container_apps_mi 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: take('container_apps_mi-${uniqueString(resourceGroup().id)}', 128)
  location: location
  tags: tags
}

resource container_apps_acr 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: take('containerappsacr${uniqueString(resourceGroup().id)}', 50)
  location: location
  sku: {
    name: 'Basic'
  }
  tags: tags
}

resource container_apps_acr_container_apps_mi_AcrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(container_apps_acr.id, container_apps_mi.id, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d'))
  properties: {
    principalId: container_apps_mi.properties.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
    principalType: 'ServicePrincipal'
  }
  scope: container_apps_acr
}

resource container_apps_law 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: take('containerappslaw-${uniqueString(resourceGroup().id)}', 63)
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
  tags: tags
}

resource container_apps 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: take('containerapps${uniqueString(resourceGroup().id)}', 24)
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: container_apps_law.properties.customerId
        sharedKey: container_apps_law.listKeys().primarySharedKey
      }
    }
    workloadProfiles: [
      {
        name: 'consumption'
        workloadProfileType: 'Consumption'
      }
    ]
  }
  tags: tags
}

resource aspireDashboard 'Microsoft.App/managedEnvironments/dotNetComponents@2024-10-02-preview' = {
  name: 'aspire-dashboard'
  properties: {
    componentType: 'AspireDashboard'
  }
  parent: container_apps
}

resource container_apps_Contributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(container_apps.id, userPrincipalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c'))
  properties: {
    principalId: userPrincipalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
  }
  scope: container_apps
}

resource container_apps_storageVolume 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: take('containerappsstoragevolume${uniqueString(resourceGroup().id)}', 24)
  kind: 'StorageV2'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    largeFileSharesState: 'Enabled'
  }
  tags: tags
}

resource storageVolumeFileService 'Microsoft.Storage/storageAccounts/fileServices@2024-01-01' = {
  name: 'default'
  parent: container_apps_storageVolume
}

resource shares_volumes_postgres_0 'Microsoft.Storage/storageAccounts/fileServices/shares@2024-01-01' = {
  name: take('sharesvolumespostgres0-${uniqueString(resourceGroup().id)}', 63)
  properties: {
    enabledProtocols: 'SMB'
    shareQuota: 1024
  }
  parent: storageVolumeFileService
}

resource managedStorage_volumes_postgres_0 'Microsoft.App/managedEnvironments/storages@2024-03-01' = {
  name: take('managedstoragevolumespostgres${uniqueString(resourceGroup().id)}', 24)
  properties: {
    azureFile: {
      accountName: container_apps_storageVolume.name
      accountKey: container_apps_storageVolume.listKeys().keys[0].value
      accessMode: 'ReadWrite'
      shareName: shares_volumes_postgres_0.name
    }
  }
  parent: container_apps
}

output volumes_postgres_0 string = managedStorage_volumes_postgres_0.name

output MANAGED_IDENTITY_NAME string = container_apps_mi.name

output MANAGED_IDENTITY_PRINCIPAL_ID string = container_apps_mi.properties.principalId

output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = container_apps_law.name

output AZURE_LOG_ANALYTICS_WORKSPACE_ID string = container_apps_law.id

output AZURE_CONTAINER_REGISTRY_NAME string = container_apps_acr.name

output AZURE_CONTAINER_REGISTRY_ENDPOINT string = container_apps_acr.properties.loginServer

output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = container_apps_mi.id

output AZURE_CONTAINER_APPS_ENVIRONMENT_NAME string = container_apps.name

output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = container_apps.id

output AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = container_apps.properties.defaultDomain