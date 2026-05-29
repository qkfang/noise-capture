using 'main.bicep'

param baseName = 'noisecap'
param location = 'westus3'
param sqlAzureAdAdminLogin = 'danielfang@MngEnvMCAP951655.onmicrosoft.com'
param sqlAzureAdAdminObjectId = '4b74544b-02c6-4e4f-b936-732c9c3fff65' // Run: az ad user show --id danielfang@MngEnvMCAP951655.onmicrosoft.com --query id -o tsv
