# Workflows and Actions

This repository contains three github workflows, and three actions. The workflows are set up with OpenID Connect authentication to Azure ([see here](https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure-openid-connect)). The workflows are environment specific, and requires the following secrets to be set in the repository:


|         **Name**        |                                                           **Description**                                                           |                                               **Example**                                              |
|:-----------------------:|:-----------------------------------------------------------------------------------------------------------------------------------:|:------------------------------------------------------------------------------------------------------:|
| AZURE_CLIENT_ID       | ClientId of the app in Azure with federated access to this repository | 7d2ecf9e-195d-43b9-94f0-a136e40641aa |
| AZURE_SUBSCRIPTION_ID       | SubscriptionId of the Azure subscription where the app will be deployed | 60bdc055-f3bb-40f5-9712-02949a143e2a |
| AZURE_TENANT_ID       | TenantId where the app will be deployed | 4a4fe840-4aca-4ccd-bf88-4c629cd7b542 |
| AZURE_RESOURCE_GROUP                | Name of the resource group where your resources will be created                                                                     | rg-explore-dev                                                                                         |
| WEBAPP_DEPLOYMENT_TOKEN | Deployment token for your web app                                                                                                   | <>                                                                                                     |

`AZURE_CLIENT_ID` is the clientId of the app with federated access to this repository (see link above).

The `WEBAPP_DEPLOYMENT_TOKEN` was obtained on the resource in Azure _after_ the static web app was deployed.

In addition to these secrets, one will have to set secrets for the web app deployment:


|         **Name**        |                                                           **Description**                                                           |                                               **Example**                                              |
|:-----------------------:|:-----------------------------------------------------------------------------------------------------------------------------------:|:------------------------------------------------------------------------------------------------------:|
| VITE_API_BASE_URL   | Base url of the deployed api                                      | https://my.api.example.com/api                         |
| VITE_API_SCOPE      | Scope to be requested by the frontend for access to the api       | api://60bdc055-f3bb-40f5-9712-02949a143e2a/Api.Execute |
| VITE_AUTH_CLIENT_ID | ClientId of the AzureAd application registration for the frontend | 4a4fe840-4aca-4ccd-bf88-4c629cd7b542                   |

## Actions

The actions are created generic, so that they can be reused for different web apps and apis.

### Build web app

Builds web app with `npm run build`. 

Workflow file: [Build web workflow](../.github/workflows/build-webapp.yaml).

### Deploy web app

Builds and deploys web app to Azure using [Azure/static-web-apps-deploy](https://github.com/Azure/static-web-apps-deploy).

Action file: [Deploy web app action](../.github/actions/deploy-web-app/action.yaml)

|    **Parameter name**     |                            **Description**                             |              **Example**               |
|---------------------------|------------------------------------------------------------------------|----------------------------------------|
| web_app_location          | The location of the web app                                            | project/web                            |
| web_app_artifact_location | The location of the web app artifact, relative to the web app location | dist                                   |
| github_token              | The token to use to login to GitHub                                    | ${{ secrets.GITHUB_TOKEN }}            |
| deployment_token          | The token to use to deploy the web app                                 | ${{ secrets.WEBAPP_DEPLOYMENT_TOKEN }} |
| client_id                 | Azure ClientId with federated access to this repository                | ${{ secrets.AZURE_CLIENT_ID }}         |
| tenant_id                 | TenantId where the app will be deployed                                | ${{ secrets.AZURE_TENANT_ID }}         |
| subscription_id           | SubscriptionId of the subscription where the app will be deployed      | ${{ secrets.SUBSCRIPTION_ID }}         |

### Infrastructure

Deploys, or verifies, infrastructure to Azure using [azure/arm-deploy](https://github.com/Azure/arm-deploy).

Action file: [Deploy infrastructure action](../.github/actions/infrastructure/action.yaml)

| **Parameter name** |              **Description**             |               **Example**               |
|:------------------:|:----------------------------------------:|:---------------------------------------:|
| deploy             | Whether to deploy the infrastructure                     | "true"                                  |
| resource_group     | The resource group to deploy to                          | ${{ secrets.AZURE_RG }}                 |
| subscription_id    | The subscription to deploy to                            | ${{ secrets.AZURE_SUBSCRIPTION }}       |
| parameter_file     | The parameter file to use                                | ./infrastructure/parameters/common.json |
| environment        | The environment to deploy to                             | dev                                     |
| template_file      | The template file to use                                 | ./infrastructure/main.bicep             |
| tenant_id          | TenantId where the app will be deployed                  | ${{ secrets.AZURE_TENANT_ID }}          |
| client_id          | Azure ClientId with federated access to this repository  | ${{ secrets.AZURE_CLIENT_ID }}          |
| deployment_name    | Name of the deployment                                   | api-${{ github.run_id }}                |


## Workflows

The workflows are created to be used for the Graduate case 2024. For each web app or api, one should duplicate the workflow and change parameters.

### Verify or deploy infrastructure

Runs on every PR to the main branch, and on every push to the main branch _when the_ [infrastructure files](../infrastructure/) _have changed_. On PRs, it will _only verify_ that the infrastructure can be deployed (parameters, syntax, etc...), and not actually deploy it. On pushes to the main branch, it _will deploy_ the infrastructure to the specified environment and resource group. This is specified with the `deploy` parameter to the [infrastructure action](#infrastructure) action.

The workflow uses the [infrastructure action](#infrastructure) to verify or deploy the infrastructure.

This workflow can also be triggered manually. When triggering manually, you can specify the environment and whether to deploy or not.

### Build and test webapp

Runs on every PR to the main branch, _when the_ [web app files](../excursion/web/) _have changed_. It will build the web app, and run tests. 

The workflow uses the [build web app action](#build-web-app) to build the web app.

This workflow can also be triggered manually.

### Build and deploy webapp

Runs on every push to the main branch, _when the_ [web app files](../exursion/web/) _have changed_. It will build the web app, and deploy it to Azure.

The workflow uses the [deploy web app action](#deploy-web-app) to build and deploy the web app.

This workflow can also be triggered manually.
