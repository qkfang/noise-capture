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

1. Each new entry is stored in Azure SQL through Entity Framework Core.
2. The app applies pending Entity Framework migrations during startup.
3. JSON export remains available from the list page and is generated from database rows.

## Configuration

`src/NoiseCapture.Web/appsettings.json`

- `ConnectionStrings:NoiseCaptureDatabase` - SQL Server connection string used by Entity Framework Core

## Infrastructure (`bicep`)

Bicep now provisions and configures:

- App Service plan + Linux Web App (.NET 10)
- Log Analytics + Application Insights
- Azure SQL Server + DTU-based Azure SQL Database
- Web app connection string for the SQL database

## Deploy

- Infrastructure workflow: `.github/workflows/10_deploy_iac.yml`
- App workflow: `.github/workflows/20_deploy_app.yml`

Set the `AZURE_SQL_ADMIN_PASSWORD` GitHub secret before running the infrastructure deployment workflow.
