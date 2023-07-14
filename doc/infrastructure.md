# Infrastructrure with Bicep

The infrastructure for the project is defined in the `infrastructure` directory. The infrastructure is defined using Bicep templates. The main infrastructure template is the [main.bicep](../infrastructure/main.bicep) file.

## Deployment

The infrastructure can either be deployed through the GitHub Actions [infrastructure workflow](../.github/workflows/verify-or-deploy-infrastructure.yaml) (see [workflows.md](./infrastructure.md)), or manually using the Azure CLI:

Make sure [azure cli](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) is installed.


1. Create a resource group

Either in the Azure portal, or using the Azure CLI:

```bash
az group create --name <rg name> --location <location>
```

2. Login to the account where your subscription is

```bash
az login
```

3. Set subscription

```bash
az account set --subscription "<subscription-id>"
```

4. Edit the parameters in the parameter file to fit your needs
5. Run the deployment command

```bash
az deployment group create --resource-group <rg name> --template-file main.bicep --parameters ./parameters/common.json
```

## Resources

The infrastructure consists of the following resources:

- Storage account
- Function app
- Cosmos DB account
- Application insights
- App service plan
- Static web app

## Parameters

The infrastructure is parameterized using the [common.json](../infrastructure/parameters/common.json) file. The parameters are:

- `environment`: The environment to deploy to. Used to create unique names for resources.
  - Must be one of `dev`, `test`, `prod`.
- `appName`: The name of the app. Used to create unique names for the static web app and function app.
  - No restrictions on the name.
- `projectName`: The name of the project. Used to create unique names for all resources.
  - Max length is 17 characters.
- `stAccountName`: The name of the storage account. Used to create unique name for the storage account.
  - Max length is 19 characters.

Example:

```json
{
  "environment": "dev",
  "appName": "IteraProject",
  "projectName": "project",
  "stAccountName": "iterabaseproject"
}
```

Will produce the following resources:

- Storage account: `stiterabaseproject001`
- Function app: `func-IteraProject-dev-{region}-001`
- Cosmos DB account: `cosmos-project-dev-westeurope-001`
- Application insights: `appi-project-dev-{region}-001`
- App service plan: `asp-project-dev-{region}-001`
- Static web app: `stapp-IteraProject-dev-{region}-001`

Where `{region}` is the region of the resource group the resource is deployed to.
