targetScope = 'resourceGroup'

@description('Azure location')
param location string = resourceGroup().location

@description('Base name for resources')
param baseName string = 'noisecap'

@description('SKU for App Service plan')
@allowed([
  'F1'
  'B1'
  'S1'
])
param appServiceSku string = 'S1'

@description('Administrator login for Azure SQL Server')
param sqlAdministratorLogin string = 'noisecaptureadmin'

@secure()
@description('Administrator password for Azure SQL Server')
param sqlAdministratorPassword string

@description('DTU-based Azure SQL database SKU')
@allowed([
  'Basic'
  'S0'
  'S1'
])
param sqlDatabaseSku string = 'S0'

var uniqueSuffix = uniqueString(resourceGroup().id)
var logAnalyticsName = '${baseName}-law'
var appInsightsName = '${baseName}-appi'
var appServicePlanName = '${baseName}-plan'
var webAppName = '${baseName}-web'
var sqlServerName = toLower('${baseName}sql')
var sqlDatabaseName = '${baseName}-db'

module monitoring 'monitoring.bicep' = {
  name: 'monitoring'
  params: {
    location: location
    logAnalyticsName: logAnalyticsName
    appInsightsName: appInsightsName
  }
}

module sql 'sql.bicep' = {
  name: 'sql'
  params: {
    location: location
    sqlServerName: sqlServerName
    sqlDatabaseName: sqlDatabaseName
    sqlAdministratorLogin: sqlAdministratorLogin
    sqlAdministratorPassword: sqlAdministratorPassword
    sqlDatabaseSku: sqlDatabaseSku
  }
}

module appService 'appservice.bicep' = {
  name: 'appservice'
  params: {
    location: location
    webAppName: webAppName
    appServicePlanName: appServicePlanName
    appServiceSku: appServiceSku
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    sqlServerFullyQualifiedDomainName: sql.outputs.sqlServerFullyQualifiedDomainName
    sqlDatabaseName: sql.outputs.sqlDatabaseName
    sqlAdministratorLogin: sqlAdministratorLogin
    sqlAdministratorPassword: sqlAdministratorPassword
  }
}

output webAppName string = appService.outputs.webAppName
output sqlServerName string = sql.outputs.sqlServerName
output sqlDatabaseName string = sql.outputs.sqlDatabaseName
