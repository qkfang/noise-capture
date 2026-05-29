$token = (az account get-access-token --resource https://database.windows.net/ --query accessToken -o tsv)

# noisecap-db - grant App Service managed identity access
Invoke-Sqlcmd -ServerInstance "noisecapsql.database.windows.net" -Database "noisecap-db" -AccessToken $token -Query @"
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'noisecap-web')
    CREATE USER [noisecap-web] FROM EXTERNAL PROVIDER;
ALTER ROLE db_owner ADD MEMBER [noisecap-web];
"@
