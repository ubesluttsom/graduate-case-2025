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

@description('Location for the Cosmos DB.')
param cosmosRegion string = 'westeurope'

@description('Name of the project.')
@maxLength(17)
param projectName string = 'project'

var hostingPlanName = 'asp-${projectName}-${environment}-${region}-${instance}'
var cosmosDbName = 'cosmos-${projectName}-${environment}-${cosmosRegion}-${instance}'
var storageAccountName = toLower('st${stAccountName}${instance}')

module storageAccount 'storageAccount.bicep' = {
  name: storageAccountName
  params: {
    name: storageAccountName
    region: region
    containerName: toLower(appName)
  }
}

module cosmosDb 'cosmosDb.bicep' = {
  name: cosmosDbName
  params: {
    name: cosmosDbName
    region: cosmosRegion
    primaryRegion: cosmosRegion
  }
}

module hostingPlan 'appServicePlan.bicep' = {
  name: hostingPlanName
  params: {
    name: hostingPlanName
    region: region
  }
}
