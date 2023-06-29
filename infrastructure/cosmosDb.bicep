@description('Azure Cosmos DB account name, max length 44 characters')
param name string

@description('Location for the Azure Cosmos DB account.')
param region string = resourceGroup().location

@description('The primary region for the Azure Cosmos DB account.')
param primaryRegion string = resourceGroup().location

@description('The secondary region for the Azure Cosmos DB account.')
param secondaryRegion string = 'swedencentral'

@allowed([
  'Eventual'
  'ConsistentPrefix'
  'Session'
  'BoundedStaleness'
  'Strong'
])
@description('The default consistency level of the Cosmos DB account.')
param defaultConsistencyLevel string = 'Session'

@description('Specifies the MongoDB server version to use.')
@allowed([
  '3.2'
  '3.6'
  '4.0'
  '4.2'
])
param serverVersion string = '4.2'


var maxStalenessPrefix = 100000
var maxIntervalInSeconds = 300

var consistencyPolicy = {
  Eventual: {
    defaultConsistencyLevel: 'Eventual'
  }
  ConsistentPrefix: {
    defaultConsistencyLevel: 'ConsistentPrefix'
  }
  Session: {
    defaultConsistencyLevel: 'Session'
  }
  BoundedStaleness: {
    defaultConsistencyLevel: 'BoundedStaleness'
    maxStalenessPrefix: maxStalenessPrefix
    maxIntervalInSeconds: maxIntervalInSeconds
  }
  Strong: {
    defaultConsistencyLevel: 'Strong'
  }
}

var locations = [
  {
    locationName: primaryRegion
    failoverPriority: 0
    isZoneRedundant: false
  }
  {
    locationName: secondaryRegion
    failoverPriority: 1
    isZoneRedundant: false
  }
]

resource account 'Microsoft.DocumentDB/databaseAccounts@2022-05-15' = {
  name: toLower(name)
  location: region
  kind: 'MongoDB'
  properties: {
    consistencyPolicy: consistencyPolicy[defaultConsistencyLevel]
    locations: locations
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: true
    apiProperties: {
      serverVersion: serverVersion
    }
    capabilities: [
      {
        name: 'DisableRateLimitingResponses'
      }
    ]
  }
}
