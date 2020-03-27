# IO Config Demo
Demo application showcasing Configuration with Design Automation for Inventor

## Setup
1. Create forge app at https://forge.autodesk.com/
1. Note Client ID and Secret.

### VSCode extensions
* In order to debug the solution in VS Code you have to install the extension [Debugger for Chrome](https://marketplace.visualstudio.com/items?itemName=msjsdiag.debugger-for-chrome)
* [ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint)
* [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode)

## Run the app

1. Clone repository
1. Go inside the repository
1. Specify [forge credentials](#specify-forge-credentials).
1. `dotnet run`
1. Open https://localhost:5001

### Clear and load initial data during app launch time

 - Create initial data `dotnet run initialize=true`
 - Clear data `dotnet run clear=true`
 - Clear and then load initial data `dotnet run initialize=true clear=true`

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
We are using forge service on the backend https://forge.autodesk.com/

### .NET Core
Using latest version of .NET Core (now 3.1) https://dotnet.microsoft.com/download/dotnet-core/3.1

Project is created using `dotnet new react`

### Forge Design Automation
https://forge.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/

C# SDK https://github.com/Autodesk-Forge/forge-api-dotnet-design.automation

We are using Inventor and Revit engine. 

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

1. Using command line go to ClientApp and write `npm install <package>`

### Run unit/integration tests for client app

1. Using command line go to ClientApp and execute `npm run test`
