@description('Name of the Application Insights instance')
param name string

@description('Azure region where the Application Insights instance should be created')
param region string

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: name
  location: region
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

output instrumentationKey string = applicationInsights.properties.InstrumentationKey
