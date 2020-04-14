# IO Config Demo
Demo application showcasing Configuration with Design Automation for Inventor

## Prerequisites

1. .NET Core 3.1
1. Node.js
1. _(if you want to build app bundles)_ Autodesk Inventor 2021

## Setup
1. Create a forge app at https://forge.autodesk.com/, and select the Design Automation V3 API
1. Note the Client ID and Secret generated.
1. In order to debug the solution in VS Code you have to install the extension Debugger for Chrome

## Build the solution

1. Clone repository
1. Specify [forge credentials](#specify-forge-credentials).
1. From a command prompt, go to the root directory, and run `dotnet build`. This builds the projects and also installs required packages (can take several minutes).

## Run The App Without Debugging

1. From a command prompt, go to the `WebApplication` directory, and run `dotnet run`
1. Open https://localhost:5001

## Debug The App With VS Code

1. Open the repository root folder in VS Code
1. In the Run tab, select the `Server/Client` configuration and click the "Start Debugging" (arrow) button

### Clear and load initial data during app launch time

 - Create initial data: from the `WebApplication` directory, run `dotnet run initialize=true`
 - Clear data: from the `WebApplication` directory, run `dotnet run clear=true`
 - Clear and then load initial data: from the `WebApplication` directory, run `dotnet run initialize=true clear=true`

### Specify Forge credentials
Use one of the following approaches:
* Set environment variables `FORGE_CLIENT_ID` and `FORGE_CLIENT_SECRET`.
* Create `appsettings.Local.json` in root dir and use the following as its content template:
```json
{
    "Forge": {
        "clientId": "<YOUR CLIENT ID>",
        "clientSecret": "<YOUR CLIENT SECRET>"
       }
}
```
* Set environment variables `Forge__ClientId` and `Forge__ClientSecret`.
* _(not on dev machine)_ Modify `appsettings.json` (or `appsettings.<ENVIRONMENT>.json`) with the template above.

## Backend
We are using the forge service on the backend https://forge.autodesk.com/

### .NET Core
We are using the latest version of .NET Core (3.1 at the time of writing) https://dotnet.microsoft.com/download/dotnet-core/3.1

The project was initally created using the command `dotnet new react`

### Forge Design Automation
https://forge.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/

C# SDK https://github.com/Autodesk-Forge/forge-api-dotnet-design.automation

We are using the Inventor and Revit engines.

### OSS
Used for storing your designs. For communication with Forge Design Automation

https://forge.autodesk.com/en/docs/data/v2/developers_guide/overview/

C# SDK https://github.com/Autodesk-Forge/forge-api-dotnet-client

## Client app

### React
https://reactjs.org/

### Redux
https://redux.js.org/

We are using redux-thunk for complex and asynchronous operations https://github.com/reduxjs/redux-thunk

### Autodesk HIG React

https://github.com/Autodesk/hig

Table is not part of todays React HIG implementation so we will use https://github.com/Autodesk/react-base-table

## How to

### Add npm package to project

We are using npm.

1. Using command line go to WebApplication/ClientApp and run `npm install <package>`
    * Note that packages are normally installed as part of the build, but only if the `npm_modules` directory is not found. This means that when new packages are added, `WebApplication/ClientApp/npm install` needs to be run again manually by other users (who did not add the new package).

### Run unit/integration tests for client app

1. Using command line go to WebApplication/ClientApp and execute `npm run test`
    * Note that the server needs to be running for integration tests
