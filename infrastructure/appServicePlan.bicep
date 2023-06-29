@description('App sercice plan name')
param name string

@description('Region')
param region string

resource hostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: name
  location: region
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {}
}
