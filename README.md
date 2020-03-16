# IO Config Demo
Demo application showcasing Configuration with Design Automation for Inventor

## Run the app

1. Clone repository
1. Go inside the repository
1. `dotnet run`

### Backend
We are using forge service on the backend https://forge.autodesk.com/

#### .NET Core

Using latest version of .NET Core (now 3.1).
Project is created using `dotnet new react`

#### Forge Design Automation
https://forge.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/

C# SDK https://github.com/Autodesk-Forge/forge-api-dotnet-design.automation

We are using Inventor and Revit engine. 

#### OSS
Used for storing your designs. For communication with Forge Design Automation

https://forge.autodesk.com/en/docs/data/v2/developers_guide/overview/

C# SDK https://github.com/Autodesk-Forge/forge-api-dotnet-client

### Client app

#### React
https://reactjs.org/

#### Redux
https://redux.js.org/

We are using redux-thunk for complex and asynchronous operations https://github.com/reduxjs/redux-thunk

#### Autodesk HIG React

https://github.com/Autodesk/hig

Table is not part of todays React HIG implementation so we will use https://github.com/Autodesk/react-base-table

### 