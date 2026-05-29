@description('Azure location')
param location string

@description('Base name for resources')
param baseName string

@description('SKU for App Service plan')
param appServiceSku string

@description('App Insights connection string')
param appInsightsConnectionString string

@description('Storage account name used for noise log blobs')
param storageAccountName string

@description('Storage container name used for noise log blobs')
param logsContainerName string

@description('Folder path for local JSON persistence')
param localDataFolder string = '/home/site/data'

var appServicePlanName = '${baseName}-plan'
var webAppName = '${baseName}-web-${uniqueString(resourceGroup().id)}'

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: appServiceSku
  }
  kind: 'app'
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'NoiseStorage__AccountUrl'
          value: 'https://${storageAccountName}.blob.core.windows.net'
        }
        {
          name: 'NoiseStorage__ContainerName'
          value: logsContainerName
        }
        {
          name: 'LocalData__FolderPath'
          value: localDataFolder
        }
      ]
    }
    httpsOnly: true
  }
}

output webAppName string = webApp.name
output principalId string = webApp.identity.principalId
