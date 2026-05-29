# noise-capture

This project collects and tracks household noise impact from nearby hotel equipment.

## Web app (`src/NoiseCapture.Web`)

A .NET 10 Razor Pages app provides a mobile-friendly form for noise impact entries:

- Sydney date/time capture (`Australia/Sydney`)
- Noise source checkboxes: club roof vent, A/C units, roof vent, wall vent
- Intensity and feeling levels: extreme, high, medium, low
- Location: living room or bedroom
- Notes field
- Prefill from the latest saved entry
- Reset button for fast re-entry

### Data flow

1. Each new entry is appended to a local JSON file (`noise-log.json`) in `LocalData:FolderPath`.
2. The same JSON file is then uploaded to Azure Blob Storage.

## Configuration

`src/NoiseCapture.Web/appsettings.json`

- `LocalData:FolderPath` - local folder for JSON persistence
- `NoiseStorage:AccountUrl` - blob service URL (`https://<account>.blob.core.windows.net`)
- `NoiseStorage:ContainerName` - blob container name
- `NoiseStorage:BlobName` - blob file name

## Infrastructure (`bicep`)

Bicep now provisions and configures:

- App Service plan + Linux Web App (.NET 10)
- Log Analytics + Application Insights
- Storage account + `noise-logs` blob container
- Web app app settings for local data path and blob configuration
- RBAC role assignment: `Storage Blob Data Contributor` for the web app managed identity

## Deploy

- Infrastructure workflow: `.github/workflows/10_deploy_iac.yml`
- App workflow: `.github/workflows/20_deploy_app.yml`
