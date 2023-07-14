# Workflows and Actions

This repository contains three github workflows, and three actions. The workflows are environment specific, and requires the following secrets to be set in the repository:


|         **Name**        |                                                           **Description**                                                           |                                               **Example**                                              |
|:-----------------------:|:-----------------------------------------------------------------------------------------------------------------------------------:|:------------------------------------------------------------------------------------------------------:|
| AZURE_CREDENTIALS       | JSON object containing clientId, clientSecret, subscriptionId, and tenantId for an Azure service principal. Used for login to Azure | ```{   "clientId": "",   "clientSecret": "<>",   "subscriptionId": "<>",   "tenantId": "<>" } ``` |
| AZURE_RG                | Name of the resource group where your resources will be created                                                                     | rg-explore-dev                                                                                         |
| AZURE_SUBSCRIPTION      | Azure subscriptionId for the subscription where your resources will be created                                                      | b8fad83e-2fb5-4291-a386-2aec11d2c173                                                                   |
| WEBAPP_DEPLOYMENT_TOKEN | Deployment token for your web app                                                                                                   | <>                                                                                                     |

The `AZURE_CREDENTIALS` for this project was obtained by creating a service principal in Azure, which have contributor role to the resource group, by running the following commands:

```bash
az login
az account set --subscription "<subscription-id>"
az ad sp create-for-rbac --name <sp-name> --role contributor --scopes /subscriptions/{subscription-id}/resourceGroups/<rg-name> --sdk-auth
```

This can also be done in the Azure portal.

The `WEBAPP_DEPLOYMENT_TOKEN` was obtained on the resource in Azure _after_ the static web app was deployed.

## Actions

The actions are created generic, so that they can be reused for different web apps and apis.

### Build web app

Builds web app with `npm run build`.

Action file: [Build web app action](../.github/actions/build-web-app/action.yaml)

| **Parameter name** |       **Description**       | **Example** |
|--------------------|-----------------------------|-------------|
| web_app_location   | The location of the web app | project/web |

### Deploy web app

Builds and deploys web app to Azure using [Azure/static-web-apps-deploy](https://github.com/Azure/static-web-apps-deploy).

Action file: [Deploy web app action](../.github/actions/deploy-web-app/action.yaml)

|    **Parameter name**     |                            **Description**                             |              **Example**               |
|---------------------------|------------------------------------------------------------------------|----------------------------------------|
| web_app_location          | The location of the web app                                            | project/web                            |
| web_app_artifact_location | The location of the web app artifact, relative to the web app location | dist                                   |
| azure_credentials         | The credentials to use to login to Azure                               | ${{ secrets.AZURE_CREDENTIALS }}       |
| github_token              | The token to use to login to GitHub                                    | ${{ secrets.GITHUB_TOKEN }}            |
| deployment_token          | The token to use to deploy the web app                                 | ${{ secrets.WEBAPP_DEPLOYMENT_TOKEN }} |

### Infrastructure

Deploys, or verifies, infrastructure to Azure using [azure/arm-deploy](https://github.com/Azure/arm-deploy).

Action file: [Deploy infrastructure action](../.github/actions/infrastructure/action.yaml)

| **Parameter name** |              **Description**             |               **Example**               |
|:------------------:|:----------------------------------------:|:---------------------------------------:|
| deploy             | Whether to deploy the infrastructure     | "true"                                  |
| resource_group     | The resource group to deploy to          | ${{ secrets.AZURE_RG }}                 |
| subscription_id    | The subscription to deploy to            | ${{ secrets.AZURE_SUBSCRIPTION }}       |
| parameter_file     | The parameter file to use                | ./infrastructure/parameters/common.json |
| environment        | The environment to deploy to             | dev                                     |
| template_file      | The template file to use                 | ./infrastructure/main.bicep             |
| azure_credentials  | The credentials to use to login to Azure | ${{ secrets.AZURE_CREDENTIALS }}        |

## Workflows

The workflows are created to be used for the Graduate case 2023. For each web app or api, one should duplicate the workflow and change parameters.

### Verify or deploy infrastructure

Runs on every PR to the main branch, and on every push to the main branch _when the_ [infrastructure files](../infrastructure/) _have changed_. On PRs, it will _only verify_ that the infrastructure can be deployed (parameters, syntax, etc...), and not actually deploy it. On pushes to the main branch, it _will deploy_ the infrastructure to the specified environment and resource group. This is specified with the `deploy` parameter to the [infrastructure action](#infrastructure) action.

The workflow uses the [infrastructure action](#infrastructure) to verify or deploy the infrastructure.

This workflow can also be triggered manually. When triggering manually, you can specify the environment and whether to deploy or not.

### Build and test webapp

Runs on every PR to the main branch, _when the_ [web app files](../project/web/) _have changed_. It will build the web app, and run tests. 

The workflow uses the [build web app action](#build-web-app) to build the web app.

This workflow can also be triggered manually.

### Build and deploy webapp

Runs on every push to the main branch, _when the_ [web app files](../project/web/) _have changed_. It will build the web app, and deploy it to Azure.

The workflow uses the [deploy web app action](#deploy-web-app) to build and deploy the web app.

This workflow can also be triggered manually.
