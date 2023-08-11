# Company api

## Introduction

This project contain the Company API.

## Getting Started

### Prerequisites

1. [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2. [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio)
3. [Azure function core tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=macos%2Cportal%2Cv2%2Cbash&pivots=programming-language-csharp#install-the-azure-functions-core-tools)

### Configuration

Add a `local.settings.json` file with the following content in the API project's root folder; [./Company.Api](./Company.Api/)

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "CmsOptions:BaseUrl": "<your cms url>"
  },
  "Host": {
    "LocalHttpPort": 7072,
    "CORS": "*"
  }
}
```

See the [./Company.Api/local.settings.example.json](./Company.Api/local.settings.example.json) file for an example.


### Running the project

Navigate to the [./Company.Api](./Company.Api) project. Then, make sure [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio) is running. See [Microsoft's guide](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio#run-azurite) on how to start Azurite.

Then, start the function app with the following command:

```bash
func host start --csharp
```

The function should then be running on [http://localhost:7071](http://localhost:7071). Make sure that the CMS is running as well.


You can see if the function started up correctly by navigating to [http://localhost:7071/api/health](http://localhost:7071/api/health). You should see a response like this:

```json
Up and running!

CMS status: Up and running!
```

OpenAPI is available at [http://localhost:7071/api/swagger/ui](http://localhost:7071/api/swagger/ui).


**_Alternatively_**, you can open the solution in Visual Studio and run the project from there, **_OR_** you can run the project from Visual Studio Code by opening the project in VS Code and attatching the project to the degugger in the debug menu.
