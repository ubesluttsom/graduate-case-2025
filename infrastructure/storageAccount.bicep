@description('The name of the storage account')
param name string

@description('The region where the storage account will be created')
param region string

@description('The sku of the storage account')
param sku string = 'Standard_LRS'

@description('The name of a container to create in the storage account')
param containerName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: name
  location: region
  sku: {
    name: sku
  }
  kind: 'Storage'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  name: 'default'
  parent: storageAccount
}

resource defaultContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  name: containerName
  parent: blobService
  properties: {
    publicAccess: 'None'
    metadata: {}
  }
}

resource imagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  name: 'images'
  parent: blobService
  properties: {
    publicAccess: 'Blob'
    metadata: {}
  }
}
