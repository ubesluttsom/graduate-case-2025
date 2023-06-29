@description('Static web app name')
param name string

@description('Static web app region')
param region string = 'westeurope'

resource staticWebApp 'Microsoft.Web/staticSites@2021-01-15' = {
    name: name
    location: region
    tags: null
    properties: {}
    sku: {
        name: 'free'
        size: 'free'
    }
}

output staticWebAppName string = staticWebApp.name
