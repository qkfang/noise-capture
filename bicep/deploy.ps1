
az group create --name 'rg-noise-capture' --location 'westus3'

az deployment group create --name 'noise-capture-deploy' --resource-group 'rg-noise-capture' --template-file './main.bicep' --parameters './main.bicepparam'

$spAppId = az ad sp list --display-name 'sp-demo-01' --query '[0].appId' -o tsv
$subscriptionId = az account show --query 'id' -o tsv
az role assignment create --assignee $spAppId --role 'Contributor' --scope "/subscriptions/$subscriptionId/resourceGroups/rg-noise-capture"
az role assignment create --assignee $spAppId --role 'User Access Administrator' --scope "/subscriptions/$subscriptionId/resourceGroups/rg-noise-capture"
    