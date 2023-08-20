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

@description('Deploy Static Web App?')
param deploySwa bool = true

@description('Location for the Static Web App.')
param swaRegion string = 'westeurope'

@description('Location for the Cosmos DB.')
param cosmosRegion string = 'westeurope'

@description('Name of the project.')
@maxLength(17)
param projectName string = 'project'

var funcAppName = 'func-${appName}-${environment}-${region}-${instance}' 
var hostingPlanName = 'asp-${projectName}-${environment}-${region}-${instance}'
var applicationInsightsName = 'appi-${appName}-${environment}-${region}-${instance}'
var cosmosDbName = 'cosmos-${projectName}-${environment}-${cosmosRegion}-${instance}'
var storageAccountName = toLower('st${stAccountName}${instance}')
var swaName = 'stapp-${appName}-${environment}-${region}-${instance}'

module applicationInsights 'applicationInsights.bicep' = {
  name: applicationInsightsName
  params: {
    name: applicationInsightsName
    region: region
  }
}

module functionApp 'functionApp.bicep' = {
  name: funcAppName
  params: {
    name: funcAppName
    hostingPlanName: hostingPlanName
    region: region
    storageAccountName: storageAccountName
    applicationInsightsName: applicationInsightsName
    cosmosDbName: cosmosDbName
  }
  dependsOn: [
    applicationInsights
  ]
}

module staticWebApp 'staticWebApp.bicep' = if (deploySwa) {
  name: swaName
  params: {
    name: swaName
    region: swaRegion
  }
}
