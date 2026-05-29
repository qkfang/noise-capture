@description('Azure location')
param location string

@description('Azure SQL Server name')
param sqlServerName string

@description('Azure SQL Database name')
param sqlDatabaseName string

@description('Azure AD administrator login (email)')
param sqlAzureAdAdminLogin string

@description('Azure AD administrator object ID (run: az ad user show --id <email> --query id -o tsv)')
param sqlAzureAdAdminObjectId string

@description('DTU-based Azure SQL database SKU')
param sqlDatabaseSku string

var sqlSkuMap = {
  Basic: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  S0: {
    name: 'S0'
    tier: 'Standard'
    capacity: 10
  }
  S1: {
    name: 'S1'
    tier: 'Standard'
    capacity: 20
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    version: '12.0'
    publicNetworkAccess: 'Enabled'
    minimalTlsVersion: '1.2'
    administrators: {
      administratorType: 'ActiveDirectory'
      login: sqlAzureAdAdminLogin
      sid: sqlAzureAdAdminObjectId
      tenantId: subscription().tenantId
      azureADOnlyAuthentication: true
    }
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  sku: sqlSkuMap[sqlDatabaseSku]
  properties: {
    requestedBackupStorageRedundancy: 'Local'
    zoneRedundant: false
  }
}

resource allowAzureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output sqlServerName string = sqlServer.name
output sqlServerFullyQualifiedDomainName string = sqlServer.properties.fullyQualifiedDomainName
output sqlDatabaseName string = sqlDatabase.name
