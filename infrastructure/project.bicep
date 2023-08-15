@description('The name of the app')
param appName string

@description('Environment to deploy to.')
@allowed([
  'dev'
  'test'
  'prod'
])
param environment string

@description('Name of the storage account to create.')
@maxLength(19)
param stAccountName string

@description('Location for all resources.')
param region string = resourceGroup().location

@description('Instance of the resources.')
param instance string = '001'

@description('Location for the Static Web App.')
param swaRegion string = 'westeurope'

@description('Location for the Cosmos DB.')
param cosmosRegion string = 'westeurope'

@description('Name of the project.')
@maxLength(17)
param projectName string = 'project'

var swaName = 'stapp-${appName}-${environment}-${region}-${instance}'

module commonModules 'common.bicep' = {
  name: 'commonModules'
  params: {
    appName: appName
    environment: environment
    region: region
    instance: instance
    stAccountName: stAccountName
    cosmosRegion: cosmosRegion
    projectName: projectName
  }
}


module staticWebApp 'staticWebApp.bicep' = {
  name: swaName
  params: {
    name: swaName
    region: swaRegion
  }
}

output staticWebAppName string = staticWebApp.outputs.staticWebAppName
