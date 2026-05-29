targetScope = 'resourceGroup'

@description('Azure location')
param location string = resourceGroup().location

@description('Base name for resources')
param baseName string = 'noisecapture'

@description('SKU for App Service plan')
@allowed([
  'F1'
  'B1'
  'S1'
])
param appServiceSku string = 'B1'

@description('Blob container name for noise log files')
param logsContainerName string = 'noise-logs'

module monitoring 'monitoring.bicep' = {
  name: 'monitoring'
  params: {
    location: location
    baseName: baseName
  }
}

module storage 'storage.bicep' = {
  name: 'storage'
  params: {
    location: location
    baseName: baseName
    logsContainerName: logsContainerName
  }
}

module appService 'appservice.bicep' = {
  name: 'appservice'
  params: {
    location: location
    baseName: baseName
    appServiceSku: appServiceSku
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    storageAccountName: storage.outputs.storageAccountName
    logsContainerName: storage.outputs.logsContainerName
  }
}

resource blobDataContributorAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: resourceId('Microsoft.Storage/storageAccounts', storage.outputs.storageAccountName)
  name: guid(storage.outputs.storageAccountId, appService.outputs.principalId, 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
    principalId: appService.outputs.principalId
    principalType: 'ServicePrincipal'
  }
}

output webAppName string = appService.outputs.webAppName
output storageAccountName string = storage.outputs.storageAccountName
output logsContainerName string = storage.outputs.logsContainerName
