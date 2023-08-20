@description('The name of the function app')
param name string

@description('The region where the function app will be created')
param region string

@description('The hosting plan id')
param hostingPlanName string

@description('The storage account name')
param storageAccountName string

@description('Application insights name')
param applicationInsightsName string

@description('Cosmos DB name')
param cosmosDbName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
  name: storageAccountName
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: applicationInsightsName
}

resource functionAppPlan 'Microsoft.Web/serverfarms@2021-02-01' existing = {
  name: hostingPlanName
}

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: toLower(cosmosDbName)
}

resource functionApp 'Microsoft.Web/sites@2022-09-01' = {

  name: toLower(name)
  location: region
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  
  properties: {
    serverFarmId: functionAppPlan.id
    siteConfig: {
      connectionStrings: [
        {
          name: 'CosmosDB'
          connectionString: cosmosDb.listConnectionStrings().connectionStrings[0].connectionString
          type: 'Custom'
        }
      ]
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}

module appSettings 'appsettings.bicep' = {
  name: '${functionApp.name}-appsettings'
  params: {
    appName: functionApp.name
    currentAppSettings: list(resourceId('Microsoft.Web/sites/config', functionApp.name, 'appsettings'), '2022-09-01').properties
    appSettings: {
      AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
      WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
      WEBSITE_CONTENTSHARE: toLower(name)
      FUNCTIONS_EXTENSION_VERSION: '~4'
      WEBSITE_NODE_DEFAULT_VERSION: '~18'
      APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsights.properties.InstrumentationKey
      FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    }
  }
}

output functionAppName string = functionApp.name
