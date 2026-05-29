
az group create --name 'rg-noise-capture' --location 'eastus2'

az deployment group create --name 'noise-capture-deploy' --resource-group 'rg-noise-capture' --template-file './main.bicep' --parameters './main.bicepparam'
